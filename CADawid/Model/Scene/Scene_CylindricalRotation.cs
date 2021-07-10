using CADawid.DxModule;
using CADawid.Model.Cloth;
using CADawid.Model.Collider;
using CADawid.Simulation;
using CADawid.Simulation.Simulation;
using CADawid.Utils;
using SharpDX;
using System;
using System.Collections.Generic;
namespace CADawid.Model.Scene
{
    public class Scene_CylindricalRotation : Scene
    {
        private Vector3[] plate1;
        private Vector3[] plate2;
        private float currentRotation = 0;
        private float rotationSpeed = 1f;
        private float direction = 1;
        private float angleLimit = (float)Math.PI / 2.0f;
        public Scene_CylindricalRotation()
        {
            Geometries = new List<IGeometryObject>();
            Camera = new DxCamera();
            CameraTransforms = new List<(Vector3 pos, Vector3 rot, Vector3 scl)>();
            CameraTransforms.Add((new Vector3(-1.12f, 0.26f, -0.57f), new Vector3(-0.6f, 2.14f, 0.0f), new Vector3(3.8f)));
            CameraTransforms.Add((new Vector3(-0.65f, 0.23f, -0.49f), new Vector3(-0.12f, 3.27f, 0.0f), new Vector3(3.8f)));
            CameraTransforms.Add((new Vector3(-0.65f, 0.23f, -0.49f), new Vector3(-0.62f, 4.25f, 0.0f), new Vector3(3.0f)));
            CameraTransforms.Add((new Vector3(-0.65f, 0.23f, -0.49f), new Vector3(-0.06f, 4.79f, 0.0f), new Vector3(3.0f)));
            UpdateCameraTransform(0);

            float size = 200;
            Geometries.Add(new Segment(new Vertex(-size / 50, 0, 0, 1), new Vertex(size, 0, 0, 1),
                Matrix.Identity, new Vector4(0.5f, 0, 0, 1)));

            Geometries.Add(new Segment(new Vertex(0, -size / 50, 0, 1), new Vertex(0, size, 0, 1),
                Matrix.Identity, new Vector4(0, 0.5f, 0, 1)));

            Geometries.Add(new Segment(new Vertex(0, 0, -size / 50, 1), new Vertex(0, 0, size, 1),
                Matrix.Identity, new Vector4(0, 0, 0.5f, 1)));

            Cloths = new List<ClothModel>();
            var cloth = new CylindricalCloth(1, 3, 15, 25, new Vector4(255f, 43f, 251f, 255f) / 255f, 
                Matrix.RotationQuaternion(MathExt.EulerToQuaternion(new Vector3(MathExt.ToRadians(90), 0, 0))));
            Geometries.Add(cloth);
            Cloths.Add(cloth);

            plate1 = new Vector3[cloth.DivR];
            plate2 = new Vector3[cloth.DivR];

            for(int j = 0; j < cloth.DivR; j++)
            {
                plate1[j] = cloth.Vertices[cloth.Indices2Index(0, j)].BalancePosition;
                plate2[j] = cloth.Vertices[cloth.Indices2Index(cloth.DivH - 1, j)].BalancePosition;
            }

            ClothSimulation = new MassSpringClothSimulationEuler(Cloths, new ICollider[] { }, new float[] { -1 }, new float[] { -1 });
            //ClothSimulation = new InequalityClothSimulation(Cloths, new ICollider[] { });
        }


        public override void HandleInput(Vector4 input)
        {
            currentRotation += input.Y * rotationSpeed;

            var cloth = Cloths[0] as CylindricalCloth;

            for (int j = 0; j < cloth.DivR; j++)
            {
                int v1 = cloth.Indices2Index(0, j);
                int v2 = cloth.Indices2Index(cloth.DivH - 1, j);


                plate1[j] = Vector3.TransformCoordinate(cloth.Vertices[v1].BalancePosition, Matrix.RotationQuaternion(MathExt.EulerToQuaternion(new Vector3(0, 0, currentRotation))));
                plate2[j] = Vector3.TransformCoordinate(cloth.Vertices[v2].BalancePosition, Matrix.RotationQuaternion(MathExt.EulerToQuaternion(new Vector3(0, 0, -currentRotation))));
            }
        }

        public override List<List<Constraint>> UpdateConstraints()
        {
            List<List<Constraint>> allConstraints = new List<List<Constraint>>();

            var c = new List<Constraint>();

            var cloth = Cloths[0] as CylindricalCloth;

            for (int j = 0; j < cloth.DivR; j++)
            {
                int v1 = cloth.Indices2Index(0, j);
                int v2 = cloth.Indices2Index(cloth.DivH - 1, j);

                c.Add(new Constraint(v1 * 3, plate1[j].X));
                c.Add(new Constraint(v1 * 3 + 1, plate1[j].Y));
                c.Add(new Constraint(v1 * 3 + 2, plate1[j].Z));

                c.Add(new Constraint(v2 * 3, plate2[j].X));
                c.Add(new Constraint(v2 * 3 + 1, plate2[j].Y));
                c.Add(new Constraint(v2 * 3 + 2, plate2[j].Z));
            }

            allConstraints.Add(c);

            return allConstraints;
        }

        public override void UpdateObjects()
        {
            if(Math.Abs(currentRotation) > angleLimit)
            {
                direction *= -1;
            }
            currentRotation += direction * ClothSimulation.dt * rotationSpeed;

            var cloth = Cloths[0] as CylindricalCloth;

            for (int j = 0; j < cloth.DivR; j++)
            {
                int v1 = cloth.Indices2Index(0, j);
                int v2 = cloth.Indices2Index(cloth.DivH - 1, j);


                plate1[j] = Vector3.TransformCoordinate(cloth.Vertices[v1].BalancePosition, Matrix.RotationQuaternion(MathExt.EulerToQuaternion(new Vector3(0, 0, currentRotation))));
                plate2[j] = Vector3.TransformCoordinate(cloth.Vertices[v2].BalancePosition, Matrix.RotationQuaternion(MathExt.EulerToQuaternion(new Vector3(0, 0, -currentRotation))));
            }
        }

        public override void Reset()
        {
            base.Reset();

            var cloth = Cloths[0] as CylindricalCloth;
            currentRotation = 0;
            for (int j = 0; j < cloth.DivR; j++)
            {
                plate1[j] = cloth.Vertices[cloth.Indices2Index(0, j)].BalancePosition;
                plate2[j] = cloth.Vertices[cloth.Indices2Index(cloth.DivH - 1, j)].BalancePosition;
            }
        }
    }
}

