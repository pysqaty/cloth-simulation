using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Simulation.Strain
{
    public class VelocityBasedStrainCorrector : StrainCorrector
    {
        private float dt;
        public VelocityBasedStrainCorrector(float a, int maxIterations, float dt) : base(a, maxIterations)
        {
            this.dt = dt;
        }

        public override void Correct(SystemState state, float[] pointMasses)
        {
            for (int k = 0; k < MaxIterations; k++)
            {
                HashSet<(int, int)> checkedConn = new HashSet<(int, int)>();
                bool correct = true;
                for (int i = 0; i < state.PointStates.Length; i++)
                {
                    foreach (Connection connection in state.Connections[i].Connections)
                    {
                        if(checkedConn.Contains((connection.from, connection.to)))
                        {
                            continue;
                        }
                        checkedConn.Add((connection.from, connection.to));
                        checkedConn.Add((connection.to, connection.from));
                        var p1 = state.PointStates[connection.from];
                        var p2 = state.PointStates[connection.to];

                        var m1 = pointMasses[connection.from];
                        var m2 = pointMasses[connection.to];

                        Vector3 connectionV = p1.X - p2.X;
                        float length = connectionV.Length();

                        float rate = length / connection.l0;

                        if (rate > 1.0f + A)
                        {
                            correct = false;
                            Vector3 p2Dir = Vector3.Normalize(connectionV);
                            Vector3 p1Dir = -p2Dir;
                            float dist = (length - (1.0f + A) * connection.l0) / 2.0f;
                            var currV1 = Vector3.Dot(p1.V, p1Dir);

                            var v1prim = (dist / dt - currV1) * p1Dir + p1.V;
                            var v2prim = (m1 * p1.V + m2 * p2.V - m1 * v1prim) / m2;
                            p1.V = v1prim;
                            p2.V = v2prim;

                            state.PointStates[connection.from] = p1;
                            state.PointStates[connection.to] = p2;
                        }
                        else if (rate < 1.0f - A)
                        {
                            correct = false;
                            Vector3 p1Dir = Vector3.Normalize(connectionV);
                            Vector3 p2Dir = -p1Dir;
                            float dist = ((1.0f - A) * connection.l0 - length) / 2.0f;
                            var currV1 = Vector3.Dot(p1.V, p1Dir);

                            var v1prim = (dist / dt - currV1) * p1Dir + p1.V;
                            var v2prim = (m1 * p1.V + m2 * p2.V - m1 * v1prim) / m2;
                            p1.V = v1prim;
                            p2.V = v2prim;

                            state.PointStates[connection.from] = p1;
                            state.PointStates[connection.to] = p2;
                        }
                    }
                }
                if (correct)
                {
                    break;
                }
            }
        }

        public override string GetMetaData()
        {
            StringBuilder meta = new StringBuilder();
            meta.AppendLine("Velocity based strain corrector");
            meta.Append(base.GetMetaData());
            return meta.ToString();
        }
    }
}
