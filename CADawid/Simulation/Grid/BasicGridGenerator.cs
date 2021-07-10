using CADawid.Model.Cloth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Simulation.Grid
{
    public class BasicGridGenerator : GridGenerator
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

                foreach(var ti in tis)
                {
                    var t = cloth.Quads[ti];
                    int n1 = -1;
                    int n2 = -1;
                    int n3 = -1;
                    if(t.p1 == i)
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
                    else if(t.p4 == i)
                    {
                        n1 = t.p1;
                        n2 = t.p2;
                        n3 = t.p3;
                    }
                    shearNeighs.Add(n1);
                    shearNeighs.Add(n2);
                    shearNeighs.Add(n3);
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
            meta.AppendLine("Basic grid generator");
            return meta.ToString();
        }
    }
}
