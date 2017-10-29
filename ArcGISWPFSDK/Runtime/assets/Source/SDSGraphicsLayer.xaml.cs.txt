using System;
using System.Windows;
using System.Windows.Controls;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Tasks;

namespace ArcGISWPFSDK
{
  public partial class SDSGraphicsLayer : UserControl
  {
    public SDSGraphicsLayer()
    {
      InitializeComponent();

      // Wait for Map spatial reference to be set by first layer. Unnecessary if set explicitly.  
      MyMap.Layers.LayersInitialized += Layers_LayersInitialized;
    }

    void Layers_LayersInitialized(object sender, EventArgs args)
    {
      LoadGraphics();
    }

    private void LoadGraphics()
    {
      QueryTask queryTask =
          new QueryTask("http://mapit.cloudapp.net/databases/Demo/dbo.WorldCities");

      queryTask.ExecuteCompleted += queryTask_ExecuteCompleted;
      queryTask.DisableClientCaching = true;

      Query query = new ESRI.ArcGIS.Client.Tasks.Query();
      query.OutSpatialReference = MyMap.SpatialReference;
      query.ReturnGeometry = true;
      query.Where = "1=1";
      // Note, query.Text is not supported for use with Spatial Data Services

      query.OutFields.AddRange(new string[] { "NAME", "POPULATION" });
      queryTask.ExecuteAsync(query);
    }

    void queryTask_ExecuteCompleted(object sender, QueryEventArgs args)
    {
      FeatureSet featureSet = args.FeatureSet;

      if (featureSet == null || featureSet.Features.Count < 1)
      {
        MessageBox.Show("No features returned from query");
        return;
      }

      GraphicsLayer graphicsLayer = MyMap.Layers["MyGraphicsLayer"] as GraphicsLayer;

      foreach (Graphic graphic in featureSet.Features)
      {
        graphic.Symbol = LayoutRoot.Resources["YellowMarkerSymbol"] as ESRI.ArcGIS.Client.Symbols.Symbol;
        graphicsLayer.Graphics.Add(graphic);
      }
    }
  }
}
