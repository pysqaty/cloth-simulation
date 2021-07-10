using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Simulation.Strain
{
    public abstract class StrainCorrector
    {
        public float A { get; set; }

        public int MaxIterations { get; set; }

        public StrainCorrector(float a, int maxIterations)
        {
            A = a;
            MaxIterations = maxIterations;
        }

        public abstract void Correct(SystemState state, float[] pointMasses);

        public virtual string GetMetaData()
        {
            StringBuilder meta = new StringBuilder();
            meta.AppendLine("A = " + A);
            meta.AppendLine("MaxIterations = " + MaxIterations);
            return meta.ToString();
        }
    }
}
