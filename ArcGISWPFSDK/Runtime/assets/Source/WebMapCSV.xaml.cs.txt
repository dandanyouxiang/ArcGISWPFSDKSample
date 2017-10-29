using System.Windows.Controls;
using ESRI.ArcGIS.Client.WebMap;
using ESRI.ArcGIS.Client;
using System.Windows.Media;

namespace ArcGISWPFSDK
{
  public partial class WebMapCSV : UserControl
  {
    public WebMapCSV()
    {
      InitializeComponent();

      Document webMap = new Document();
      webMap.GetMapCompleted += webMap_GetMapCompleted;

      webMap.GetMapAsync("e64c82296b5a48acb0a7f18e3f556607");
    }

    void webMap_GetMapCompleted(object sender, GetMapCompletedEventArgs e)
    {
      if (e.Error == null)
        LayoutRoot.Children.Add(e.Map);
      int i = 0;

      foreach (Layer layer in e.Map.Layers)
      {
        layer.ID = i.ToString();
        if (layer is FeatureLayer)
        {
          Border maptip = (layer as FeatureLayer).MapTip as Border;
          ContentControl scv = maptip.Child as ContentControl;
          scv.Foreground = new SolidColorBrush(Colors.Black);
        }
        i++;
      }
    }
  }
}
