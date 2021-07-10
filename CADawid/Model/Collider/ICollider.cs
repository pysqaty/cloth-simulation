using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Model.Collider
{
    public interface ICollider
    {
        bool IsPointInside(Vector3 p);
        Vector3 GetClosestPoint(Vector3 p);

        float Sdf(float px, float py, float pz);
        float dxSdf(float px, float py, float pz);
        float dySdf(float px, float py, float pz);
        float dzSdf(float px, float py, float pz);
    }
}
