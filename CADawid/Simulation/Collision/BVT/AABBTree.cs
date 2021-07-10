using CADawid.Model.Cloth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Simulation.Collision.BVT
{
    public class AABBTree
    {
        private float margin;
        private Node Root;
        private List<Node> InvalidNodes { get; set; }

        public AABBTree(float margin)
        {
            this.margin = margin;
            Root = null;
            InvalidNodes = new List<Node>();
        }

        public AABBTree(ClothModel cloth, float margin)
        {
            this.margin = margin;
            Root = null;
            InvalidNodes = new List<Node>();

            foreach (var q in cloth.Quads)
            {
                Add(q.AABB);
            }
        }

        public void Add(AABB aabb)
        {
            if(Root != null)
            {
                Node node = new Node();
                node.SetLeaf(aabb);
                node.UpdateAABB(margin);
                InsertNode(node, ref Root);
            }
            else
            {
                Root = new Node();
                Root.SetLeaf(aabb);
                Root.UpdateAABB(margin);
            }
        }

        private void InsertNode(Node node, ref Node parent)
        {
            if(parent.IsLeaf)
            {
                Node newP = new Node();
                newP.Parent = parent.Parent;
                newP.SetBranch(node, parent);
                parent = newP;
            }
            else
            {
                AABB aabb1 = parent.Children[0].AABB;
                AABB aabb2 = parent.Children[1].AABB;

                float volumeDiff1 = AABB.Union(aabb1, node.AABB).Volume() - aabb1.Volume();
                float volumeDiff2 = AABB.Union(aabb2, node.AABB).Volume() - aabb2.Volume();

                if(volumeDiff1 < volumeDiff2)
                {
                    InsertNode(node, ref parent.Children[0]);
                }
                else
                {
                    InsertNode(node, ref parent.Children[1]);
                }
            }

            parent.UpdateAABB(margin);
        }

        public void Remove(AABB aabb)
        {
            Node node = aabb.Node;
            node.AABB = null;
            aabb.Node = null;

            RemoveNode(node);
        }

        private void RemoveNode(Node node)
        {
            Node parent = node.Parent;
            if(parent != null)
            {
                Node sibling = node.GetSibling();
                if(parent.Parent != null)
                {
                    sibling.Parent = parent.Parent;
                    if(parent == parent.Parent.Children[0])
                    {
                        parent.Parent.Children[0] = sibling;
                    }
                    else
                    {
                        parent.Parent.Children[1] = sibling;
                    }
                }
                else
                {
                    Root = sibling;
                    sibling.Parent = null;
                }
            }
            else
            {
                Root = null;
            }
        }

        public void Update()
        {
            if(Root != null)
            {
                if(Root.IsLeaf)
                {
                    Root.UpdateAABB(margin);
                }
                else
                {
                    InvalidNodes.Clear();
                    UpdateNode(Root);

                    foreach(var node in InvalidNodes)
                    {
                        Node parent = node.Parent;
                        Node sibling = node.GetSibling();

                        if(parent.Parent != null)
                        {
                            if(parent == parent.Parent.Children[0])
                            {
                                parent.Parent.Children[0] = sibling;
                            }
                            else
                            {
                                parent.Parent.Children[1] = sibling;
                            }
                        }
                        else
                        {
                            Root = sibling;
                        }

                        sibling.Parent = (parent.Parent != null) ? 
                            parent.Parent :
                            null; //stupid code probably

                        node.UpdateAABB(margin);
                        InsertNode(node, ref Root);
                    }

                    InvalidNodes.Clear(); //double check
                }
            }
        }

        private void UpdateNode(Node node) //AABB added to QuadMEsh in clothes and updated every frame ??
        {
            if(node.IsLeaf)
            {
                if(!node.FatAABB.Contains(node.AABB))
                {
                    InvalidNodes.Add(node);
                }
            }
            else
            {
                UpdateNode(node.Children[0]);
                UpdateNode(node.Children[1]);
            }
        }


        public List<AABB> GetIntersections(AABB aabb)
        {
            List<AABB> intersected = new List<AABB>();
            Stack<Node> s = new Stack<Node>();
            s.Push(Root);
            while (s.Count > 0)
            {
                Node n = s.Pop();
                if(!AABB.Intersect(aabb, n.AABB))
                {
                    continue;
                }
                if(n.IsLeaf)
                {
                    intersected.Add(n.AABB);
                    continue;
                }
                s.Push(n.Children[0]);
                s.Push(n.Children[1]);
            }
            return intersected;
        }


    }
}
