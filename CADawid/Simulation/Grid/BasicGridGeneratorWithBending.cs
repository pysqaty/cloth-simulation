using CADawid.Model.Cloth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Simulation.Grid
{
    public class BasicGridGeneratorWithBending : GridGenerator
    {
        public override SystemState PrepareGrid(ClothModel cloth)
        {
            PointState[] pointStates = new PointState[cloth.Vertices.Length];
            ConnectionList[] connections = new ConnectionList[cloth.Vertices.Length];
            for (int i = 0; i < cloth.Vertices.Length; i++)
            {
                pointStates[i] = new PointState()
                {
                    X = cloth.Vertices[i].Position,
                    V = cloth.Vertices[i].Velocity
                };

                var tis = cloth.IncidentQuads[i];

                HashSet<int> shearNeighs = new HashSet<int>();
                HashSet<int> bendingNeighs = new HashSet<int>();

                foreach (var ti in tis)
                {
                    var t = cloth.Quads[ti];
                    int n1 = -1;
                    int n2 = -1;
                    int n3 = -1;
                    if (t.p1 == i)
                    {
                        n1 = t.p2;
                        n2 = t.p3;
                        n3 = t.p4;
                    }
                    else if (t.p2 == i)
                    {
                        n1 = t.p1;
                        n2 = t.p3;
                        n3 = t.p4;
                    }
                    else if (t.p3 == i)
                    {
                        n1 = t.p1;
                        n2 = t.p2;
                        n3 = t.p4;
                    }
                    else if (t.p4 == i)
                    {
                        n1 = t.p1;
                        n2 = t.p2;
                        n3 = t.p3;
                    }
                    shearNeighs.Add(n1);
                    shearNeighs.Add(n2);
                    shearNeighs.Add(n3);
                }

                //bending
                foreach(var n in shearNeighs)
                {
                    var ntis = cloth.IncidentQuads[n];
                    foreach(var nti in ntis)
                    {
                        var q = cloth.Quads[nti];
                        if(!shearNeighs.Contains(q.p1))
                        {
                            bendingNeighs.Add(q.p1);
                        }
                        if (!shearNeighs.Contains(q.p2))
                        {
                            bendingNeighs.Add(q.p2);
                        }
                        if (!shearNeighs.Contains(q.p3))
                        {
                            bendingNeighs.Add(q.p3);
                        }
                        if (!shearNeighs.Contains(q.p4))
                        {
                            bendingNeighs.Add(q.p4);
                        }
                    }
                }

                List<Connection> s = new List<Connection>();
                foreach (var n in shearNeighs)
                {
                    if (n == i)
                    {
                        continue;
                    }
                    s.Add(new Connection()
                    {
                        from = i,
                        to = n,
                        l0 = (cloth.Vertices[i].BalancePosition - cloth.Vertices[n].BalancePosition).Length(),
                        isBending = false
                    });
                }
                foreach (var n in bendingNeighs)
                {
                    if (n == i)
                    {
                        continue;
                    }
                    s.Add(new Connection()
                    {
                        from = i,
                        to = n,
                        l0 = (cloth.Vertices[i].BalancePosition - cloth.Vertices[n].BalancePosition).Length(),
                        isBending = true
                    });
                }
                connections[i] = new ConnectionList()
                {
                    Connections = s.ToArray(),
                };
            }


            return new SystemState()
            {
                Connections = connections,
                PointStates = pointStates
            };
        }

        public override string GetMetaData()
        {
            StringBuilder meta = new StringBuilder();
            meta.AppendLine("Basic grid generator with bending");
            return meta.ToString();
        }
    }
}
