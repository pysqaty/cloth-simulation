using CADawid.DxModule;
using SharpDX;
using SharpDX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Model
{
    public class Sphere : GeometryObject<Vertex, Index>
    {
        public float R { get; set; }
        public int Division { get; set; }

        public Sphere(float r, int division)
        {
            R = r;
            Division = division;
        }
        protected override Geometry<Vertex, Index> GenerateGeometry()
        {
            List<Vertex> vertices = new List<Vertex>();
            float alphaStep = (float)(Math.PI / Division);
            float betaStep = 2 * alphaStep;

            vertices.Add(new Vertex(0, R, 0, 1));
            for (int i = 0; i < Division; i++)
            {
                float alpha = i * alphaStep;
                for(int j = 0; j <= Division; j++)
                {
                    float beta = j * betaStep;
                    var p = new Vector4(
                        (float)(R * Math.Sin(alpha) * Math.Cos(beta)),
                        (float)(R * Math.Cos(alpha)),
                        (float)(R * Math.Sin(alpha) * Math.Sin(beta)), 1);
                    vertices.Add(new Vertex(p));
                }
            }
            vertices.Add(new Vertex(0, -R, 0, 1));

            List<Index> indices = new List<Index>();

            for (int i = 1; i <= Division; i++)
            {
                indices.Add(new Index(0));
                indices.Add(new Index((uint)(i + 1)));
                indices.Add(new Index((uint)(i + 1)));
                indices.Add(new Index((uint)(i)));
                indices.Add(new Index((uint)(i)));
                indices.Add(new Index(0));
            }

            var baseIndex = 1;
            var ringVertexCount = Division + 1;

            for (int i = 0; i < Division - 1; i++)
            {
                for (int j = 0; j < Division; j++)
                {
                    indices.Add(new Index((uint)(baseIndex + i * ringVertexCount + j)));
                    indices.Add(new Index((uint)(baseIndex + i * ringVertexCount + j + 1)));
                    indices.Add(new Index((uint)(baseIndex + i * ringVertexCount + j + 1)));
                    indices.Add(new Index((uint)(baseIndex + (i + 1) * ringVertexCount + j)));
                    indices.Add(new Index((uint)(baseIndex + (i + 1) * ringVertexCount + j)));
                    indices.Add(new Index((uint)(baseIndex + i * ringVertexCount + j)));

                    indices.Add(new Index((uint)(baseIndex + (i + 1) * ringVertexCount + j)));
                    indices.Add(new Index((uint)(baseIndex + i * ringVertexCount + j + 1)));
                    indices.Add(new Index((uint)(baseIndex + i * ringVertexCount + j + 1)));
                    indices.Add(new Index((uint)(baseIndex + (i + 1) * ringVertexCount + j + 1)));
                    indices.Add(new Index((uint)(baseIndex + (i + 1) * ringVertexCount + j + 1)));
                    indices.Add(new Index((uint)(baseIndex + (i + 1) * ringVertexCount + j)));
                }
            }

            var southPoleIndex = vertices.Count - 1;
            baseIndex = southPoleIndex - ringVertexCount;
            for (int i = 0; i < Division; i++)
            {

                indices.Add(new Index((uint)(southPoleIndex)));
                indices.Add(new Index((uint)(baseIndex + i)));
                indices.Add(new Index((uint)(baseIndex + i)));
                indices.Add(new Index((uint)(baseIndex + i + 1)));
                indices.Add(new Index((uint)(baseIndex + i + 1)));
                indices.Add(new Index((uint)(southPoleIndex)));
            }

            return new Geometry<Vertex, Index>(vertices.ToArray(), indices.ToArray());
        }
    }
}
