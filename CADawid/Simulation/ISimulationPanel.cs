using CADawid.Simulation.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Simulation
{
    public interface ISimulationPanel
    {
        void Accept(MassSpringClothSimulation simulation);
        void Accept(InequalityClothSimulation simulation);
    }
}
