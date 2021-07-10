using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Model
{
    public class Point
    {
        public Vector3 Position { get; set; }
        public Vector3 BalancePosition { get; set; }

        public Vector3 Velocity { get; set; }

        public Point(Vector3 position)
        {
            BalancePosition = position;
            Position = position;
            Velocity = new Vector3();
        }
    }
}
