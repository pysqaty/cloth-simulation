using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Simulation
{
    public struct SystemState
    {
        public ConnectionList[] Connections;
        public PointState[] PointStates;

        public static SystemState operator *(float val, SystemState qw)
        {
            for (int i = 0; i < qw.PointStates.Length; i++)
            {
                qw.PointStates[i].X *= val;
                qw.PointStates[i].V *= val;
            }
            return qw;
        }

        public static SystemState operator +(SystemState var1, SystemState var2)
        {
            for (int i = 0; i < var1.PointStates.Length; i++)
            {
                var1.PointStates[i].X += var2.PointStates[i].X;
                var1.PointStates[i].V += var2.PointStates[i].V;
            }
            return var1;
        }

    }
}
