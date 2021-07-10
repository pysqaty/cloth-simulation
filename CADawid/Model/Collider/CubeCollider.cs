using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Model.Collider
{
    public class CubeCollider : Cube, ICollider
    {
        public Vector3 Center => Vector3.Zero;

        public CubeCollider(float side) : base(side, side, side)
        {
        }

        public CubeCollider(float sideX, float sideY, float sideZ) : base(sideX, sideY, sideZ)
        { 
        }

        public float Sdf(float px, float py, float pz)
        {
            Vector3 p = new Vector3(px, py, pz);
            p = Vector3.TransformCoordinate(p, Matrix.Invert(CurrentModel));
            px = p.X;
            py = p.Y;
            pz = p.Z;
            var c = this.Center;
            float ox = Math.Abs(c.X - px) - SideX / 2;
            float oy = Math.Abs(c.Y - py) - SideY / 2;
            float oz = Math.Abs(c.Z - pz) - SideZ / 2;

            float udx = Math.Max(ox, 0);
            float udy = Math.Max(oy, 0);
            float udz = Math.Max(oz, 0);
            float unsignedDst = (new Vector3(udx, udy, udz)).Length();

            float dibx = Math.Min(ox, 0);
            float diby = Math.Min(oy, 0);
            float dibz = Math.Min(oz, 0);
            float dstInsideBox = Math.Max(dibx, Math.Max(diby, dibz));

            return unsignedDst + dstInsideBox;
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

        public bool IsPointInside(Vector3 p)
        {
            p = Vector3.TransformCoordinate(p, Matrix.Invert(CurrentModel));
            bool insideX = p.X > Center.X - SideX / 2 && p.X < Center.X + SideX / 2;
            bool insideY = p.Y > Center.Y - SideY / 2 && p.Y < Center.Y + SideY / 2;
            bool insideZ = p.Z > Center.Z - SideZ / 2 && p.Z < Center.Z + SideZ / 2;
            return insideX && insideY && insideZ;
        }

        public Vector3 GetClosestPoint(Vector3 p)
        {
            bool isInside = IsPointInside(p);
            p = Vector3.TransformCoordinate(p, Matrix.Invert(CurrentModel));
            Vector3 closestPoint = Vector3.Zero;
            if(!isInside)
            {
                closestPoint.X = Math.Max(Center.X - SideX / 2, Math.Min(p.X, Center.X + SideX / 2));
                closestPoint.Y = Math.Max(Center.Y - SideY / 2, Math.Min(p.Y, Center.Y + SideY / 2));
                closestPoint.Z = Math.Max(Center.Z - SideZ / 2, Math.Min(p.Z, Center.Z + SideZ / 2));
            }
            else
            {
                Vector3 bounds = new Vector3(SideX, SideY, SideZ) / 2;
                Vector3 vectorToPositiveBounds = Center + bounds - p;
                Vector3 vectorToNegativeBounds = -(Center - bounds - p);
                float smallestX = Math.Min(vectorToPositiveBounds.X, vectorToNegativeBounds.X);
                float smallestY = Math.Min(vectorToPositiveBounds.Y, vectorToNegativeBounds.Y);
                float smallestZ = Math.Min(vectorToPositiveBounds.Z, vectorToNegativeBounds.Z);
                float smallestDistance = Math.Min(smallestX, Math.Min(smallestY, smallestZ));

                if (smallestDistance == vectorToPositiveBounds.X)
                    closestPoint = new Vector3(Center.X + bounds.X, p.Y, p.Z);
                else if (smallestDistance == vectorToNegativeBounds.X)
                    closestPoint = new Vector3(Center.X - bounds.X, p.Y, p.Z);
                else if (smallestDistance == vectorToPositiveBounds.Y)
                    closestPoint = new Vector3(p.X , Center.Y + bounds.Y, p.Z);
                else if (smallestDistance == vectorToNegativeBounds.Y)
                    closestPoint = new Vector3(p.X, Center.Y - bounds.Y, p.Z);
                else if (smallestDistance == vectorToPositiveBounds.Z)
                    closestPoint = new Vector3(p.X, p.Y, Center.Z + bounds.Z);
                else if (smallestDistance == vectorToNegativeBounds.Z)
                    closestPoint = new Vector3(p.X, p.Y, Center.Z - bounds.Z);
            }
            return Vector3.TransformCoordinate(closestPoint, CurrentModel);
        }
    }
}
