using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Model
{
    public class Cube : GeometryObject<Vertex, Index>
    {
        public float SideX { get; set; }
        public float SideY { get; set; }
        public float SideZ { get; set; }
        public Point[,,] Vertices { get; set; }

        public Cube(float side) : this(side, side, side)
        {
            
        }

        public Cube(float sideX, float sideY, float sideZ)
        {
            SideX = sideX;
            SideY = sideY;
            SideZ = sideZ;
            Color = new Vector4(1, 1, 1, 1);
            float calcPosX(int ind) => ((float)ind - 0.5f) * SideX;
            float calcPosY(int ind) => ((float)ind - 0.5f) * SideY;
            float calcPosZ(int ind) => ((float)ind - 0.5f) * SideZ;

            Vertices = new Point[2, 2, 2];
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        float x = calcPosX(i);
                        float y = calcPosY(j);
                        float z = calcPosZ(k);
                        Vertices[i, j, k] = new Point(new Vector3(x, y, z));
                    }
                }
            }
        }

        protected override Geometry<Vertex, Index> GenerateGeometry()
        {
            Vertex[] vertices = new Vertex[8];
            int ind = 0;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        vertices[ind++] = new Vertex(new Vector4(Vertices[k, j, i].Position, 1));
                    }
                }
            }

            Index[] indices = new Index[24];
            ind = 0;
            indices[ind++] = new Index(0);
            indices[ind++] = new Index(1);
            indices[ind++] = new Index(1);
            indices[ind++] = new Index(5);
            indices[ind++] = new Index(5);
            indices[ind++] = new Index(4);
            indices[ind++] = new Index(4);
            indices[ind++] = new Index(0);
            indices[ind++] = new Index(0);
            indices[ind++] = new Index(2);
            indices[ind++] = new Index(1);
            indices[ind++] = new Index(3);
            indices[ind++] = new Index(5);
            indices[ind++] = new Index(7);
            indices[ind++] = new Index(4);
            indices[ind++] = new Index(6);
            indices[ind++] = new Index(2);
            indices[ind++] = new Index(3);
            indices[ind++] = new Index(3);
            indices[ind++] = new Index(7);
            indices[ind++] = new Index(7);
            indices[ind++] = new Index(6);
            indices[ind++] = new Index(6);
            indices[ind++] = new Index(2);

            return new Geometry<Vertex, Index>(vertices, indices);
        }
    }
}
