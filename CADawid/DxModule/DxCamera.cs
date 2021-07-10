using System;
using System.ComponentModel;
using SharpDX;

namespace CADawid.DxModule
{
    public class DxCamera : INotifyPropertyChanged
    {
        public const float ScaleVar = 0.04f;
        public const float RotateVar = 0.02f;
        public const float TranslationVar = 0.02f;

        public Matrix ViewMatrix;
        public Matrix ProjectionMatrix;
        public Matrix VP;

        public Vector3 Eye;
        public Vector3 At;
        public Vector3 Up;

        public float width;
        public float height;

        public Matrix InvView => Matrix.Invert(ViewMatrix);

        public Vector3 TranslationV { get; set; }
        public Vector3 ScaleV { get; set; }
        public Vector3 RotationV { get; set; }

        public Matrix Ry => Matrix.RotationY(RotationV.Y);
        public Matrix Rx => Matrix.RotationX(RotationV.X);
        public Matrix S => Matrix.Scaling(ScaleV);
        public Matrix T => Matrix.Translation(TranslationV);
        public Matrix R => Ry * Rx;
        public DxCamera()
        {
            Reset();
        }
        public void Reset()
        {
            ViewMatrix = Matrix.Identity;
            ProjectionMatrix = Matrix.Identity;
            Eye = new Vector3(0.0f, 0.0f, -20.0f);
            At = new Vector3(0.0f, 0.0f, 0.0f);
            Up = new Vector3(0.0f, 1.0f, 0.0f);
            ScaleV = new Vector3(1f, 1f, 1f);
            RotationV = new Vector3(0f, 0f, 0f);
            TranslationV = new Vector3(0f, 0f, 0f);
            Update();
        }
        public void Update()
        {
            Matrix.LookAtLH(ref Eye, ref At, ref Up, out ViewMatrix);
            ViewMatrix = T * R * S * ViewMatrix;
            VP = CalculateVPMatrix();

            UpdateUIValues();
        }

        private void UpdateUIValues()
        {

            NotifyPropertyChanged(nameof(PosX));
            NotifyPropertyChanged(nameof(PosY));
            NotifyPropertyChanged(nameof(PosZ));

            NotifyPropertyChanged(nameof(RotX));
            NotifyPropertyChanged(nameof(RotY));
            NotifyPropertyChanged(nameof(RotZ));

            NotifyPropertyChanged(nameof(SclX));
            NotifyPropertyChanged(nameof(SclY));
            NotifyPropertyChanged(nameof(SclZ));
        }

        private Matrix CalculateVPMatrix()
        {
            return ViewMatrix * ProjectionMatrix;
        }

        public void Scale(float factor)
        {
            ScaleV += factor * new Vector3(ScaleVar, ScaleVar, ScaleVar);
            if(ScaleV.X <= 0.01f)
            {
                ScaleV = new Vector3(0.01f, 0.01f, 0.01f);
            }
            Update();
        }

        public void Rotate(Vector3 rotation)
        {
            RotationV += new Vector3(RotateVar * (float)rotation.Y, RotateVar * (float)rotation.X, 0f);
            Update();
        }

        public void Translate(Vector3 translation)
        {
            TranslationV += TranslationVar * translation;
            Update();
        }

        public Vector4 ScreenToWorld(float screenX, float screenY, float screenZ = 1f, bool isW0 = true)
        {
            float x = (float)(screenX / width) * 2f - 1f;
            float y = (float)(screenY / height) * 2f - 1f;
            Vector4 screenPos = new Vector4(x, -y, screenZ, 1);
            screenPos = Vector4.Transform(screenPos, Matrix.Invert(ProjectionMatrix));
            if (isW0)
            {
                screenPos = new Vector4(screenPos.X, screenPos.Y, screenPos.Z, 0);
            }
            Vector4 worldPos = Vector4.Transform(screenPos, SharpDX.Matrix.Invert(ViewMatrix));
            return worldPos;
        }

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public float PosX => TranslationV.X;
        public float PosY => TranslationV.Y;
        public float PosZ => TranslationV.Z;

        public float RotX => RotationV.X;
        public float RotY => RotationV.Y;
        public float RotZ => RotationV.Z;

        public float SclX => ScaleV.X;
        public float SclY => ScaleV.Y;
        public float SclZ => ScaleV.Z;
    }
}
