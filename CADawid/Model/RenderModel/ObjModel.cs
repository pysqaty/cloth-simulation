using CADawid.DxModule;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using SharpDX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Model
{
    public class ObjModel : GeometryObject<VertexNormal, Index>
    {
        private ObjReader objReader { get; set; }
        private List<Object3D> objects { get; set; }

        public Point[,,] Nodes { get; set; }

        private BoundingBox boundingBox { get; set; }

        public ObjModel(string path)
        {
            objReader = new ObjReader();
            objects = objReader.Read(path);

            var bb = objects[0].Geometry.Bound;

            float xLength = bb.Maximum.X - bb.Minimum.X;
            float yLength = bb.Maximum.Y - bb.Minimum.Y;
            float zLength = bb.Maximum.Z - bb.Minimum.Z;

            if(xLength >= yLength && xLength >= zLength)
            {
                float yOffset = (xLength - yLength) / 2;
                float zOffset = (xLength - zLength) / 2;

                boundingBox = new BoundingBox(new Vector3(bb.Minimum.X, bb.Minimum.Y - yOffset, bb.Minimum.Z - zOffset),
                    new Vector3(bb.Maximum.X, bb.Maximum.Y + yOffset, bb.Maximum.Z + zOffset));
            }
            else if(yLength >= xLength && yLength >= zLength)
            {
                float xOffset = (yLength - xLength) / 2;
                float zOffset = (yLength - zLength) / 2;

                boundingBox = new BoundingBox(new Vector3(bb.Minimum.X - xOffset, bb.Minimum.Y, bb.Minimum.Z - zOffset),
                    new Vector3(bb.Maximum.X + xOffset, bb.Maximum.Y, bb.Maximum.Z + zOffset));
            }
            else
            {
                float xOffset = (zLength - xLength) / 2;
                float yOffset = (zLength - yLength) / 2;

                boundingBox = new BoundingBox(new Vector3(bb.Minimum.X - xOffset, bb.Minimum.Y - yOffset, bb.Minimum.Z),
                    new Vector3(bb.Maximum.X + xOffset, bb.Maximum.Y + yOffset, bb.Maximum.Z));
            }

        }
        protected override Geometry<VertexNormal, Index> GenerateGeometry()
        {
            var obj = objects[0].Geometry as MeshGeometry3D;
            VertexNormal[] vertices = new VertexNormal[obj.Positions.Count];
            Index[] indices = new Index[obj.Indices.Count];

            for(int i = 0; i < obj.Positions.Count; i++)
            {
                vertices[i] = new VertexNormal(obj.Normals[i], AdjustToJellybean(boundingBox, obj.Positions[i]));
            }

            for(int i = 0; i < obj.Indices.Count; i++)
            {
                indices[i] = new Index((UInt32)obj.Indices[i]);
            }

            return new Geometry<VertexNormal, Index>(vertices, indices);
        }

        Vector4 AdjustToJellybean(BoundingBox bb, Vector3 position)
        {
            float x = (position.X - bb.Minimum.X) / (bb.Maximum.X - bb.Minimum.X);
            float y = (position.Y - bb.Minimum.Y) / (bb.Maximum.Y - bb.Minimum.Y);
            float z = (position.Z - bb.Minimum.Z) / (bb.Maximum.Z - bb.Minimum.Z);

            return new Vector4(x, y, z, 1);
        }

        public override void Render(DxRenderer dxRenderer)
        {
            if (!Visible)
            {
                return;
            }
            dxRenderer.device.ImmediateContext.InputAssembler.InputLayout = dxRenderer.phongInputLayout;
            dxRenderer.device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            Geometry<VertexNormal, Index> g = TryUpdateGeometry(dxRenderer);
            if (g.Indices.Length == 0 || g.Vertices.Length == 0)
            {
                return;
            }
            unsafe
            {
                dxRenderer.SetVertexBuffer<VertexNormal>(vertexBuffer, sizeof(VertexNormal));
            }
            dxRenderer.SetIndexBuffer(indexBuffer, SharpDX.DXGI.Format.R32_UInt);


            Matrix modelMatrix = CurrentModel;
            DxConstantBuffer cb = new DxConstantBuffer();
            cb.MVP = modelMatrix * dxRenderer.Scene.Camera.VP;
            cb.Color = Color;

            dxRenderer.device.ImmediateContext.UpdateSubresource<DxConstantBuffer>(ref cb, dxRenderer.constantBuffer);

            DxViewConstantBuffer vcb = new DxViewConstantBuffer()
            {
                InvView = dxRenderer.Scene.Camera.InvView,
            };

            dxRenderer.device.ImmediateContext.UpdateSubresource<DxViewConstantBuffer>(ref vcb, dxRenderer.viewConstantBuffer);

            dxRenderer.device.ImmediateContext.VertexShader.Set(dxRenderer.phongVertexShader);
            dxRenderer.device.ImmediateContext.VertexShader.SetConstantBuffer(0, dxRenderer.constantBuffer);
            dxRenderer.device.ImmediateContext.PixelShader.Set(dxRenderer.phongPixelShader);
            dxRenderer.device.ImmediateContext.HullShader.Set(null);
            dxRenderer.device.ImmediateContext.DomainShader.Set(null);

            float[] controlpoints = new float[64 * 4];
            int i = 0;
            foreach (var node in Nodes)
            {
                controlpoints[i++] = node.Position.X;
                controlpoints[i++] = node.Position.Y;
                controlpoints[i++] = node.Position.Z;
                controlpoints[i++] = 1;
            }
            byte[] byteArray = new byte[controlpoints.Length * sizeof(float)];
            System.Buffer.BlockCopy(controlpoints, 0, byteArray, 0, byteArray.Length);
            dxRenderer.device.ImmediateContext.MapSubresource(dxRenderer.controlPoints, SharpDX.Direct3D11.MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None, out DataStream stream);
            stream.Write(byteArray, 0, byteArray.Length);
            stream.Dispose();
            dxRenderer.device.ImmediateContext.UnmapSubresource(dxRenderer.controlPoints, 0);

            dxRenderer.device.ImmediateContext.DrawIndexed(g.Indices.Length, 0, 0);

            dxRenderer.device.ImmediateContext.InputAssembler.InputLayout = dxRenderer.inputLayout;

        }
    }
}
