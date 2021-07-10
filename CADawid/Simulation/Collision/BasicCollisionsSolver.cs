using CADawid.Model.Collider;
using CADawid.Simulation;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Simulation.Collision
{
    public class BasicCollisionsSolver : ICollisionsSolver
    {
        public void Solve(SystemState state, ICollider[] colliders)
        {
            for (int i = 0; i < state.PointStates.Length; i++)
            {
                PointState v = state.PointStates[i];
                foreach (var collider in colliders)
                {
                    if (collider.IsPointInside(v.X))
                    {
                        v.X = collider.GetClosestPoint(v.X);
                        v.V = Vector3.Zero;
                        state.PointStates[i] = v;
                    }
                }
            }
        }
    }
}
