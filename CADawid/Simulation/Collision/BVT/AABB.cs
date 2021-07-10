using CADawid.Model.Cloth;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Simulation.Collision.BVT
{
    public class AABB
    {
        public Vector3 Max { get; set; }
        public Vector3 Min { get; set; }

        public Node Node { get; set; }
        public QuadMesh ContainedQuad { get; set; }

        public static AABB Union(AABB a, AABB b)
        {
            AABB c = new AABB();
            c.Min = Vector3.Min(a.Min, b.Min);
            c.Max = Vector3.Max(a.Max, b.Max);
            return c;
        }

        public static bool Intersect(AABB a, AABB b)
        {
            bool intersectX = !(a.Min.X > b.Max.X || b.Min.X > a.Max.X);
            bool intersectY = !(a.Min.Y > b.Max.Y || b.Min.Y > a.Max.Y);
            bool intersectZ = !(a.Min.Z > b.Max.Z || b.Min.Z > a.Max.Z);

            return intersectX && intersectY && intersectZ;
        }

        public bool Contains(AABB a)
        {
            if(a.Max.X > Max.X || a.Max.Y > Max.Y || a.Max.Z > Max.Z ||
                a.Min.X < Min.X || a.Min.Y > Min.Y || a.Min.Z > Min.Z)
            {
                return false;
            }
            return true;
        }

        public float Volume()
        {
            float x = Math.Abs(Max.X - Min.X);
            float y = Math.Abs(Max.Y - Min.Y);
            float z = Math.Abs(Max.Z - Min.Z);

            return x * y * z;
        }



    }
}
