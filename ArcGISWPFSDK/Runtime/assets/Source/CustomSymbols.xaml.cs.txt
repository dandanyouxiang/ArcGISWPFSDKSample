using System.Windows.Controls;
using System.Windows.Input;

namespace ArcGISWPFSDK
{
  public partial class CustomSymbols : UserControl
  {
    public CustomSymbols()
    {
      InitializeComponent();
    }

    private void GraphicsLayer_MouseLeftButtonDown(object sender, ESRI.ArcGIS.Client.GraphicMouseButtonEventArgs e)
    {
      if (e.Graphic.Selected)
        e.Graphic.UnSelect();
      else
        e.Graphic.Select();
    }
  }
}
