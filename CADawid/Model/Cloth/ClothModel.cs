using CADawid.DxModule;
using SharpDX;
using SharpDX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Model.Cloth
{
    public abstract class ClothModel : GeometryObject<VertexNormal, Index>
    {
        public Point[] Vertices { get; set; }
        public List<QuadMesh> Quads { get; set; }
        public Dictionary<int, List<int>> IncidentQuads { get; set; } // map vertex index -> quads indices

        public void AddIncidentTriangle(int p, int k)
        {
            if (IncidentQuads.ContainsKey(p))
            {
                IncidentQuads[p].Add(k);
            }
            else
            {
                IncidentQuads.Add(p, new List<int>() { k });
            }
        }

        protected override Geometry<VertexNormal, Index> GenerateGeometry()
        {
            VertexNormal[] vertices = new VertexNormal[Vertices.Length];
            for (int vi = 0; vi < Vertices.Length; vi++)
            {
                Point v = Vertices[vi];
                Vector3 n = Vector3.Zero;
                foreach (var ti in IncidentQuads[vi])
                {
                    QuadMesh t = Quads[ti];
                    n += -t.GetNormal(Vertices);
                }

                n.Normalize();

                vertices[vi] = new VertexNormal(n, new Vector4(v.Position, 1));
            }

            Index[] indices = new Index[Quads.Count * 3 * 2];

            int k = 0;

            foreach (var t in Quads)
            {
                indices[k++] = new Index((UInt32)t.p1);
                indices[k++] = new Index((UInt32)t.p2);
                indices[k++] = new Index((UInt32)t.p3);
                indices[k++] = new Index((UInt32)t.p3);
                indices[k++] = new Index((UInt32)t.p4);
                indices[k++] = new Index((UInt32)t.p1);
            }

            return new Geometry<VertexNormal, Index>(vertices, indices);
        }

        public override void Render(DxRenderer dxRenderer)
        {
            if (Visible)
            {
                dxRenderer.device.ImmediateContext.InputAssembler.InputLayout = dxRenderer.phongInputLayout;
                dxRenderer.device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                Geometry<VertexNormal, Index> g = TryUpdateGeometry(dxRenderer);
                if (g.Vertices.Length == 0)
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

                DxViewConstantBuffer cb2 = new DxViewConstantBuffer();
                cb2.InvView = dxRenderer.Scene.Camera.InvView;

                dxRenderer.device.ImmediateContext.UpdateSubresource<DxConstantBuffer>(ref cb, dxRenderer.constantBuffer);
                dxRenderer.device.ImmediateContext.UpdateSubresource<DxViewConstantBuffer>(ref cb2, dxRenderer.viewConstantBuffer);

                dxRenderer.device.ImmediateContext.VertexShader.Set(dxRenderer.phongTVertexShader);
                dxRenderer.device.ImmediateContext.PixelShader.Set(dxRenderer.phongTPixelShader);
                dxRenderer.device.ImmediateContext.HullShader.Set(null);
                dxRenderer.device.ImmediateContext.DomainShader.Set(null);
                dxRenderer.device.ImmediateContext.VertexShader.SetConstantBuffer(0, dxRenderer.constantBuffer);
                dxRenderer.device.ImmediateContext.VertexShader.SetConstantBuffer(1, dxRenderer.viewConstantBuffer);

                dxRenderer.device.ImmediateContext.DrawIndexed(g.Indices.Length, 0, 0);

                dxRenderer.device.ImmediateContext.InputAssembler.InputLayout = dxRenderer.inputLayout;
            }
        }
    }
}
