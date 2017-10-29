using System.Windows.Controls;
using System.Windows.Input;

namespace ArcGISWPFSDK
{
    public partial class Magnify : UserControl
    {
        public Magnify()
        {
            InitializeComponent();
        }

        private void MyMagnifyImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyMagnifier.Enabled = !MyMagnifier.Enabled;
        }
    }
}
