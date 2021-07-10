using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADawid.Simulation
{
    public class SimulationInfo : INotifyPropertyChanged
    {
        public float UpdateTime
        {
            get => updateTime;
            set
            {
                updateTime = value;
                NotifyPropertyChanged(nameof(UpdateTime));
            }
        }
        public float FPS
        {
            get => fps;
            set
            {
                fps = value;
                NotifyPropertyChanged(nameof(FPS));
            }
        }

        public List<float> UpdateTimes { get; set; }
        public List<SimulationTimes> PartialTimes { get; set; }

        private float updateTime;
        private float fps;

        public SimulationInfo()
        {
            UpdateTimes = new List<float>();
            PartialTimes = new List<SimulationTimes>();
        }

        public void Reset()
        {
            UpdateTimes.Clear();
            PartialTimes.Clear();
        }

        public void AddUpdateTime(SimulationTimes times)
        {
            UpdateTimes.Add(UpdateTime);
            PartialTimes.Add(times);
        }


        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
