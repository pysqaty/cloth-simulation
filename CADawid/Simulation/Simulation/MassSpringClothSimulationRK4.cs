using CADawid.Model;
using CADawid.Model.Cloth;
using CADawid.Model.Collider;
using CADawid.Simulation.Collision;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Point = CADawid.Model.Point;

namespace CADawid.Simulation.Simulation
{
    public class MassSpringClothSimulationRK4 : MassSpringClothSimulation
    {
        public MassSpringClothSimulationRK4(List<ClothModel> cloths, ICollider[] colliders, float[] shearCs, float[] bendingCs) : base(cloths)
        {
            Colliders = colliders;
            shearC = 10f;
            bendingC = 10f;
            k = 1f;
            m = 15f;
            g = -10f;
            this.bendingCs = bendingCs;
            this.shearCs = shearCs;
            State = SimulationState.Running;

            for (int j = 0; j < Cloths.Count; j++)
            {
                InitState(j);
            }

            ClothesCollisionsSolver = new BVTClothesCollisionsSolver(SystemStates, Cloths, PointMasses, dt);

        }

        public override void ResolveDynamic(int j)
        {
            SystemState dy1 = Equation(T, SystemStates[j], j);
            SystemState dy2 = Equation(T + dt / 2, SystemStates[j] + dt / 2 * dy1, j);
            SystemState dy3 = Equation(T + dt / 2, SystemStates[j] + dt / 2 * dy2, j);
            SystemState dy4 = Equation(T + dt, SystemStates[j] + dt * dy3, j);

            SystemStates[j] = SystemStates[j] + (dt / 6) * (dy1 + 2 * dy2 + 2 * dy3 + dy4);
        }

        public override string GetMetaData()
        {
            StringBuilder meta = new StringBuilder();
            meta.AppendLine("Spring based simulation (RK4)");
            meta.Append(base.GetMetaData());
            return meta.ToString();
        }
    }
}
