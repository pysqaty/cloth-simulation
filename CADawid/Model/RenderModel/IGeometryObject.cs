using CADawid.DxModule;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Model
{
    public interface IGeometryObject
    {
        void Render(DxRenderer dxRenderer);
        Vector3 Translation { get; set; }
        Vector3 EulerAngles { get; set; }
        Quaternion Rotation { get; set; }
        Vector3 Scale { get; set; }

    }
}
