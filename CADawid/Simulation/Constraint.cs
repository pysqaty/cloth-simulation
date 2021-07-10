using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Simulation
{
    public struct Constraint
    {
        public Constraint(int index, float value)
        {
            Index = index;
            Value = value;
        }

        public int Index { get; set; }
        public float Value { get; set; }
    }
}
