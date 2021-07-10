using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Model
{
    public struct Triangle
    {
        public UInt16 Index1 { get; set; }
        public UInt16 Index2 { get; set; }
        public UInt16 Index3 { get; set; }

        public Triangle(UInt16 index1, UInt16 index2, UInt16 index3)
        {
            Index1 = index1;
            Index2 = index2;
            Index3 = index3;
        }
    }
}
