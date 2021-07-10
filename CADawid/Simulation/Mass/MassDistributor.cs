using CADawid.Model.Cloth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Simulation.Mass
{
    public abstract class MassDistributor
    {
        public abstract float[] DistributeMass(ClothModel cloth, float m);
        public abstract string GetMetaData();
    }
}
