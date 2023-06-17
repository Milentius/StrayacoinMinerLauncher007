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

namespace StrayacoinMinerLauncher007.Resources
{
    /// <summary>
    /// Interaction logic for DevMonPanelCurrentOnly.xaml
    /// </summary>
    public partial class DevMonPanelCurrentOnly : UserControl
    {

        private string sensorName;

        public string SensorName
        {
            get { return sensorName; }
            set { sensorName = value; lblSensorName.Content = sensorName; }
        }

        // property for the sensor current value
        private string sensorValue;

        public string SensorValue
        {
            get { return sensorValue; }
            set { sensorValue = value; lblSensorValue.Content = sensorValue.ToString(); }
        }

        // property for the stackpanel background color
        private bool backColor;

        public bool BackColor
        {
            get { return backColor; }
            set
            {
                backColor = value;
                if (backColor == true)
                {
                    BrushConverter converter = new BrushConverter();
                    Brush brush = (Brush)converter.ConvertFromString("#FF373737");
                    StackPanelBase.Background = brush;
                }
                else
                {
                    BrushConverter converter = new BrushConverter();
                    Brush brush = (Brush)converter.ConvertFromString("#FF232323");
                    StackPanelBase.Background = brush;
                }
            }
        }

        public DevMonPanelCurrentOnly()
        {
            InitializeComponent();
        }
    }
}
