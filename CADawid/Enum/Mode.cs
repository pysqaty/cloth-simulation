using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Enum
{
    public enum Mode
    {
        None,
        Obstacle,
        ObstacleStart,
        Start,
        StartChoice,
        End,
        EndChoice,
        Resize,
        Move
    }

    public enum ResizeMode
    {
        StartX,
        StartY,
        EndX,
        EndY
    }
}
