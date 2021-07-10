using CADawid.Model;
using CADawid.Model.Cloth;
using CADawid.Model.Collider;
using CADawid.Simulation.Collision;
using CADawid.Simulation.Grid;
using CADawid.Simulation.Mass;
using CADawid.Simulation.Strain;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Simulation.Simulation
{
    public abstract class ClothSimulation : INotifyPropertyChanged
    {
        private float t = 0.0f;

        public float T
        {
            get => t;
            set
            {
                t = value;
                NotifyPropertyChanged(nameof(T));
            }
        }

        public float dt = 0.016f;
        public readonly object mutex = new object();
        public readonly object resetMutex = new object();
        protected float m;

        public SimulationState State { get; set; }

        public List<List<Constraint>> Constraints { get; set; }

        public List<SystemState> SystemStates { get; set; }

        public List<ClothModel> Cloths { get; set; }

        public StrainCorrector StrainCorrector { get; set; }

        public ICollisionsSolver CollisionsSolver { get; set; }

        public ClothesCollisionsSolver ClothesCollisionsSolver { get; set; }

        public GridGenerator GridGenerator { get; set; }
        public MassDistributor MassDistributor { get; set; }

        public ICollider[] Colliders { get; set; }

        public ClothSimulation(List<ClothModel> cloths)
        {
            Cloths = cloths;
            Constraints = new List<List<Constraint>>(new List<Constraint>[Cloths.Count]);
            SystemStates = new List<SystemState>(new SystemState[Cloths.Count]);
            PointMasses = new List<float[]>();
            for(int j = 0; j < Cloths.Count; j++)
            {
                PointMasses.Add(null);
            }

            GridGenerator = new BasicGridGenerator();
            MassDistributor = new EqualMassDistributor();
            StrainCorrector = new PositionBasedStrainCorrector(0.1f, 3);
            CollisionsSolver = new BasicCollisionsSolver();
        }

        public SimulationTimes Update()
        {
            if (State == SimulationState.Running)
            {
                Stopwatch bVHTreeUpdateSW = new Stopwatch();
                Stopwatch dynamicSW = new Stopwatch();
                Stopwatch collisionsSW = new Stopwatch();
                Stopwatch clothCollisionsSW = new Stopwatch();
                Stopwatch strainLimitSW = new Stopwatch();
                Stopwatch visualizationUpdateSW = new Stopwatch();

                lock (resetMutex)
                {
                    bVHTreeUpdateSW.Start();
                    ClothesCollisionsSolver?.UpdateTrees();
                    bVHTreeUpdateSW.Stop();
                    for (int j = 0; j < Cloths.Count; j++)
                    {
                        dynamicSW.Start();
                        ResolveDynamic(j);
                        dynamicSW.Stop();

                        strainLimitSW.Start();
                        ResolveStrain(j);
                        strainLimitSW.Stop();

                        collisionsSW.Start();
                        ResolveCollisions(j);
                        collisionsSW.Stop();

                        clothCollisionsSW.Start();
                        ResolveClothCollisions(j);
                        clothCollisionsSW.Stop();

                        ResolveConstraints(j);

                        visualizationUpdateSW.Start();
                        for (int i = 0; i < Cloths[j].Vertices.Length; i++)
                        {
                            Cloths[j].Vertices[i].Position = SystemStates[j].PointStates[i].X;
                            Cloths[j].Vertices[i].Velocity = SystemStates[j].PointStates[i].V;
                        }
                        visualizationUpdateSW.Stop();
                    }


                    T += dt;
                }

                lock (mutex)
                {
                    for (int j = 0; j < Cloths.Count; j++)
                    {
                        Cloths[j].ResetGeometry();
                    }
                }

                return new SimulationTimes()
                {
                    BVHTreeUpdateTime = bVHTreeUpdateSW.ElapsedMilliseconds,
                    DynamicTime = dynamicSW.ElapsedMilliseconds,
                    CollisionsTime = collisionsSW.ElapsedMilliseconds,
                    ClothCollisionsTime = clothCollisionsSW.ElapsedMilliseconds,
                    StrainLimitTime = strainLimitSW.ElapsedMilliseconds,
                    VisualizationUpdateTime = visualizationUpdateSW.ElapsedMilliseconds
                };

            }

            return default(SimulationTimes);
        }
        public abstract void Reset();

        public abstract void ResolveDynamic(int j);
        public virtual void ResolveCollisions(int j)
        {
            CollisionsSolver?.Solve(SystemStates[j], Colliders);
        }

        public virtual void ResolveClothCollisions(int j)
        {
            ClothesCollisionsSolver?.Solve(j);
        }

        public virtual void ResolveStrain(int j)
        {
            StrainCorrector?.Correct(SystemStates[j], PointMasses[j]);
        }
        public virtual void ResolveConstraints(int j)
        {

            foreach(var c in Constraints[j])
            {
                int ind = c.Index / 3;
                if (c.Index % 3 == 0) // X
                {
                    SystemStates[j].PointStates[ind].X.X = c.Value;
                }
                else if(c.Index % 3 == 1) // Y
                {
                    SystemStates[j].PointStates[ind].X.Y = c.Value;
                }
                else // Z
                {
                    SystemStates[j].PointStates[ind].X.Z = c.Value;
                }
                SystemStates[j].PointStates[ind].V = Vector3.Zero;
            }
        }

        public List<float[]> PointMasses { get; set; }
        protected void InitMasses(int j)
        {
            PointMasses[j] = MassDistributor.DistributeMass(Cloths[j], m);
        }

        protected virtual void InitState(int j)
        {
            SystemStates[j] = GridGenerator.PrepareGrid(Cloths[j]);

            InitMasses(j);
        }

        public abstract void Visit(ISimulationPanel panel);

        public virtual string GetMetaData()
        {
            StringBuilder meta = new StringBuilder();
            meta.AppendLine();
            meta.Append(GridGenerator?.GetMetaData());
            meta.AppendLine();
            meta.Append(MassDistributor?.GetMetaData());
            meta.AppendLine();
            meta.Append(StrainCorrector?.GetMetaData());
            meta.AppendLine();
            meta.Append(ClothesCollisionsSolver?.GetMetaData());

            return meta.ToString();
        }

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
