using CADawid.Model.Cloth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Simulation.Mass
{
    public class EqualMassDistributor : MassDistributor
    {
        public override float[] DistributeMass(ClothModel cloth, float m)
        {
            float[] masses = new float[cloth.Vertices.Length];

            float totalArea = 0f;
            for (int i = 0; i < cloth.Quads.Count; i++)
            {
                totalArea += cloth.Quads[i].GetArea(cloth.Vertices);
            }

            float totalM = totalArea * m;

            for (int i = 0; i < masses.Length; i++)
            {
                masses[i] = totalM / masses.Length;
            }

            return masses;
        }

        public override string GetMetaData()
        {
            StringBuilder meta = new StringBuilder();
            meta.AppendLine("equal mass distributor");
            return meta.ToString();
        }
    }
}
