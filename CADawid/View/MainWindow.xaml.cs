using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using SharpDX;
using CADawid.DxModule;
using CADawid.Utils;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using CADawid.View.SimulationPanel;
using CADawid.Simulation.Simulation;
using CADawid.Simulation;
using System.Runtime.InteropServices;
using CADawid.Video;

namespace CADawid
{
    public partial class MainWindow : Window, ISimulationPanel
    {
        private TimeSpan lastRender;
        private bool lastVisible;
        private System.Windows.Point mousePosition;

        private Stopwatch stopwatch;
        public SimulationInfo SimulationInfo;

        private DxRenderer dxRenderer;

        private BackgroundWorker simulationThread;

        public VideoExporter VideoExporter { get; set; }

        private void simulationThread_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if(dxRenderer.Scene.ClothSimulation.State == SimulationState.Running)
                {
                    VideoExporter?.Record();
                    stopwatch.Start();
                    var times = dxRenderer.Scene.Update();
                    stopwatch.Stop();
                    var elapsed = stopwatch.ElapsedMilliseconds;
                    stopwatch.Reset();
                    long timeToSleep = (long)(dxRenderer.Scene.ClothSimulation.dt * 1000 - elapsed);
                    SimulationInfo.UpdateTime = elapsed;
                    SimulationInfo.AddUpdateTime(times);
                    Thread.Sleep(timeToSleep >= 0 ? (int)timeToSleep : 0);
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.host.Loaded += new RoutedEventHandler(this.Host_Loaded);
            this.host.SizeChanged += new SizeChangedEventHandler(this.Host_SizeChanged);   
        }

        private void Host_Loaded(object sender, RoutedEventArgs e)
        {
            dxRenderer = new DxRenderer();
            dxRenderer.Scene.ClothSimulation.Visit(this);
            SimulationInfo = new SimulationInfo();
            stopwatch = new Stopwatch();
            simulationThread = new BackgroundWorker();
            simulationThread.DoWork +=
                new DoWorkEventHandler(simulationThread_DoWork);
            simulationThread.RunWorkerAsync();
            this.InitializeRendering();
            PropertiesPanel.DataContext = dxRenderer.Scene.ClothSimulation;
            SimulationInfoPanel.DataContext = SimulationInfo;
            CameraPanel.DataContext = dxRenderer.Scene.Camera;
            SimulationPanel.DataContext = dxRenderer.Scene.ClothSimulation;
        }

        private void Host_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double dpiScale = 1.0;

            int surfWidth = (int)(host.ActualWidth < 0 ? 0 : Math.Ceiling(host.ActualWidth * dpiScale));
            int surfHeight = (int)(host.ActualHeight < 0 ? 0 : Math.Ceiling(host.ActualHeight * dpiScale));

            InteropImage.SetPixelSize(surfWidth, surfHeight);

            bool isVisible = (surfWidth != 0 && surfHeight != 0);
            if (lastVisible != isVisible)
            {
                lastVisible = isVisible;
                if (lastVisible)
                {
                    CompositionTarget.Rendering += CompositionTarget_Rendering;
                }
                else
                {
                    CompositionTarget.Rendering -= CompositionTarget_Rendering;
                }
            }
        }

        private void InitializeRendering()
        {
            IntPtr windowHandle = new WindowInteropHelper(this).Handle;
            var d3d = new SharpDX.Direct3D9.Direct3D();
            var d3d2 = new SharpDX.Direct3D9.Direct3D();
            var presentparams = new SharpDX.Direct3D9.PresentParameters(1, 1);
            SharpDX.Direct3D9.Device d3d9Device = new SharpDX.Direct3D9.Device(d3d, 0,
                SharpDX.Direct3D9.DeviceType.Hardware, windowHandle, SharpDX.Direct3D9.CreateFlags.HardwareVertexProcessing, presentparams);
            var presentparams2 = new SharpDX.Direct3D9.PresentParameters(1, 1);
            SharpDX.Direct3D9.Device d3d9Device2 = new SharpDX.Direct3D9.Device(d3d2, 0,
                SharpDX.Direct3D9.DeviceType.Hardware, windowHandle, SharpDX.Direct3D9.CreateFlags.HardwareVertexProcessing, presentparams);
            var surface = d3d9Device.GetBackBuffer(0, 0);
            var surface2 = d3d9Device2.GetBackBuffer(0, 0);

            InteropImage.Lock();
            InteropImage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, surface.NativePointer);
            InteropImage.Unlock();

            InteropImage.WindowOwner = (new System.Windows.Interop.WindowInteropHelper(this)).Handle;
            InteropImage.OnRender = this.DoRender;
            InteropImage.RequestRender();

            VideoExporter = new VideoExporter(dxRenderer, dxRenderer.Scene.ClothSimulation);
            RecordingPanel.DataContext = VideoExporter;
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            RenderingEventArgs args = (RenderingEventArgs)e;

            if (this.lastRender != args.RenderingTime)
            {
                InteropImage.RequestRender();
                SimulationInfo.FPS = (float)(1.0 / (args.RenderingTime.TotalSeconds - this.lastRender.TotalSeconds));
                this.lastRender = args.RenderingTime;
            }
        }

        private void UninitializeRendering()
        {
            CompositionTarget.Rendering -= this.CompositionTarget_Rendering;
        }

        private void DoRender(IntPtr surface, bool isNewSurface)
        {
            dxRenderer.Render(surface, isNewSurface);
        }

        private void ImageHost_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            dxRenderer.Scene.Camera.Scale(e.Delta > 0 ? 1f : -1f);
        }

        private void ImageHost_MouseMove(object sender, MouseEventArgs e)
        {
            System.Windows.Point currentPosition = e.GetPosition(this);
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                Vector3 difference = new Vector3((float)(currentPosition.X - mousePosition.X), (float)(currentPosition.Y - mousePosition.Y), 0f);
                dxRenderer.Scene.Camera.Rotate(difference);
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                Vector4 difference = new Vector4((float)(currentPosition.X - mousePosition.X), (float)(-currentPosition.Y + mousePosition.Y), 0f, 1f);
                dxRenderer.Scene.Camera.Translate(MathExt.RotateVector(difference, dxRenderer.Scene.Camera.R));
            }
            else if(e.LeftButton == MouseButtonState.Pressed)
            {
                Vector4 difference = new Vector4((float)(currentPosition.X - mousePosition.X), (float)(-currentPosition.Y + mousePosition.Y), 0f, 1f);
                dxRenderer.Scene.HandleInput(difference);
            }
            mousePosition = currentPosition;
        }




        private void ImageHost_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void ImageHost_MouseDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            ImageHost.Focusable = true;
            ImageHost.Focus();
        }

        

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

       

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            dxRenderer.Scene.HandleKeyUp(e.Key);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            dxRenderer.Scene.Reset();
            SimulationInfo.Reset();
        }

        public void Accept(MassSpringClothSimulation simulation)
        {
            SimulationProperties.Children.Clear();
            SimulationProperties.Children.Add(new MassSpringPanel(simulation));
        }

        public void Accept(InequalityClothSimulation simulation)
        {
            SimulationProperties.Children.Clear();
            SimulationProperties.Children.Add(new InequalityClothPanel(simulation));
        }

        private void RecordingButton_Click(object sender, RoutedEventArgs e)
        {
            if (VideoExporter.IsRecording)
            {
                VideoExporter.Stop(SimulationInfo);
            }
            else
            {
                VideoExporter.Start();
            }
        }

        private void ResetAndRecordButton_Click(object sender, RoutedEventArgs e)
        {
            dxRenderer.Scene.Reset();
            VideoExporter.Start();
        }
    }
}
