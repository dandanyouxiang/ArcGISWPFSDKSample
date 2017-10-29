using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ArcGISWPFSDK
{
    public partial class LogosFade : UserControl
    {
        public LogosFade()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard sb = Logos.Resources["slStoryboard"] as Storyboard;
            sb.Begin();
        }
    }
}
