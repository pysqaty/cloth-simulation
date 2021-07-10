using CADawid.Model.Cloth;
using CADawid.Simulation.Collision.BVT;
using CADawid.Utils;
using SharpDX;
using System;
using System.Collections.Generic;
using MathNet.Numerics;
using System.Linq;
using System.Text;

namespace CADawid.Simulation.Collision
{
    public class BVTClothesCollisionsSolver : ClothesCollisionsSolver
    {
        public float dt { get; set; }

        private float h_self = 0.01f;
        private float h_other = 0.1f;
        private float h = 0f;
        private float h_mtp = 100f;
        private float limitI = 0.01f;
        private float margin = 0.02f;

        public BVTClothesCollisionsSolver(List<SystemState> systemStates, List<ClothModel> cloths, List<float[]> pointMasses, float dt)
        {
            SystemStates = systemStates;
            Cloths = cloths;
            PointMasses = pointMasses;
            Trees = new List<AABBTree>();
            this.dt = dt;

            for(int j = 0; j < Cloths.Count; j++)
            {
                foreach(var q in Cloths[j].Quads)
                {
                    q.UpdateAABB(SystemStates[j].PointStates, dt);
                }
                Trees.Add(new AABBTree(Cloths[j], margin));
            }
        }

        public override void UpdateTrees()
        {
            for (int j = 0; j < Cloths.Count; j++)
            {
                foreach (var q in Cloths[j].Quads)
                {
                    q.UpdateAABB(SystemStates[j].PointStates, dt);
                }

                Trees[j].Update();
            }

        }

        public override void Solve(int j)
        {
            ClothModel cloth = Cloths[j];
            for (int i = 0; i < cloth.Quads.Count; i++)
            {
                QuadMesh quad = cloth.Quads[i];
                for (int k = 0; k < Trees.Count; k++)
                {
                    if (j == k)
                    {
                        h = h_self;
                    }
                    else
                    {
                        h = h_other;
                    }
                    AABBTree tree = Trees[k];
                    var intersected = tree.GetIntersections(quad.AABB);

                    //perform exact tests: point-quad and edge-edge
                    foreach(var iaabb in intersected)
                    {
                        QuadMesh quad2 = iaabb.ContainedQuad;
                        int cloth1 = j; //pi in c1
                        int cloth2 = k; //q1 in c2
                        int p1 = quad.p1, q1 = quad2.p1;
                        int p2 = quad.p2, q2 = quad2.p2;
                        int p3 = quad.p3, q3 = quad2.p3;
                        int p4 = quad.p4, q4 = quad2.p4;

                        PointTriangleTest(q1, q2, q3, p1, cloth2, cloth1);
                        PointTriangleTest(q1, q2, q3, p2, cloth2, cloth1);
                        PointTriangleTest(q1, q2, q3, p3, cloth2, cloth1);
                        PointTriangleTest(q1, q2, q3, p4, cloth2, cloth1);

                        PointTriangleTest(q3, q4, q1, p1, cloth2, cloth1);
                        PointTriangleTest(q3, q4, q1, p2, cloth2, cloth1);
                        PointTriangleTest(q3, q4, q1, p3, cloth2, cloth1);
                        PointTriangleTest(q3, q4, q1, p4, cloth2, cloth1);

                        EdgeEdgeTest(q1, q2, p1, p2, cloth2, cloth1);
                        EdgeEdgeTest(q1, q2, p2, p3, cloth2, cloth1);
                        EdgeEdgeTest(q1, q2, p3, p4, cloth2, cloth1);
                        EdgeEdgeTest(q1, q2, p4, p1, cloth2, cloth1);

                        EdgeEdgeTest(q2, q3, p1, p2, cloth2, cloth1);
                        EdgeEdgeTest(q2, q3, p2, p3, cloth2, cloth1);
                        EdgeEdgeTest(q2, q3, p3, p4, cloth2, cloth1);
                        EdgeEdgeTest(q2, q3, p4, p1, cloth2, cloth1);

                        EdgeEdgeTest(q3, q4, p1, p2, cloth2, cloth1);
                        EdgeEdgeTest(q3, q4, p2, p3, cloth2, cloth1);
                        EdgeEdgeTest(q3, q4, p3, p4, cloth2, cloth1);
                        EdgeEdgeTest(q3, q4, p4, p1, cloth2, cloth1);

                        EdgeEdgeTest(q4, q1, p1, p2, cloth2, cloth1);
                        EdgeEdgeTest(q4, q1, p2, p3, cloth2, cloth1);
                        EdgeEdgeTest(q4, q1, p3, p4, cloth2, cloth1);
                        EdgeEdgeTest(q4, q1, p4, p1, cloth2, cloth1);

                    }
                }
            }
        }

        private void PointTriangleTest(int p1, int p2, int p3, int p4, int c1, int c2) //p123 - clockwise triangle from systemState c1; p4 point from c2
        {
            if(c1 == c2)
            {
                if(p1 == p4 || p2 == p4 || p3 == p4)
                {
                    return;
                }
            }

            var cx1 = SystemStates[c1].PointStates[p1].X;
            var cx2 = SystemStates[c1].PointStates[p2].X;
            var cx3 = SystemStates[c1].PointStates[p3].X;

            

            var cx4 = SystemStates[c2].PointStates[p4].X;

            var v1 = SystemStates[c1].PointStates[p1].V;
            var v2 = SystemStates[c1].PointStates[p2].V;
            var v3 = SystemStates[c1].PointStates[p3].V;

            var v4 = SystemStates[c2].PointStates[p4].V;

            var ts = GetCollisionMoments(cx2 - cx1, cx3 - cx1, cx4 - cx1, v2 - v1, v3 - v1, v4 - v1, dt);

            ts.Add(dt);

            foreach(var t in ts)
            {
                var x1 = cx1 + t * v1;
                var x2 = cx2 + t * v2;
                var x3 = cx3 + t * v3;
                var x4 = cx4 + t * v4;

                var n = MathExt.TriangleNormal(x1, x2, x3);
                var triangleCharactericticLength = (float)Math.Sqrt(MathExt.TriangleArea(x1, x2, x3));

                if (Math.Abs(Vector3.Dot(x4 - x3, n)) >= h) //not close enough to calc projection
                {
                    continue;
                }

                (bool solFound, float w1, float w2) = MathExt.SolveLinearSystemSize2(
                    Vector3.Dot(x1 - x3, x1 - x3), Vector3.Dot(x1 - x3, x2 - x3),
                    Vector3.Dot(x1 - x3, x2 - x3), Vector3.Dot(x2 - x3, x2 - x3),
                    Vector3.Dot(x1 - x3, x4 - x3), Vector3.Dot(x2 - x3, x4 - x3));

                if (!solFound)
                {
                    continue;
                }

                float w3 = 1 - w1 - w2;

                float delta = h / triangleCharactericticLength;

                if (w1 < -delta || w1 > delta + 1 ||
                    w2 < -delta || w2 > delta + 1 ||
                    w3 < -delta || w3 > delta + 1) //projection outside triangle
                {
                    continue;
                }

                // perform correction

                float m1 = PointMasses[c1][p1];
                float m2 = PointMasses[c1][p2];
                float m3 = PointMasses[c1][p3];

                float m4 = PointMasses[c2][p4];

                var projX4 = w1 * x1 + w2 * x2 + w3 * x3;
                var projV4 = w1 * v1 + w2 * v2 + w3 * v3;
                var projM4 = w1 * m1 + w2 * m2 + w3 * m3;

                Vector3 dir = Vector3.Normalize(projX4 - x4);
                
                float vn = Vector3.Dot(projV4 - v4, dir);

                float I = 0.0f;

                if((projX4 - x4).Length() < (h / h_mtp)) //repulsion
                {
                    float repulsionK = 10;
                    float d = h - (projX4 - x4).Length();
                    I = -(float)Math.Min(t * repulsionK * d, projM4 * (0.1 * d / t - vn));

                }
                else
                {
                    I = projM4 * vn / 2.0f;
                    if (I < 0f) //not approaching
                    {
                        continue;
                    }
                }

                I = Math.Abs(I) < limitI ? I : (I < 0 ? -limitI : limitI);
                float impulse = 2 * I / (1 + w1 * w1 + w2 * w2 + w3 * w3);


                SystemStates[c1].PointStates[p1].V += w1 * (impulse / m1) * dir;
                SystemStates[c1].PointStates[p2].V += w2 * (impulse / m2) * dir;
                SystemStates[c1].PointStates[p3].V += w3 * (impulse / m3) * dir;

                SystemStates[c2].PointStates[p4].V -= (impulse / m4) * dir;

                break;
            }

           
        }
        private void EdgeEdgeTest(int p1, int p2, int p3, int p4, int c1, int c2) //p12 - edge from systemState c1; p3p4 edge from c2
        {
            if (c1 == c2)
            {
                if (p1 == p3 || p1 == p4 || 
                    p2 == p3 || p2 == p4)
                {
                    return;
                }
            }

            var cx1 = SystemStates[c1].PointStates[p1].X;
            var cx2 = SystemStates[c1].PointStates[p2].X;

            var cx3 = SystemStates[c2].PointStates[p3].X;
            var cx4 = SystemStates[c2].PointStates[p4].X;

            var v1 = SystemStates[c1].PointStates[p1].V;
            var v2 = SystemStates[c1].PointStates[p2].V;

            var v3 = SystemStates[c2].PointStates[p3].V;
            var v4 = SystemStates[c2].PointStates[p4].V;


            var ts = GetCollisionMoments(cx2 - cx1, cx3 - cx1, cx4 - cx1, v2 - v1, v3 - v1, v4 - v1, dt);

            ts.Add(dt);

            foreach(var t in ts)
            {
                var x1 = cx1 + t * v1;
                var x2 = cx2 + t * v2;
                var x3 = cx3 + t * v3;
                var x4 = cx4 + t * v4;

                if (Vector3.Cross(x2 - x1, x4 - x3).Length() <= 1e-6f)
                {
                    continue;
                }

                (bool solFound, float a, float b) = MathExt.SolveLinearSystemSize2(
                    Vector3.Dot(x2 - x1, x2 - x1), -Vector3.Dot(x2 - x1, x4 - x3),
                    -Vector3.Dot(x2 - x1, x4 - x3), Vector3.Dot(x4 - x3, x4 - x3),
                    Vector3.Dot(x2 - x1, x3 - x1), -Vector3.Dot(x4 - x3, x3 - x1));

                if (!solFound)
                {
                    continue;
                }

                if (a < 0) a = 0;
                if (b < 0) b = 0;
                if (a > 1) a = 1;
                if (b > 1) b = 1;

                var projX12 = x1 + a * (x2 - x1);
                var projX34 = x3 + b * (x4 - x3);

                if ((projX12 - projX34).Length() >= h)
                {
                    continue;
                }

                //perform correction

                var projV12 = (1 - a) * v1 + a * v2;
                var projV34 = (1 - b) * v3 + b * v4;

                float m1 = PointMasses[c1][p1];
                float m2 = PointMasses[c1][p2];

                float m3 = PointMasses[c2][p3];
                float m4 = PointMasses[c2][p4];

                var projM12 = (1 - a) * m1 + a * m2;
                var projM34 = (1 - b) * m3 + b * m4;

                var dir = Vector3.Normalize(projX12 - projX34);
                float vn = Vector3.Dot(projV12 - projV34, dir);

                float I = 0.0f;

                if ((projX12 - projX34).Length() < (h / h_mtp)) //repulsion
                {
                    float repulsionK = 10;
                    float d = h - (projX12 - projX34).Length();
                    I = -(float)Math.Min(t * repulsionK * d, projM12 * (0.1 * d / t - vn));

                }
                else
                {
                    I = projM12 * vn / 2.0f;
                    if (I < 0f) //not approaching
                    {
                        continue;
                    }
                }

                I = Math.Abs(I) < limitI ? I : (I < 0 ? -limitI : limitI);

                float impulse = 2 * I / (a * a + (1 - a) * (1 - a) + b * b + (1 - b) * (1 - b));


                SystemStates[c1].PointStates[p1].V += (1 - a) * (impulse / m1) * dir;
                SystemStates[c1].PointStates[p2].V += a * (impulse / m2) * dir;

                SystemStates[c2].PointStates[p3].V -= (1 - b) * (impulse / m3) * dir;
                SystemStates[c2].PointStates[p4].V -= b * (impulse / m4) * dir;

                break;
            }

            
        }

        public List<float> GetCollisionMoments(Vector3 x21, Vector3 x31, Vector3 x41, Vector3 v21, Vector3 v31, Vector3 v41, float dt)
        {
            float a1 = v41.X * (v21.Y + v31.Z - v21.Z * v31.Y);
            float a2 = v41.Y * (v21.Z + v31.X - v21.X * v31.Z);
            float a3 = v41.Z * (v21.X + v31.Y - v21.Y * v31.X);
            float a = a1 + a2 + a3;

            float b1 = x41.X * (v21.Y * v31.Z - v21.Z * v31.Y) + v41.X * (x21.Y * v31.Z + x31.Z * v21.Y - x21.Z * v31.Y - x31.Y * v21.Z);
            float b2 = x41.Y * (v21.Z * v31.X - v21.X * v31.Z) + v41.Y * (x21.Z * v31.X + x31.X * v21.Z - x21.X * v31.Z - x31.Z * v21.X);
            float b3 = x41.Z * (v21.X * v31.Y - v21.Y * v31.X) + v41.Z * (x21.X * v31.Y + x31.Y * v21.X - x21.Y * v31.X - x31.X * v21.Y);
            float b = b1 + b2 + b3;

            float c1 = v41.X * (x21.Y * x31.Z - x21.Z * x31.Y) + x41.X * (x21.Y * v31.Z + x31.Z * v21.Y - x21.Z * v31.Y - x31.Y * v21.Z);
            float c2 = v41.Y * (x21.Z * x31.X - x21.X * x31.Z) + x41.Y * (x21.Z * v31.X + x31.X * v21.Z - x21.X * v31.Z - x31.Z * v21.X);
            float c3 = v41.Z * (x21.X * x31.Y - x21.Y * x31.X) + x41.Z * (x21.X * v31.Y + x31.Y * v21.X - x21.Y * v31.X - x31.X * v21.Y);
            float c = c1 + c2 + c3;

            float d1 = x41.X * (x21.Y * x31.Z - x21.Z * x31.Y);
            float d2 = x41.Y * (x21.Z * x31.X - x21.X * x31.Z);
            float d3 = x41.Z * (x21.X * x31.Y - x21.Y * x31.X);
            float d = d1 + d2 + d3;

            List<float> ts = new List<float>();

            float eps = 1e-4f;

            if(Math.Abs(a) < eps &&
                Math.Abs(b) < eps &&
                Math.Abs(c) < eps &&
                Math.Abs(d) < eps)
            {
                return ts;
            }

            var res = FindRoots.Cubic(d, c, b, a);

            

            if(res.Item1.IsReal())
            {
                float t = (float)res.Item1.Real;
                if(t >= 0 && t <= dt)
                {
                    ts.Add(t);
                }
            }

            if (res.Item2.IsReal())
            {
                float t = (float)res.Item2.Real;
                if (t >= 0 && t <= dt)
                {
                    ts.Add(t);
                }
            }

            if (res.Item3.IsReal())
            {
                float t = (float)res.Item3.Real;
                if (t >= 0 && t <= dt)
                {
                    ts.Add(t);
                }
            }

            ts =  ts.Distinct().ToList();
            ts.Sort();
            return ts;
        }

        public override string GetMetaData()
        {
            StringBuilder meta = new StringBuilder();
            meta.AppendLine("h_self = " + h_self);
            meta.AppendLine("h_other = " + h_other);
            meta.AppendLine("h_mtp = " + h_mtp);
            meta.AppendLine("limitI = " + limitI);
            meta.AppendLine("margin = " + margin);

            return meta.ToString();
        }
    }
}
