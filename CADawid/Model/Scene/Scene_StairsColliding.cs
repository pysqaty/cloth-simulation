using CADawid.DxModule;
using CADawid.Model.Cloth;
using CADawid.Model.Collider;
using CADawid.Simulation;
using CADawid.Simulation.Simulation;
using CADawid.Utils;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace CADawid.Model.Scene
{
    public class Scene_StairsColliding : Scene
    { 
        public ICollider[] BoxColliders { get; set; }

        private int stepsCount = 9;
        private float firstStepHeight = 2.0f;
        private float stepWidth = 0.3f;
        private float stepHeightDifference = 0.15f;


        Vector3 colliderPos = new Vector3(0.5f, -1.5f, 0.0f);
        
        public Scene_StairsColliding()
        {
            Geometries = new List<IGeometryObject>();
            Camera = new DxCamera();
            CameraTransforms = new List<(Vector3 pos, Vector3 rot, Vector3 scl)>();
            CameraTransforms.Add((new Vector3(-0.28f, 1.71f, -0.86f), new Vector3(-0.1f, 1.63f, 0.0f), new Vector3(3.56f)));
            CameraTransforms.Add((new Vector3(-0.5f, 2.18f, -1.03f), new Vector3(-0.3f, 3.13f, 0.0f), new Vector3(3.56f)));
            CameraTransforms.Add((new Vector3(-0.3f, 2.03f, -1.10f), new Vector3(-0.68f, 2.2f, 0.0f), new Vector3(3.56f)));
            UpdateCameraTransform(0);

            float size = 200;
            Geometries.Add(new Segment(new Vertex(-size / 50, 0, 0, 1), new Vertex(size, 0, 0, 1),
                Matrix.Identity, new Vector4(0.5f, 0, 0, 1)));

            Geometries.Add(new Segment(new Vertex(0, -size / 50, 0, 1), new Vertex(0, size, 0, 1),
                Matrix.Identity, new Vector4(0, 0.5f, 0, 1)));

            Geometries.Add(new Segment(new Vertex(0, 0, -size / 50, 1), new Vertex(0, 0, size, 1),
                Matrix.Identity, new Vector4(0, 0, 0.5f, 1)));

            BoxColliders = new ICollider[stepsCount];

            for(int i = 0; i < stepsCount; i++)
            {
                float currHDiff = i * stepHeightDifference;
                BoxColliders[i] = new CubeCollider(2, firstStepHeight - currHDiff, stepWidth);
                (BoxColliders[i] as IGeometryObject).Translation = colliderPos + new Vector3(0.0f, -currHDiff / 2, i * stepWidth);
                Geometries.Add(BoxColliders[i] as IGeometryObject);
            }

            Cloths = new List<ClothModel>();
            var cloth = new PlaneCloth(1, 3, 6, 51, new Vector4(255f, 43f, 251f, 255f) / 255f);
            Geometries.Add(cloth);
            Cloths.Add(cloth);

            ClothSimulation = new MassSpringClothSimulationEuler(Cloths, BoxColliders, new float[] { -1 }, new float[] { -1 });
            //ClothSimulation = new InequalityClothSimulation(Cloths, BoxColliders);
        }

        public override void HandleInput(Vector4 input)
        {
        }

        public override List<List<Constraint>> UpdateConstraints()
        {
            List<List<Constraint>> allConstraints = new List<List<Constraint>>();

            for (int j = 0; j < 1; j++)
            {
                List<Constraint> constraints = new List<Constraint>();
                allConstraints.Add(constraints);
            }
            return allConstraints;
        }

        public override void UpdateObjects()
        {

        }

        public override void Reset()
        {
            base.Reset();
        }
    }
}

