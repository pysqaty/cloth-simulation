using CADawid.DxModule;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Utils
{
    public static class MathExt
    {
        public static float Mod(float x, float m)
        {
            float r = x % m;
            return r < 0 ? r + m : r;
        }
        public static int Mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }
        public static float ToRadians(float val)
        {
            return (float)(Math.PI / 180) * val;
        }

        public static Quaternion EulerToQuaternion(Vector3 euler) // roll x, pitch y, yaw z
        {
            float cr = (float)Math.Cos(euler.X / 2.0f);
            float sr = (float)Math.Sin(euler.X / 2.0f);
            float cp = (float)Math.Cos(euler.Y / 2.0f);
            float sp = (float)Math.Sin(euler.Y / 2.0f);
            float cy = (float)Math.Cos(euler.Z / 2.0f);
            float sy = (float)Math.Sin(euler.Z / 2.0f);

            float qw = cr * cp * cy + sr * sp * sy;
            float qx = sr * cp * cy - cr * sp * sy;
            float qy = cr * sp * cy + sr * cp * sy;
            float qz = cr * cp * sy - sr * sp * cy;

            return new Quaternion(qx, qy, qz, qw);
        }
        public static Vector3 QuaternionToEuler(Quaternion q)
        {
            float sinr_cosp = 2 * (q.W * q.X + q.Y * q.Z);
            float cosr_cosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
            float roll = (float)Math.Atan2(sinr_cosp, cosr_cosp);

            float pitch;
            float sinp = 2 * (q.W * q.Y - q.Z * q.X);
            if (Math.Abs(sinp) >= 1)
            {
                pitch = CopySign((float)(Math.PI / 2.0f), sinp); // use 90 degrees if out of range
            }
            else
            {
                pitch = (float)Math.Asin(sinp);
            }

            float siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
            float cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
            float yaw = (float)Math.Atan2(siny_cosp, cosy_cosp);

            return new Vector3(roll, pitch, yaw);


        }

        public static float CopySign(float x, float y)
        {
            int sign = Math.Sign(y);
            sign = sign == 0 ? 1 : sign;
            float mgn = Math.Abs(x);
            return mgn * sign;
        }

        public static Vector3 RotateVector(Vector4 vector, Matrix rotation)
        {
            Matrix result = Matrix.Zero;
            result.Column4 = vector;
            result = rotation * result;
            return new Vector3(result.M14, result.M24, result.M34);
        }

        public static int CirclesIntersection(
            Vector2 c1, float r1,
            Vector2 c2, float r2,
            out Vector2 intersection1, out Vector2 intersection2)
        {
            // Find the distance between the centers.
            float dx = c1.X - c2.X;
            float dy = c1.Y - c2.Y;
            double dist = Math.Sqrt(dx * dx + dy * dy);

            // See how many solutions there are.
            if (dist > r1 + r2)
            {
                // No solutions, the circles are too far apart.
                intersection1 = new Vector2(float.NaN, float.NaN);
                intersection2 = new Vector2(float.NaN, float.NaN);
                return 0;
            }
            else if (dist < Math.Abs(r1 - r2))
            {
                // No solutions, one circle contains the other.
                intersection1 = new Vector2(float.NaN, float.NaN);
                intersection2 = new Vector2(float.NaN, float.NaN);
                return 0;
            }
            else if ((dist == 0) && (r1 == r2))
            {
                // No solutions, the circles coincide.
                intersection1 = new Vector2(float.NaN, float.NaN);
                intersection2 = new Vector2(float.NaN, float.NaN);
                return 0;
            }
            else
            {
                // Find a and h.
                double a = (r1 * r1 -
                    r2 * r2 + dist * dist) / (2 * dist);
                double h = Math.Sqrt(r1 * r1 - a * a);

                // Find P2.
                double cx2 = c1.X + a * (-dx) / dist;
                double cy2 = c1.Y + a * (-dy) / dist;

                // Get the points P3.
                intersection1 = new Vector2(
                    (float)(cx2 + h * (-dy) / dist),
                    (float)(cy2 - h * (-dx) / dist));
                intersection2 = new Vector2(
                    (float)(cx2 - h * (-dy) / dist),
                    (float)(cy2 + h * (-dx) / dist));

                // See if we have 1 or 2 solutions.
                if (dist == r1 + r2)
                {
                    return 1;
                }
                return 2;
            }
        }

        public static float GetAngleBetweenPoints(Vector2 endPt1, Vector2 connectingPt, Vector2 endPt2)
        {
            float x1 = endPt1.X - connectingPt.X; //Vector 1 - x
            float y1 = endPt1.Y - connectingPt.Y; //Vector 1 - y

            float x2 = endPt2.X - connectingPt.X; //Vector 2 - x
            float y2 = endPt2.Y - connectingPt.Y; //Vector 2 - y

            float angle = (float)(Math.Atan2(y1, x1) - Math.Atan2(y2, x2));
            angle = angle * 360 / (2 * (float)Math.PI);

            if (angle < 0)
            {
                angle += 360;
            }

            return angle;
        }

        public static Vector2 ScreenToWorld(System.Windows.Point screen, DxRenderer dxRenderer)
        {
            float rx = (float)(screen.X / dxRenderer.Width);
            float ry = (float)(screen.Y / dxRenderer.Height);
            Vector2 world = new Vector2((rx - 0.5f) * dxRenderer.Scene.Camera.width / dxRenderer.Scene.Camera.ScaleV.X - dxRenderer.Scene.Camera.TranslationV.X, 
                -(ry - 0.5f) * dxRenderer.Scene.Camera.height / dxRenderer.Scene.Camera.ScaleV.Y - dxRenderer.Scene.Camera.TranslationV.Y);
            return world;
        }

        public static void FindIntersection(
            Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4,
            out bool lines_intersect, out bool segments_intersect,
            out Vector2 intersection,
            out Vector2 close_p1, out Vector2 close_p2)
        {
            // Get the segments' parameters.
            float dx12 = p2.X - p1.X;
            float dy12 = p2.Y - p1.Y;
            float dx34 = p4.X - p3.X;
            float dy34 = p4.Y - p3.Y;

            // Solve for t1 and t2
            float denominator = (dy12 * dx34 - dx12 * dy34);

            float t1 =
                ((p1.X - p3.X) * dy34 + (p3.Y - p1.Y) * dx34)
                    / denominator;
            if (float.IsInfinity(t1))
            {
                // The lines are parallel (or close enough to it).
                lines_intersect = false;
                segments_intersect = false;
                intersection = new Vector2(float.NaN, float.NaN);
                close_p1 = new Vector2(float.NaN, float.NaN);
                close_p2 = new Vector2(float.NaN, float.NaN);
                return;
            }
            lines_intersect = true;

            float t2 =
                ((p3.X - p1.X) * dy12 + (p1.Y - p3.Y) * dx12)
                    / -denominator;

            // Find the point of intersection.
            intersection = new Vector2(p1.X + dx12 * t1, p1.Y + dy12 * t1);

            // The segments intersect if t1 and t2 are between 0 and 1.
            segments_intersect =
                ((t1 >= 0) && (t1 <= 1) &&
                 (t2 >= 0) && (t2 <= 1));

            // Find the closest points on the segments.
            if (t1 < 0)
            {
                t1 = 0;
            }
            else if (t1 > 1)
            {
                t1 = 1;
            }

            if (t2 < 0)
            {
                t2 = 0;
            }
            else if (t2 > 1)
            {
                t2 = 1;
            }

            close_p1 = new Vector2(p1.X + dx12 * t1, p1.Y + dy12 * t1);
            close_p2 = new Vector2(p3.X + dx34 * t2, p3.Y + dy34 * t2);
        }

        public static float TriangleArea(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            var AB = p2 - p1;
            var AC = p3 - p1;

            return Vector3.Cross(AB, AC).Length() / 2.0f;
        }

        public static Vector3 TriangleNormal(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            return Vector3.Normalize(Vector3.Cross(p1 - p2, p3 - p2));
        }


        public static (bool foundSol, float x1, float x2) SolveLinearSystemSize2(float a, float b, float c, float d, float y1, float y2)
        {
            bool foundSol = false;
            float x1 = float.NaN;
            float x2 = float.NaN;
            float det = a * d - b * c;
            if (det != 0)
            {
                x1 = (y1 * d - b * y2) / det;
                x2 = (a * y2 - y1 * c) / det;
                foundSol = true;
            }
            return (foundSol, x1, x2);
        }
    }
}
