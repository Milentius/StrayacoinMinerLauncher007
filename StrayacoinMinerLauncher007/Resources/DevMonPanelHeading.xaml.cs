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
    /// Interaction logic for DevMonPanelHeading.xaml
    /// </summary>
    public partial class DevMonPanelHeading : UserControl
    {

        // property for the sensor name
        private string headingText;

        public string HeadingText
        {
            get { return headingText; }
            set { headingText = value; lblHeadingText.Content = headingText; }
        }

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
            }
        }

        public DevMonPanelHeading()
        {
            InitializeComponent();
        }
    }
}
