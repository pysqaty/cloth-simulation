using CADawid.Model.Cloth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Simulation.Mass
{
    public class AreaBasedMassDistributor : MassDistributor
    {
        public override float[] DistributeMass(ClothModel cloth, float m)
        {
            float[] masses = new float[cloth.Vertices.Length];

            for (int i = 0; i < masses.Length; i++)
            {
                var quads = cloth.IncidentQuads[i];
                float mass = 0f;
                foreach (var qi in quads)
                {
                    var q = cloth.Quads[qi];
                    float area = q.GetArea(cloth.Vertices);
                    mass += area * m / 4.0f;
                }

                masses[i] = mass;
            }

            return masses;
        }

        public override string GetMetaData()
        {
            StringBuilder meta = new StringBuilder();
            meta.AppendLine("Area based mass distributor");
            return meta.ToString();
        }
    }
}
