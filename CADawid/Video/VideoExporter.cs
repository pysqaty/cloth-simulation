using CADawid.DxModule;
using CADawid.Simulation;
using CADawid.Simulation.Simulation;
using CADawid.Utils;
using FFMediaToolkit.Encoding;
using Microsoft.Win32;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Video
{
    public class VideoExporter : INotifyPropertyChanged
    {
        public DxRenderer dxRenderer { get; private set; }
        public ClothSimulation Simulation { get; private set; }

        private bool SaveMeta = true;

        public bool isRecording;
        public bool IsRecording
        {
            get => isRecording;
            set
            {
                isRecording = value;
                NotifyPropertyChanged(nameof(IsRecording));
            }
        }

        public string FileName { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
        public int FrameRate { get; set; }

        private MediaOutput Writer { get; set; }

        public VideoExporter(DxRenderer dxr, ClothSimulation simulation)
        {
            string externalsPath = System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\..\\..\\externals";
            FFMediaToolkit.FFmpegLoader.FFmpegPath = externalsPath;//"C:\\Users\\dawid\\source\\repos\\masterthesis\\externals";
            Simulation = simulation;
            dxRenderer = dxr;
            Width = dxr.Width;
            Height = dxr.Height;
            FrameRate = (int)(1 / Simulation.dt);
            IsRecording = false;
        }

        public void Start()
        {
            Simulation.State = CADawid.Simulation.SimulationState.Paused;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "mp4 files (*.mp4)|*.mp4|All files (*.*)|*.*";
            saveFileDialog.DefaultExt = "mp4";
            if (saveFileDialog.ShowDialog() == true)
            {
                lock(dxRenderer.renderMutex)
                {
                    FileName = saveFileDialog.FileName;

                    if(SaveMeta)
                    {
                        using(StreamWriter swMeta = new StreamWriter(FileName + ".txt"))
                        {
                            swMeta.Write(Simulation.GetMetaData());
                            swMeta.Close();
                        }
                    }

                    //size must be even
                    var settings = new VideoEncoderSettings(width: Width % 2 == 0 ? Width : Width - 1, height: Height % 2 == 0 ? Height : Height - 1, framerate: FrameRate, codec: VideoCodec.H264);
                    settings.EncoderPreset = EncoderPreset.Fast;
                    settings.CRF = 17;
                    Writer = MediaBuilder.CreateContainer(FileName).WithVideo(settings).Create();
                    IsRecording = true;
                }
            }
            Simulation.State = CADawid.Simulation.SimulationState.Running;
        }

        public void Stop(SimulationInfo info)
        {
            Simulation.State = SimulationState.Paused;
            lock(dxRenderer.renderMutex)
            {
                IsRecording = false;
                Writer.Dispose();

                if(SaveMeta)
                {
                    using (StreamWriter swUpdateTimes = new StreamWriter(FileName + ".csv"))
                    {
                        swUpdateTimes.WriteLine("FullTime;BVHTreeUpdateTime;DynamicTime;CollisionsTime;ClothCollisionsTime;StrainLimitTime;VisualizationUpdateTime");
                        for (int i = 0; i < info.UpdateTimes.Count - 1; i++)
                        {
                            float t = info.UpdateTimes[i];
                            var times = info.PartialTimes[i];
                            swUpdateTimes.WriteLine(t + ";" + times.BVHTreeUpdateTime + ";" + times.DynamicTime + ";" + times.CollisionsTime + ";" +
                                times.ClothCollisionsTime + ";" + times.StrainLimitTime + ";" + times.VisualizationUpdateTime);
                        }
                        swUpdateTimes.Close();
                    }
                }
            }
            Simulation.State = SimulationState.Running;
        }


        public void Record()
        {
            if(IsRecording)
            {
                lock(dxRenderer.renderMutex)
                {
                    var bmp = dxRenderer.renderedT2.ToDrawingBitmap(dxRenderer.device);
                    var imageData = bmp.ToImageData();
                    Writer?.Video?.AddFrame(imageData);
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
