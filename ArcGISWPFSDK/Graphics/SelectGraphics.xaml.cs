using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ArcGISWPFSDK
{
    public partial class SelectGraphics : UserControl
    {
		public SelectGraphics()
        {
            InitializeComponent();
        }

        private void GraphicsLayer_MouseLeftButtonDown(object sender, ESRI.ArcGIS.Client.GraphicMouseButtonEventArgs e)
		{
			e.Graphic.Selected = !e.Graphic.Selected;
		}
    }
}
