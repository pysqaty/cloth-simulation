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
    /// Interaction logic for InequalityClothPanel.xaml
    /// </summary>
    public partial class InequalityClothPanel : UserControl
    {
        public InequalityClothSimulation Simulation { get; set; }
        public InequalityClothPanel(InequalityClothSimulation simulation)
        {
            Simulation = simulation;
            InitializeComponent();
            DataContext = Simulation;
        }
    }
}
