using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using CADawid.DxModule;
using CADawid.Model.Cloth;
using CADawid.Simulation;
using CADawid.Simulation.Grid;
using CADawid.Simulation.Simulation;
using CADawid.Simulation.Strain;
using CADawid.Utils;
using CADawid.Video;
using SharpDX;

namespace CADawid.Model.Scene
{
    public abstract class Scene : INotifyPropertyChanged
    {
        public List<IGeometryObject> Geometries { get; set; }
        public DxCamera Camera { get; set; }
        protected List<(Vector3 pos, Vector3 rot, Vector3 scl)> CameraTransforms { get; set; }

        public List<ClothModel> Cloths { get; set; }

        public ClothSimulation ClothSimulation { get; set; }

        public SimulationTimes Update()
        {
            //update geometries move
            UpdateObjects();
            //update constraints and pass to simulation
            ClothSimulation.Constraints = UpdateConstraints();
            //update simulation
            return ClothSimulation.Update();
        }

        public abstract void UpdateObjects();
        public abstract List<List<Constraint>> UpdateConstraints();
        public abstract void HandleInput(Vector4 input);
        public virtual void Reset()
        {
            ClothSimulation.Reset();
        }

        public void UpdateCameraTransform(int i)
        {
            (var pos, var rot, var scl) = CameraTransforms[i];
            Camera.TranslationV = pos;
            Camera.RotationV = rot;
            Camera.ScaleV = scl;
        }
        public virtual void HandleKeyUp(Key input)
        {
            switch (input)
            {
                case Key.NumPad1:
                    {
                        if(CameraTransforms.Count > 0)
                        {
                            UpdateCameraTransform(0);
                        }
                        break;
                    }
                case Key.NumPad2:
                    {
                        if (CameraTransforms.Count > 1)
                        {
                            UpdateCameraTransform(1);
                        }
                        break;
                    }
                case Key.NumPad3:
                    {
                        if (CameraTransforms.Count > 2)
                        {
                            UpdateCameraTransform(2);
                        }
                        break;
                    }
                case Key.NumPad4:
                    {
                        if (CameraTransforms.Count > 3)
                        {
                            UpdateCameraTransform(3);
                        }
                        break;
                    }
                case Key.NumPad5:
                    {
                        if (CameraTransforms.Count > 4)
                        {
                            UpdateCameraTransform(4);
                        }
                        break;
                    }
            }
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
