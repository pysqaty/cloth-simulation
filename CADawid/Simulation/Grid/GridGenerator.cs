using CADawid.Model.Cloth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Simulation.Grid
{
    public abstract class GridGenerator
    {
        public abstract SystemState PrepareGrid(ClothModel cloth);

        public abstract string GetMetaData();
    }
}
