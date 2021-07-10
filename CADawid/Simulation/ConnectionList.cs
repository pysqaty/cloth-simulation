using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Simulation
{
    public struct ConnectionList
    {
        public Connection[] Connections { get; set; }

        public Connection this[int i]
        {
            get
            {
                return Connections[i];
            }
        }
    }
}
