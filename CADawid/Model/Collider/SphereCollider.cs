using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Model.Collider
{
    public class SphereCollider : Sphere, ICollider
    {
        public Vector3 Center => Vector3.Zero;


        public SphereCollider(float r, int div) : base(r, div)
        {

        }

        public float Sdf(float px, float py, float pz)
        {
            Vector3 p = new Vector3(px, py, pz);
            p = Vector3.TransformCoordinate(p, Matrix.Invert(CurrentModel));
            float L = (p - Center).Length();
            return L - R;
        }

        private float h = 0.00001f;
        public float dxSdf(float px, float py, float pz)
        {
            return (Sdf(px + h, py, pz) - Sdf(px, py, pz)) / h;
        }

        public float dySdf(float px, float py, float pz)
        {
            return (Sdf(px, py + h, pz) - Sdf(px, py, pz)) / h;
        }

        public float dzSdf(float px, float py, float pz)
        {
            return (Sdf(px, py, pz + h) - Sdf(px, py, pz)) / h;
        }

        public Vector3 GetClosestPoint(Vector3 p)
        {
            p = Vector3.TransformCoordinate(p, Matrix.Invert(CurrentModel));
            Vector3 dir = Vector3.Normalize(p - Center);
            Vector3 localCP = dir * R;
            return Vector3.TransformCoordinate(localCP, CurrentModel);
        }

        public bool IsPointInside(Vector3 p)
        {
            p = Vector3.TransformCoordinate(p, Matrix.Invert(CurrentModel));
            if((p - Center).Length() < R)
            {
                return true;
            }
            return false;
        }
    }
}
