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
    public class Scene_FreeMaterialAndTorus : Scene
    {
        public ICollider TorusCollider { get; set; }

        private float torusY = -0.3f;

        public Scene_FreeMaterialAndTorus()
        {
            Geometries = new List<IGeometryObject>();
            Camera = new DxCamera();
            CameraTransforms = new List<(Vector3 pos, Vector3 rot, Vector3 scl)>();
            CameraTransforms.Add((new Vector3(-0.62f, 0.09f, -0.9f), new Vector3(-0.97f, 2.73f, 0.0f), new Vector3(6.52f)));
            CameraTransforms.Add((new Vector3(-0.62f, 0.09f, -0.9f), new Vector3(0.05f, 3.24f, 0.0f), new Vector3(7.92f)));
            CameraTransforms.Add((new Vector3(-0.52f, 0.05f, -0.45f), new Vector3(1.67f, 3.47f, 0.0f), new Vector3(7.92f)));
            CameraTransforms.Add((new Vector3(-0.88f, 1.77f, -1.54f), new Vector3(-1.02f, 2.94f, 0.0f), new Vector3(7.92f)));
            UpdateCameraTransform(0);

            float size = 200;
            Geometries.Add(new Segment(new Vertex(-size / 50, 0, 0, 1), new Vertex(size, 0, 0, 1),
                Matrix.Identity, new Vector4(0.5f, 0, 0, 1)));

            Geometries.Add(new Segment(new Vertex(0, -size / 50, 0, 1), new Vertex(0, size, 0, 1),
                Matrix.Identity, new Vector4(0, 0.5f, 0, 1)));

            Geometries.Add(new Segment(new Vertex(0, 0, -size / 50, 1), new Vertex(0, 0, size, 1),
                Matrix.Identity, new Vector4(0, 0, 0.5f, 1)));

            TorusCollider = new TorusCollider(0.4f, 0.2f, 30, 30);
            (TorusCollider as IGeometryObject).Translation = new Vector3(0.5f, torusY, 0.5f);
            (TorusCollider as IGeometryObject).EulerAngles = new Vector3(MathExt.ToRadians(0), MathExt.ToRadians(0), MathExt.ToRadians(0));
            Geometries.Add(TorusCollider as IGeometryObject);

            Cloths = new List<ClothModel>();
            var cloth = new PlaneCloth(1, 21, new Vector4(255f, 43f, 251f, 255f) / 255f);
            Geometries.Add(cloth);
            Cloths.Add(cloth);

            ClothSimulation = new MassSpringClothSimulationEuler(Cloths, new ICollider[] { TorusCollider }, new float[] { -1 }, new float[] { -1 });
            //ClothSimulation = new InequalityClothSimulation(Cloths, new ICollider[] { TorusCollider });
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
                PlaneCloth planeCloth = Cloths[j] as PlaneCloth;
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
