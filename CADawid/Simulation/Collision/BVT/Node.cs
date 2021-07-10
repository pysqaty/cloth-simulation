using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Simulation.Collision.BVT
{
    public class Node
    {
        public Node Parent { get; set; }
        public Node[] Children { get; set; }

        public AABB AABB { get; set; }
        public AABB FatAABB { get; set; }

        public Node()
        {
            Parent = null;
            Children = new Node[2] { null, null };
        }

        public bool IsLeaf => Children[0] == null;

        public void SetBranch(Node a, Node b)
        {
            a.Parent = this;
            b.Parent = this;

            Children[0] = a;
            Children[1] = b;
        }

        public void SetLeaf(AABB data)
        {
            AABB = data;
            AABB.Node = this;

            Children[0] = null;
            Children[1] = null;
        }

        public void UpdateAABB(float margin)
        {
            if (IsLeaf)
            {
                Vector3 m = new Vector3(margin);
                FatAABB = new AABB()
                {
                    Min = AABB.Min - m,
                    Max = AABB.Max + m
                };
            }
            else
            {
                AABB = AABB.Union(Children[0].AABB, Children[1].AABB);
            }
        }

        public Node GetSibling()
        {
            return this == Parent.Children[0] ? Parent.Children[1] : Parent.Children[0];
        }
    }
}
