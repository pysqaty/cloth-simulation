using SharpDX;

namespace CADawid.Model
{
    public struct VertexNormal
    {
        public Vector4 Position { get; set; }
        public Vector4 Normal { get; set; }

        public VertexNormal(Vector3 normal, Vector4 position)
        {
            Position = position;
            Normal = new Vector4(normal, 0);
        }

        public VertexNormal(Vector3 normal, Vector3 position, float w = 1)
        {
            Position = new Vector4(position, w);
            Normal = new Vector4(normal, 0);
        }
    }
}
