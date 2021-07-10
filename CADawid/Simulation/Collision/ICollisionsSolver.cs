using CADawid.Model.Collider;
using CADawid.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Simulation.Collision
{
    public interface ICollisionsSolver
    {
        void Solve(SystemState state, ICollider[] colliders);
    }
}
