using SharpDX;
using System;

namespace CADawid.Model
{
    public class Segment : GeometryObject<Vertex, Index>
    {
        public Vertex from { get; set; }
        public Vertex to { get; set; }
        public Segment(Vertex from, Vertex to, Matrix model, Vector4 color) : base(model, color)
        {
            this.from = from;
            this.to = to;
        }

        protected override Geometry<Vertex, Index> GenerateGeometry()
        {
            Vertex[] vertexArray = new Vertex[] { from, to };
            Index[] edgeArray = new Index[] { new Index(0), new Index(1) };
            return new Geometry<Vertex, Index>(vertexArray, edgeArray);
        }
    }
}
