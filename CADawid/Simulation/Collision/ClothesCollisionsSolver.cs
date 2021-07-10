using CADawid.Model.Cloth;
using CADawid.Simulation.Collision.BVT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Simulation.Collision
{
    public abstract class ClothesCollisionsSolver
    {
        public List<SystemState> SystemStates { get; set; }

        public List<ClothModel> Cloths { get; set; }

        public List<float[]> PointMasses { get; set; }

        public List<AABBTree> Trees { get; set; }

        public abstract void UpdateTrees();

        public abstract void Solve(int j);

        public abstract string GetMetaData();
    }
}
