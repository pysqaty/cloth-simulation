using CADawid.Model;
using CADawid.Model.Cloth;
using SharpDX;
using System;
using MathWorks.MATLAB.NET.Utility;
using MathWorks.MATLAB.NET.Arrays;
using Optimization;
using System.Collections.Generic;
using CADawid.Model.Collider;
using CADawid.Simulation.Collision;
using System.Text;

namespace CADawid.Simulation.Simulation
{
    public class InequalityClothSimulation : ClothSimulation
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
        public float Alpha
        {
            get => alpha;
            set
            {
                lock (mutex)
                {
                    alpha = value;
                    NotifyPropertyChanged(nameof(Alpha));
                }
            }
        }

        #endregion
        protected Optimizer optimizer;
        protected float k;
        protected float g;
        protected float alpha;

        public InequalityClothSimulation(List<ClothModel> cloths, ICollider[] colliders) : base(cloths)
        {
            Colliders = colliders;
            k = 1;
            m = 10;
            g = -10f;
            alpha = 0.05f;
            State = SimulationState.Running;

            try
            {
                optimizer = new Optimizer();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            for (int j = 0; j < Cloths.Count; j++)
            {
                InitState(j);
                try
                {
                    optimizer.initConstraints(0, new MWObjectArray(SystemStates[j]), new MWNumericArray(alpha), 
                        new MWNumericArray(j + 1), new MWObjectArray(Colliders));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            ClothesCollisionsSolver = new BVTClothesCollisionsSolver(SystemStates, Cloths, PointMasses, dt);

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

        private float initBarrierParam = 1;
        private float initTrustRegionRadiusParam = 1e-30f;
        private float stepToleranceParam = 1e-3f;
        private int maxFunctionEvaluationsParam = 300;
        private float constraintToleranceParam = 1e-3f;
        private float optimalityToleranceParam = 1e-6f;
        private int maxIterationsParam = 1000;

        public override void ResolveDynamic(int j)
        {
            try
            {
                var retVal = optimizer.ic_barrier(2, new MWNumericArray(PointMasses[j]), new MWObjectArray(SystemStates[j]), new MWObjectArray(Constraints[j]),
                    new MWNumericArray(k), new MWNumericArray(g), new MWNumericArray(dt), new MWNumericArray(initBarrierParam), 
                    new MWNumericArray(initTrustRegionRadiusParam), new MWNumericArray(stepToleranceParam), new MWNumericArray(maxFunctionEvaluationsParam),
                    new MWNumericArray(constraintToleranceParam), new MWNumericArray(optimalityToleranceParam), new MWNumericArray(maxIterationsParam),
                    new MWNumericArray(j + 1));
                var x = (double[,])retVal[0].ToArray();
                var cforces = (double[,])retVal[1].ToArray();
                int jj = 0;
                for(int i = 0; i < x.GetLength(0); i+=3)
                {
                    Vector3 F = new Vector3();
                    F += -k * Cloths[j].Vertices[jj].Velocity;
                    F += new Vector3(0, g, 0) * PointMasses[j][jj];
                    var constraintForce = new Vector3((float)cforces[i, 0], (float)cforces[i + 1, 0], (float)cforces[i + 2, 0]);
                    F += constraintForce;
                    var newPos = new Vector3((float)x[i,0], (float)x[i + 1,0], (float)x[i + 2,0]);
                    SystemStates[j].PointStates[jj].V += dt * F / PointMasses[j][jj];
                    SystemStates[j].PointStates[jj].X = newPos;
                    jj++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public override void ResolveStrain(int j)
        {
            //base.ResolveStrain(j);
        }

        public override void ResolveConstraints(int j)
        {
            //Special case, when it is better to handle in matlab resolve dynamic code
            //base.ResolveConstraints(j);
        }

        public override void Visit(ISimulationPanel panel)
        {
            panel.Accept(this);
        }

        public override void ResolveCollisions(int j)
        {
            //Special case, when it is better to handle in matlab resolve dynamic code
        }

        public override string GetMetaData()
        {
            StringBuilder meta = new StringBuilder();
            meta.AppendLine("Inequality cloth simulation");
            meta.AppendLine("dt = " + dt);
            meta.AppendLine("k = " + k);
            meta.AppendLine("m = " + m);
            meta.AppendLine("g = " + g);
            meta.AppendLine("alpha = " + alpha);
            meta.AppendLine("initBarrierParam = " + initBarrierParam);
            meta.AppendLine("initTrustRegionRadiusParam = " + initTrustRegionRadiusParam);
            meta.AppendLine("stepToleranceParam = " + stepToleranceParam);
            meta.AppendLine("maxFunctionEvaluationsParam = " + maxFunctionEvaluationsParam);
            meta.AppendLine("constraintToleranceParam = " + constraintToleranceParam);
            meta.AppendLine("optimalityToleranceParam = " + optimalityToleranceParam);
            meta.AppendLine("maxIterationsParam = " + maxIterationsParam);

            meta.AppendLine("\nClothes:");
            for(int i = 0; i < Cloths.Count; i++)
            {
                meta.AppendLine("ClothID = " + (i + 1) + ", " + Cloths[i].Vertices.Length + " nodes");
            }

            meta.Append(base.GetMetaData());

            return meta.ToString();
        }
    }
}
