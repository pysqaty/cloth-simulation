using CADawid.Utils;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Model.Cloth
{
    public class CylindricalCloth : ClothModel
    {
        public float R1;
        public float R2;
        public float Height;

        public int DivH;
        public int DivR;

        public int Indices2Index(int i, int j)
        {
            if (i < 0 || i > DivH - 1 || j < 0 || j > DivR - 1)
            {
                return -1;
            }
            return i * DivR + j;
        }

        public (int, int) Index2Indices(int k)
        {
            if (k < 0 || k > Vertices.Length - 1)
            {
                return (-1, -1);
            }
            return ((int)(k / DivR), k % DivR);
        }

        public CylindricalCloth(float r1, float r2, float h, int divh, int divr, Vector4 color, Matrix transform)
        {
            R1 = r1;
            R2 = r2;
            Height = h;
            DivH = divh;
            DivR = divr;
            Color = color;
            Model = Matrix.Identity;

            Vertices = new Point[DivH * DivR];

            int k = 0;

            float hStep = Height / (DivH - 1);
            float rStep = MathExt.ToRadians(360) / (DivR - 1); //maybe better DivR?
            
            for(int i = 0; i < DivH; i++)
            {
                float r = r1 + ((float)i / (DivH - 1)) * (r2 - r1);
                float y = Height / 2 - i * hStep;
                for (int j = 0; j < DivR; j++)
                {
                    float t = j * rStep;

                    float x = (float)(r * Math.Sin(t));
                    float z = (float)(r * Math.Cos(t));

                    Vertices[k++] = new Point(Vector3.TransformCoordinate(new Vector3(x, y, z), transform));
                }
            }

            Quads = new List<QuadMesh>();
            IncidentQuads = new Dictionary<int, List<int>>();

            for (int i = 0; i < DivH - 1; i++)
            {
                for (int j = 0; j < DivR - 1; j++)
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

        public CylindricalCloth(float r, float h, int divh, int divr, Vector4 color, Matrix transform) : this(r, r, h, divh, divr, color, transform)
        { }

        public CylindricalCloth(float r, float h, int divh, int divr, Vector4 color) : this(r, r, h, divh, divr, color, Matrix.Identity)
        { }

        public CylindricalCloth(float r1, float r2, float h, int divh, int divr, Vector4 color) : this(r1, r2, h, divh, divr, color, Matrix.Identity)
        { }
    }
}
