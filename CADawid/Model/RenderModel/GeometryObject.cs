using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CADawid.DxModule;
using CADawid.Utils;
using SharpDX;
using SharpDX.Direct3D;

namespace CADawid.Model
{
    public abstract class GeometryObject<V,I> : IGeometryObject
        where V : unmanaged
        where I : unmanaged
    {
        public Geometry<V,I> Geometry { get; set; }


        public Matrix Model { get; set; }
        public Vector4 Color { get; set; }
        public bool Visible { get; set; }

        public Vector3 Translation { get; set; }
        public Vector3 Scale { get; set; }

        private Quaternion rotation;
        public Quaternion Rotation
        {
            get => rotation;
            set
            {
                rotation = value;
                eulerAngles = MathExt.QuaternionToEuler(rotation);
            }
        }

        private Vector3 eulerAngles;
        public Vector3 EulerAngles
        {
            get => eulerAngles;
            set
            {
                eulerAngles = value;
                rotation = MathExt.EulerToQuaternion(eulerAngles);
            }
        }

        public virtual Matrix CurrentModel =>
            Matrix.Scaling(Scale) *
            Matrix.RotationQuaternion(Rotation) *
            Matrix.Translation(Translation) *
            Model;


        public const float TranslationVar = 0.02f;
        public const float RotationVar = 0.02f;
        public virtual void Translate(Vector3 translation)
        {
            Translation += TranslationVar * translation;
        }

        public virtual void Rotate(Vector3 axis, float value)
        {
            Rotation =  Quaternion.RotationAxis(axis, value * RotationVar) * Rotation;
        }


        public GeometryObject()
        {
            Visible = true;
            Model = Matrix.Identity;
            Color = new Vector4(1, 1, 1, 1);
            Reset();
        }

        public GeometryObject(Matrix model, Vector4 color)
        {
            Visible = true;
            Model = model;
            Color = color;
            Reset();
        }

        public SharpDX.Direct3D11.Buffer vertexBuffer { get; set; }
        public SharpDX.Direct3D11.Buffer indexBuffer { get; set; }

        protected virtual Geometry<V, I> TryUpdateGeometry(DxRenderer dxRenderer)
        {
            if (Geometry.Equals(default(Geometry<V, I>)))
            {
                Geometry = GenerateGeometry();

                if (Geometry.Indices.Length != 0)
                {
                    unsafe
                    {
                        indexBuffer = dxRenderer.CreateIndexBuffer(Geometry.Indices, sizeof(I));
                    }
                }
                if (Geometry.Vertices.Length != 0)
                {
                    unsafe
                    {
                        vertexBuffer = dxRenderer.CreateVertexBuffer<V>(Geometry.Vertices, sizeof(V));
                    }
                }
            }
            return Geometry;
        }

        protected abstract Geometry<V,I> GenerateGeometry();
        public virtual void ResetGeometry()
        {
            Geometry = default(Geometry<V,I>);
        }

        public void Reset()
        {
            Rotation = Quaternion.Identity;
            Translation = Vector3.Zero;
            Scale = new Vector3(1, 1, 1);
        }

        public virtual void Render(DxRenderer dxRenderer)
        {
            if(!Visible)
            {
                return;
            }
            dxRenderer.device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineList;
            Geometry<V, I> g = TryUpdateGeometry(dxRenderer);
            if (g.Indices.Length == 0 || g.Vertices.Length == 0)
            {
                return;
            }
            unsafe
            {
                dxRenderer.SetVertexBuffer<V>(vertexBuffer, sizeof(V));
            }
            dxRenderer.SetIndexBuffer(indexBuffer, SharpDX.DXGI.Format.R32_UInt);


            Matrix modelMatrix = CurrentModel;
            DxConstantBuffer cb = new DxConstantBuffer();
            cb.MVP = modelMatrix * dxRenderer.Scene.Camera.VP;
            cb.Color = Color;

            dxRenderer.device.ImmediateContext.UpdateSubresource<DxConstantBuffer>(ref cb, dxRenderer.constantBuffer);
            
            dxRenderer.device.ImmediateContext.VertexShader.Set(dxRenderer.vertexShader);
            dxRenderer.device.ImmediateContext.VertexShader.SetConstantBuffer(0, dxRenderer.constantBuffer);
            dxRenderer.device.ImmediateContext.PixelShader.Set(dxRenderer.pixelShader);
            dxRenderer.device.ImmediateContext.HullShader.Set(null);
            dxRenderer.device.ImmediateContext.DomainShader.Set(null);

            dxRenderer.device.ImmediateContext.DrawIndexed(g.Indices.Length, 0, 0);
        }
    }
}
