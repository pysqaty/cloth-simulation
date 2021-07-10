using CADawid.Simulation.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CADawid.View.SimulationPanel
{
    /// <summary>
    /// Interaction logic for MassSpringPanel.xaml
    /// </summary>
    public partial class MassSpringPanel : UserControl
    {
        public MassSpringClothSimulation Simulation { get; set; }
        public MassSpringPanel(MassSpringClothSimulation simulation)
        {
            Simulation = simulation;
            InitializeComponent();
            DataContext = Simulation;
        }
    }
}
