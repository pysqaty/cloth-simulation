using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Model.Collider
{
    public class TorusCollider : Torus, ICollider
    {
        public Vector3 Center => Vector3.Zero;

        public TorusCollider(float R, float r, int prec1, int prec2)
            : base(R, r, prec1, prec2)
        {

        }

        public float Sdf(float px, float py, float pz)
        {
            Vector3 p = new Vector3(px, py, pz);
            p = Vector3.TransformCoordinate(p, Matrix.Invert(CurrentModel));
            Vector3 Pxz = new Vector3(p.X, Center.Y, p.Z);

            Vector3 dirXZ = Vector3.Normalize(Pxz - Center);

            Vector3 C = Center + R * dirXZ;

            float l = (p - C).Length();

            float d = l - r;

            return d;
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
            Vector3 Pxz = new Vector3(p.X, Center.Y, p.Z);

            Vector3 dirXZ = Vector3.Normalize(Pxz - Center);

            Vector3 C = Center + R * dirXZ;

            Vector3 dir = Vector3.Normalize(p - C);

            Vector3 localCP = C + r * dir;

            return Vector3.TransformCoordinate(localCP, CurrentModel); 

        }

        public bool IsPointInside(Vector3 p)
        {
            p = Vector3.TransformCoordinate(p, Matrix.Invert(CurrentModel));

            Vector3 Pxz = new Vector3(p.X, Center.Y, p.Z);

            float lXZ = (Pxz - Center).Length();

            if(lXZ >= (R + r) || lXZ <= (R - r))
            {
                return false;
            }

            Vector3 dirXZ = Vector3.Normalize(Pxz - Center);

            Vector3 C = Center + R * dirXZ;

            float l = (p - C).Length();

            if(l >= r)
            {
                return false; 
            }

            return true;
        }

       
    }
}
