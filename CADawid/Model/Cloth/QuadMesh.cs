using CADawid.Simulation;
using CADawid.Simulation.Collision.BVT;
using CADawid.Utils;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Model.Cloth
{
    public struct QuadMesh
    {
        public int p1;
        public int p2;
        public int p3;
        public int p4; //clockwise

        public AABB AABB { get; set; }

        public QuadMesh(int p1, int p2, int p3, int p4)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            this.p4 = p4;
            AABB = new AABB();
            AABB.ContainedQuad = this;
        }

        public Vector3 GetNormal(Point[] vertices)
        {
            var pos1 = vertices[p1].Position;
            var pos2 = vertices[p2].Position;
            var pos3 = vertices[p3].Position;

            return MathExt.TriangleNormal(pos1, pos2, pos3);
        }

        public Vector3 GetNormal(PointState[] vertices)
        {
            var pos1 = vertices[p1].X;
            var pos2 = vertices[p2].X;
            var pos3 = vertices[p3].X;

            return Vector3.Normalize(Vector3.Cross(pos1 - pos2, pos3 - pos2));
        }

        public float GetArea(Point[] vertices)
        {
            var pos1 = vertices[p1].Position;
            var pos2 = vertices[p2].Position;
            var pos3 = vertices[p3].Position;
            var pos4 = vertices[p4].Position;

            var ta1 = MathExt.TriangleArea(pos1, pos2, pos3);
            var ta2 = MathExt.TriangleArea(pos3, pos4, pos1);

            return ta1 + ta2;
        }

        public void UpdateAABB(PointState[] vertices, float dt)
        {
            var pos1 = vertices[p1].X;
            var pos2 = vertices[p2].X;
            var pos3 = vertices[p3].X;
            var pos4 = vertices[p4].X;

            var v1 = vertices[p1].V;
            var v2 = vertices[p2].V;
            var v3 = vertices[p3].V;
            var v4 = vertices[p4].V;

            var fpos1 = pos1 + v1 * dt;
            var fpos2 = pos2 + v2 * dt;
            var fpos3 = pos3 + v3 * dt;
            var fpos4 = pos4 + v4 * dt;

            var max = Vector3.Max(pos1, Vector3.Max(pos2, Vector3.Max(pos3, pos4)));
            var min = Vector3.Min(pos1, Vector3.Min(pos2, Vector3.Min(pos3, pos4)));

            var fmax = Vector3.Max(fpos1, Vector3.Max(fpos2, Vector3.Max(fpos3, fpos4)));
            var fmin = Vector3.Min(fpos1, Vector3.Min(fpos2, Vector3.Min(fpos3, fpos4)));

            AABB.Max = Vector3.Max(max, fmax);
            AABB.Min = Vector3.Max(min, fmin);
        }


    }
}
