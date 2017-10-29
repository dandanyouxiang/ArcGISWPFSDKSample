using System.Windows.Controls;
using ESRI.ArcGIS.Client;

namespace ArcGISWPFSDK
{
	public partial class SDSRenderersXAML:UserControl
	{
		public SDSRenderersXAML()
		{
			InitializeComponent();
		}
		private void FeatureLayer_UpdateCompleted(object sender,System.EventArgs e)
		{
			MyMap.ZoomTo((sender as FeatureLayer).FullExtent);
		}

	}
}
