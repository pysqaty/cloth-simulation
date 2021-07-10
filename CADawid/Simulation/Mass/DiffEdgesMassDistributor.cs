using CADawid.Model.Cloth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Simulation.Mass
{
    public class DiffEdgesMassDistributor : MassDistributor
    {
        private float a = 1.2f;
        public override float[] DistributeMass(ClothModel cloth, float m)
        {
            float[] masses = new float[cloth.Vertices.Length];

            float totalArea = 0f;
            for(int i = 0; i < cloth.Quads.Count; i++)
            {
                totalArea += cloth.Quads[i].GetArea(cloth.Vertices);
            }

            float totalM = totalArea * m;

            int edgeNodesCount = 0;

            for (int i = 0; i < masses.Length; i++)
            {
                var quadsCount = cloth.IncidentQuads[i].Count;
                if(quadsCount <= 2)
                {
                    edgeNodesCount++;
                }
            }

            float n1 = edgeNodesCount;
            float n2 = masses.Length - n1;
            float mass = totalM / (n1 * a + n2);
            float edgeMass = a * mass;

            for (int i = 0; i < masses.Length; i++)
            {
                var quadsCount = cloth.IncidentQuads[i].Count;
                if (quadsCount <= 2)
                {
                    masses[i] = edgeMass;
                }
                else
                {
                    masses[i] = mass;
                }
            }

            return masses;
        }

        public override string GetMetaData()
        {
            StringBuilder meta = new StringBuilder();
            meta.AppendLine("Different edges mass distributor, a = " + a);
            return meta.ToString();
        }
    }
}
