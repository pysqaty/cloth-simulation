using CADawid.DxModule;
using CADawid.Utils;
using SharpDX;
using SharpDX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Model
{
    public class Torus : GeometryObject<Vertex, Index>
    {
        public float R { get; set; }
        public float r { get; set; }

        public int Precision1 { get; set; }
        public int Precision2 { get; set; }

        public Torus(float R, float r, int prec1, int prec2)
        {
            this.R = R;
            this.r = r;
            Precision1 = prec1;
            Precision2 = prec2;
        }

        protected override Geometry<Vertex, Index> GenerateGeometry()
        {
            int k = 0;
            int l = 0;
            List<Vertex> vertexArray = new List<Vertex>();
            List<Index> edgeArray = new List<Index>();
            int[,] grid = new int[Precision2, Precision1];
            float step1 = 360f / Precision1;
            float step2 = 360f / Precision2;
            float iR = 0f;
            float ir = 0f;
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    Vertex v = Formula(MathExt.ToRadians(ir), MathExt.ToRadians(iR));
                    vertexArray.Add(v);
                    grid[i, j] = k;
                    ir += step1;
                    k++;
                }
                iR += step2;
                ir = 0f;
            }

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    edgeArray.Add(new Index((UInt32)grid[i, j]));
                    edgeArray.Add(new Index((UInt32)grid[((i + 1) % grid.GetLength(0)), j]));
                  
                    edgeArray.Add(new Index((UInt32)grid[i, j]));
                    edgeArray.Add(new Index((UInt32)grid[i, ((j + 1) % grid.GetLength(1))]));
                }
            }

            return new Geometry<Vertex, Index>(vertexArray.ToArray(), edgeArray.ToArray());
        }

        private Vertex Formula(float alfa, float beta)
        {
            float x = (float)((R + r * Math.Cos(alfa)) * Math.Cos(beta));
            float y = (float)(-r * Math.Sin(alfa));
            float z = (float)((R + r * Math.Cos(alfa)) * Math.Sin(beta));
            Vector3 localPos = new Vector3(x, y, z);
            return new Vertex(new Vector4(localPos, 1f));
        }  
    }
}
