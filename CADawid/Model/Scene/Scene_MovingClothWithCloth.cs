using CADawid.DxModule;
using CADawid.Model.Cloth;
using CADawid.Model.Collider;
using CADawid.Simulation;
using CADawid.Simulation.Simulation;
using CADawid.Utils;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CADawid.Model.Scene
{
    public class Scene_MovingClothWithCloth : Scene
    {
        public Cube Frame { get; set; }


        private Vector3 collidingClothOffset = new Vector3(-0.5f, -0.4f, 0.3f);

        private Vector3 frameSpeed = new Vector3(0, 0, -0.0007f);
        private float frameMoveTime = 10f;
        private float elasticOffset = 0.05f;

        public Scene_MovingClothWithCloth()
        {
            Geometries = new List<IGeometryObject>();
            Camera = new DxCamera();
            CameraTransforms = new List<(Vector3 pos, Vector3 rot, Vector3 scl)>();
            CameraTransforms.Add((Vector3.Zero, new Vector3(-0.14f, 1.63f, 0.0f), new Vector3(3.52f)));
            CameraTransforms.Add((Vector3.Zero, new Vector3(0.44f, 1.68f, 0.0f), new Vector3(3.52f)));
            CameraTransforms.Add((Vector3.Zero, new Vector3(0.1f, 3.1f, 0.0f), new Vector3(3.52f)));
            CameraTransforms.Add((new Vector3(-0.28f, 0.64f, 0.04f), new Vector3(-0.09f, 0.05f, 0.0f), new Vector3(3.52f)));
            UpdateCameraTransform(0);

            float size = 200;
            Geometries.Add(new Segment(new Vertex(-size / 50, 0, 0, 1), new Vertex(size, 0, 0, 1),
                Matrix.Identity, new Vector4(0.5f, 0, 0, 1)));

            Geometries.Add(new Segment(new Vertex(0, -size / 50, 0, 1), new Vertex(0, size, 0, 1),
                Matrix.Identity, new Vector4(0, 0.5f, 0, 1)));

            Geometries.Add(new Segment(new Vertex(0, 0, -size / 50, 1), new Vertex(0, 0, size, 1),
                Matrix.Identity, new Vector4(0, 0, 0.5f, 1)));

            Geometries.Add(Frame = new Cube(1));
            Frame.Translation = new Vector3(Frame.SideX, Frame.SideY, Frame.SideZ) / 2;

            Cloths = new List<ClothModel>();
            var cloth = new PlaneCloth(1, 21, new Vector4(255f, 43f, 251f, 255f) / 255f);
            var cloth1 = new PlaneCloth(2, 11, new Vector4(43f, 100f, 251f, 255f) / 255f, Matrix.Translation(collidingClothOffset));
            Geometries.Add(cloth);
            Geometries.Add(cloth1);

            Cloths.Add(cloth);
            Cloths.Add(cloth1);

            ClothSimulation = new MassSpringClothSimulationEuler(Cloths, new ICollider[] { }, new float[] { -1, 100 }, new float[] { -1, 100 });
            //ClothSimulation = new InequalityClothSimulation(Cloths, new ICollider[] { });
        }

        public override void HandleInput(Vector4 input)
        {
        }

        public override List<List<Constraint>> UpdateConstraints()
        {
            List<List<Constraint>> allConstraints = new List<List<Constraint>>();

            for(int j = 0; j < 1; j++)
            {
                List<Constraint> constraints = new List<Constraint>();
                PlaneCloth planeCloth = Cloths[j] as PlaneCloth;
                int ind1 = planeCloth.Indices2Index(0, 0) * 3;
                int ind2 = planeCloth.Indices2Index(planeCloth.DivW - 1, 0) * 3;
                Vector3 pos1 = Vector3.TransformCoordinate(Frame.Vertices[0, 0, 0].Position, Frame.CurrentModel);
                Vector3 pos2 = Vector3.TransformCoordinate(Frame.Vertices[1, 0, 0].Position, Frame.CurrentModel);
                constraints.Add(new Constraint(ind1, pos1.X));
                constraints.Add(new Constraint(ind1 + 1, pos1.Y));
                constraints.Add(new Constraint(ind1 + 2, pos1.Z));
                constraints.Add(new Constraint(ind2, pos2.X));
                constraints.Add(new Constraint(ind2 + 1, pos2.Y));
                constraints.Add(new Constraint(ind2 + 2, pos2.Z));
                allConstraints.Add(constraints);
            }

            List<Constraint> stableConstraints = new List<Constraint>();
            PlaneCloth colliderCloth = Cloths[1] as PlaneCloth;
            int i1 = colliderCloth.Indices2Index(0, 0) * 3;
            int i2 = colliderCloth.Indices2Index(colliderCloth.DivW - 1, 0) * 3;
            int i3 = colliderCloth.Indices2Index(0, colliderCloth.DivL - 1) * 3;
            int i4 = colliderCloth.Indices2Index(colliderCloth.DivW - 1, colliderCloth.DivL - 1) * 3;

            
            stableConstraints.Add(new Constraint(i1, collidingClothOffset.X - elasticOffset));
            stableConstraints.Add(new Constraint(i1 + 1, collidingClothOffset.Y));
            stableConstraints.Add(new Constraint(i1 + 2, collidingClothOffset.Z - elasticOffset));

            stableConstraints.Add(new Constraint(i2, collidingClothOffset.X + colliderCloth.Width + elasticOffset));
            stableConstraints.Add(new Constraint(i2 + 1, collidingClothOffset.Y));
            stableConstraints.Add(new Constraint(i2 + 2, collidingClothOffset.Z - elasticOffset));

            stableConstraints.Add(new Constraint(i3, collidingClothOffset.X - elasticOffset));
            stableConstraints.Add(new Constraint(i3 + 1, collidingClothOffset.Y));
            stableConstraints.Add(new Constraint(i3 + 2, collidingClothOffset.Z + colliderCloth.Length + elasticOffset));

            stableConstraints.Add(new Constraint(i4, collidingClothOffset.X + colliderCloth.Width + elasticOffset));
            stableConstraints.Add(new Constraint(i4 + 1, collidingClothOffset.Y));
            stableConstraints.Add(new Constraint(i4 + 2, collidingClothOffset.Z + colliderCloth.Length + elasticOffset));

            allConstraints.Add(stableConstraints);


            return allConstraints;
        }

        public override void UpdateObjects()
        {
            if (ClothSimulation.T <= frameMoveTime)
            {
                Frame.Translation += frameSpeed * ClothSimulation.T;
            }
        }

        public override void Reset()
        {
            foreach (var point in Frame.Vertices)
            {
                point.Velocity = new Vector3(0, 0, 0);
                point.Position = point.BalancePosition;
            }

            Frame.Translation = new Vector3(Frame.SideX, Frame.SideY, Frame.SideZ) / 2;
            Frame.Rotation = SharpDX.Quaternion.Identity;

            Frame.ResetGeometry();
            base.Reset();
        }
    }
}
