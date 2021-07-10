using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Simulation.Strain
{
    public class PositionBasedStrainCorrector : StrainCorrector
    {
        public PositionBasedStrainCorrector(float a, int maxIterations) : base(a, maxIterations)
        {
        }
        public override void Correct(SystemState state, float[] pointMasses)
        {
            for(int k = 0; k < MaxIterations; k++)
            {
                bool correct = true;
                for(int i = 0; i < state.PointStates.Length; i++)
                {
                    foreach(Connection connection in state.Connections[i].Connections)
                    {
                        var p1 = state.PointStates[connection.from];
                        var p2 = state.PointStates[connection.to];

                        Vector3 connectionV = p1.X - p2.X;
                        float length = connectionV.Length();

                        float rate = length / connection.l0;

                        if(rate > 1.0f + A)
                        {
                            correct = false;
                            Vector3 p2Dir = Vector3.Normalize(connectionV);
                            Vector3 p1Dir = -p2Dir;
                            float dist = (length - (1.0f + A) * connection.l0) / 2.0f;
                            p1.X += p1Dir * dist;
                            p2.X += p2Dir * dist;

                            state.PointStates[connection.from] = p1;
                            state.PointStates[connection.to] = p2;
                        }
                        else if (rate < 1.0f - A)
                        {
                            correct = false;
                            Vector3 p1Dir = Vector3.Normalize(connectionV);
                            Vector3 p2Dir = -p1Dir;
                            float dist = ((1.0f - A) * connection.l0 - length) / 2.0f;
                            p1.X += p1Dir * dist;
                            p2.X += p2Dir * dist;

                            state.PointStates[connection.from] = p1;
                            state.PointStates[connection.to] = p2;
                        }
                    }
                }
                if(correct)
                {
                    break;
                }
            }
        }

        public override string GetMetaData()
        {
            StringBuilder meta = new StringBuilder();
            meta.AppendLine("Position based strain corrector");
            meta.Append(base.GetMetaData());
            return meta.ToString();
        }
    }
}
