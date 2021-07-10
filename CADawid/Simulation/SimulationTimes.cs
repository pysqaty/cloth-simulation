using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Simulation
{
    public struct SimulationTimes
    {
        public float BVHTreeUpdateTime { get; set; }
        public float DynamicTime { get; set; }
        public float CollisionsTime { get; set; }
        public float ClothCollisionsTime { get; set; }
        public float StrainLimitTime { get; set; }
        public float VisualizationUpdateTime { get; set; }
    }
}
