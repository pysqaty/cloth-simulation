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
    public class MassSpringClothSimulationEuler : MassSpringClothSimulation
    {
        public MassSpringClothSimulationEuler(List<ClothModel> cloths, ICollider[] colliders, float[] shearCs, float[] bendingCs) : base(cloths)
        {
            Colliders = colliders;
            shearC = 10f;
            bendingC = 10f;
            k = 1f;
            m = 10;
            g = -10f;
            State = SimulationState.Running;
            this.bendingCs = bendingCs;
            this.shearCs = shearCs;

            for (int j = 0; j < Cloths.Count; j++)
            {
                InitState(j);
            }

            ClothesCollisionsSolver = new BVTClothesCollisionsSolver(SystemStates, Cloths, PointMasses, dt);
        }

        public override void ResolveDynamic(int j)
        {
            SystemState dy = Equation(T, SystemStates[j], j);
            SystemStates[j] = SystemStates[j] + dt * dy;
        }

        public override string GetMetaData()
        {
            StringBuilder meta = new StringBuilder();
            meta.AppendLine("Spring based simulation (Euler)");
            meta.Append(base.GetMetaData());
            return meta.ToString();
        }
    }   
}
