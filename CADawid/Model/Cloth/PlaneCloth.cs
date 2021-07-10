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
    public class PlaneCloth : ClothModel
    {
        public float Length { get; set; }
        public float Width { get; set; }

        public int DivL { get; set; }
        public int DivW { get; set; }


        public int Indices2Index(int i, int j)
        {
            if (i < 0 || i > DivW - 1 || j < 0 || j > DivL - 1)
            {
                return -1;
            }
            return i * DivL + j;
        }

        public (int, int) Index2Indices(int k)
        {
            if(k < 0 || k > Vertices.Length - 1)
            {
                return (-1, -1);
            }
            return ((int)(k / DivL), k % DivL);
        }

        public PlaneCloth(float side, int division, Vector4 color) : this(side, division, color, Matrix.Identity)
        {

        }

        public PlaneCloth(float width, float length, int divw, int divl, Vector4 color) : this(width, length, divw, divl, color, Matrix.Identity)
        {

        }

        public PlaneCloth(float width, float length, int divw, int divl, Vector4 color, Matrix transform)
        {
            Length = length;
            Width = width;
            DivL = divl;
            DivW = divw;
            Color = color;
            Model = Matrix.Identity;
            float calcPos(int ind, int div, float l) => ((float)ind / (div - 1)) * l;
            Vertices = new Point[DivW * DivL];

            int k = 0;
            for (int i = 0; i < DivW; i++)
            {
                for (int j = 0; j < DivL; j++)
                {
                    float x = calcPos(i, DivW, Width);
                    float z = calcPos(j, DivL, Length);
                    Vertices[k++] = new Point(Vector3.TransformCoordinate(new Vector3(x, 0, z), transform));
                }
            }

            Quads = new List<QuadMesh>();
            IncidentQuads = new Dictionary<int, List<int>>();


            for (int i = 0; i < DivW - 1; i++)
            {
                for (int j = 0; j < DivL - 1; j++)
                {
                    int p1 = Indices2Index(i, j);
                    int p2 = Indices2Index(i, j + 1);
                    int p3 = Indices2Index(i + 1, j + 1);
                    int p4 = Indices2Index(i + 1, j);

                    Quads.Add(new QuadMesh(p1, p2, p3, p4));
                }
            }

            for (int i = 0; i < Quads.Count; i++)
            {
                var t = Quads[i];
                AddIncidentTriangle(t.p1, i);
                AddIncidentTriangle(t.p2, i);
                AddIncidentTriangle(t.p3, i);
                AddIncidentTriangle(t.p4, i);
            }
        }


        public PlaneCloth(float side, int division, Vector4 color, Matrix transform) : this(side, side, division, division, color, transform)
        {
        }      
    }
}
