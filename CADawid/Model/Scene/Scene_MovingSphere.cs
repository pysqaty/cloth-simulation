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
    public class Scene_MovingSphere : Scene
    {
        public Cube Frame { get; set; }

        public ICollider SphereCollider { get; set; }

        private float amplitude = 1f;
        private float speed = 1.5f;
        private float sphereX = 0.5f;
        private float sphereY = -1.0f;

        public Scene_MovingSphere()
        {
            Geometries = new List<IGeometryObject>();
            Camera = new DxCamera();
            CameraTransforms = new List<(Vector3 pos, Vector3 rot, Vector3 scl)>();
            CameraTransforms.Add((Vector3.Zero, new Vector3(-0.14f, 1.9f, 0.0f), new Vector3(4.4f)));
            CameraTransforms.Add((Vector3.Zero, new Vector3(0.08f, 0.08f, 0.0f), new Vector3(3.84f)));
            CameraTransforms.Add((Vector3.Zero, new Vector3(-1.68f, -1.56f, 0.0f), new Vector3(3.84f)));
            CameraTransforms.Add((Vector3.Zero, new Vector3(0.08f, -3.16f, 0.0f), new Vector3(3.68f)));
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

            SphereCollider = new SphereCollider(0.5f, 10);
            (SphereCollider as IGeometryObject).Translation = new Vector3(sphereX, sphereY, 0.0f);
            Geometries.Add(SphereCollider as IGeometryObject);

            Cloths = new List<ClothModel>();
            var cloth = new PlaneCloth(1, 21, new Vector4(255f, 43f, 251f, 255f) / 255f, Matrix.RotationQuaternion(MathExt.EulerToQuaternion(new Vector3(MathExt.ToRadians(90), 0, 0))));
            Geometries.Add(cloth);
            Cloths.Add(cloth);

            ClothSimulation = new MassSpringClothSimulationEuler(Cloths, new ICollider[] { SphereCollider  }, new float[] { -1 }, new float[] { -1 });
            //ClothSimulation = new InequalityClothSimulation(Cloths, new ICollider[] { SphereCollider });
        }

        private void MoveFrame(Vector3 translation)
        {
            Frame.Translate(translation);
        }

        private void RotateFrame(Vector3 axis, float val)
        {
            Frame.Rotate(axis, val);
        }

        public override void HandleInput(Vector4 input)
        {
            if (Keyboard.IsKeyDown(Key.R))
            {
                Vector3 rotation = MathExt.RotateVector(new Vector4(0, 0, 1, 0), Camera.R);
                RotateFrame(rotation, (float)(input.X));
            }
            else
            {
                Vector3 translationVector = MathExt.RotateVector(input, Camera.R);
                MoveFrame(translationVector);
            }
        }

        public override List<List<Constraint>> UpdateConstraints()
        {
            List<List<Constraint>> allConstraints = new List<List<Constraint>>();

            for (int j = 0; j < 1; j++)
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
            return allConstraints;
        }

        public override void UpdateObjects()
        {
            (SphereCollider as IGeometryObject).Translation = new Vector3(sphereX, sphereY, amplitude * (float)Math.Cos(speed * ClothSimulation.T));
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
