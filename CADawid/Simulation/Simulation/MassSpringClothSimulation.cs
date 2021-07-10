using CADawid.Model;
using CADawid.Model.Cloth;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Simulation.Simulation
{
    public abstract class MassSpringClothSimulation : ClothSimulation
    {
        #region UI

        public float M
        {
            get => m;
            set
            {
                lock (mutex)
                {
                    m = value;
                    NotifyPropertyChanged(nameof(M));
                }

            }
        }
        public float K
        {
            get => k;
            set
            {
                lock (mutex)
                {
                    k = value;
                    NotifyPropertyChanged(nameof(K));
                }

            }
        }

        public float ShearC
        {
            get => shearC;
            set
            {
                lock (mutex)
                {
                    shearC = value;
                    NotifyPropertyChanged(nameof(ShearC));
                }
            }
        }

        public float BendingC
        {
            get => bendingC;
            set
            {
                lock (mutex)
                {
                    bendingC = value;
                    NotifyPropertyChanged(nameof(BendingC));
                }
            }
        }

        public float G
        {
            get => g;
            set
            {
                lock (mutex)
                {
                    g = value;
                    NotifyPropertyChanged(nameof(G));
                }
            }
        }

        #endregion

        protected float k;
        protected float g;
        protected float[] shearCs;
        protected float shearC;
        protected float[] bendingCs;
        protected float bendingC;

        public MassSpringClothSimulation(List<ClothModel> cloths) : base(cloths)
        {

        }

        protected float f(float t, float l, float v, float c)
        {
            return (-c * l);
        }

        protected SystemState Equation(float t, SystemState state, int j)
        {
            PointState[] pointStates = new PointState[state.PointStates.Length];
            for (int i = 0; i < SystemStates[j].PointStates.Length; i++)
            {
                Connection[] springs = SystemStates[j].Connections[i].Connections;
                Vector3 F = new Vector3();
                foreach (var s in springs)
                {
                    Vector3 dir = SharpDX.Vector3.Normalize(state.PointStates[s.to].X - state.PointStates[s.from].X);
                    float l = -(state.PointStates[s.to].X - state.PointStates[s.from].X).Length() + s.l0;
                    float v = SharpDX.Vector3.Dot(state.PointStates[s.from].V, dir);

                    F += (f(t, l, v, s.isBending ? (bendingCs[j] < 0 ? bendingC : bendingCs[j]) : (shearCs[j] < 0 ? shearC : shearCs[j])) * dir);
                }
                F += -k * state.PointStates[i].V;
                F += new Vector3(0, g, 0) * PointMasses[j][i];

                pointStates[i] = new PointState()
                {
                    X = state.PointStates[i].V,
                    V = F / PointMasses[j][i],
                };
            }
            return new SystemState()
            {
                PointStates = pointStates,
                Connections = state.Connections
            };
        }

        public override void Reset()
        {
            lock (mutex)
            {
                lock (resetMutex)
                {
                    for(int j = 0; j < Cloths.Count; j++)
                    {
                        T = 0;
                        foreach (var p in Cloths[j].Vertices)
                        {
                            p.Position = p.BalancePosition;
                            p.Velocity = Vector3.Zero;
                        }

                        Cloths[j].ResetGeometry();
                        InitState(j);
                    }
                    
                }
            }

        }

        public override void Visit(ISimulationPanel panel)
        {
            panel.Accept(this);
        }

        public override string GetMetaData()
        {
            StringBuilder meta = new StringBuilder();
            meta.AppendLine("dt = " + dt);
            meta.AppendLine("k = " + k);
            meta.AppendLine("m = " + m);
            meta.AppendLine("g = " + g);
            meta.AppendLine("shearC = " + shearC);
            meta.AppendLine("bendingC = " + bendingC);

            meta.AppendLine("\nClothes:");
            for (int i = 0; i < Cloths.Count; i++)
            {
                meta.AppendLine("ClothID = " + (i + 1) + ", " + Cloths[i].Vertices.Length + " nodes, shearC = " + shearCs[i] + ", bendingC = " + bendingCs[i]);
            }

            meta.Append(base.GetMetaData());

            return meta.ToString();
        }
    }
}
