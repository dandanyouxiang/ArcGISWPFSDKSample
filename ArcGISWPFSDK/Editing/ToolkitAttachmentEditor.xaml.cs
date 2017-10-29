using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;


namespace ArcGISWPFSDK
{
  public partial class ToolkitAttachmentEditor : UserControl
  {
    private FeatureLayer featureLayer;
		private static ESRI.ArcGIS.Client.Projection.WebMercator _mercator =
						new ESRI.ArcGIS.Client.Projection.WebMercator();

    public ToolkitAttachmentEditor()
    {
      InitializeComponent();
      featureLayer = MyMap.Layers[1] as FeatureLayer;
			ESRI.ArcGIS.Client.Geometry.Envelope initialExtent =
										new ESRI.ArcGIS.Client.Geometry.Envelope(
								_mercator.FromGeographic(
										new ESRI.ArcGIS.Client.Geometry.MapPoint(-122.4306073721,37.7666097907)) as MapPoint,
								_mercator.FromGeographic(
										new ESRI.ArcGIS.Client.Geometry.MapPoint(-122.4230971868,37.77197420877)) as MapPoint);

			initialExtent.SpatialReference = new SpatialReference(102100);

			MyMap.Extent = initialExtent;
    }

    private void FeatureLayer_Initialized(object sender, EventArgs e)
    {
      if (featureLayer != null)
        featureLayer.MouseLeftButtonUp += FeatureLayer_MouseLeftButtonUp;
    }

    private void FeatureLayer_MouseLeftButtonUp(object sender, GraphicMouseButtonEventArgs e)
    {
      foreach (Graphic g in featureLayer.Graphics)
        if (g.Selected)
          g.UnSelect();

      e.Graphic.Select();
      attachmentEditor.GraphicSource = e.Graphic;
    }

    private void attachmentEditor_UploadFailed(object sender, ESRI.ArcGIS.Client.Toolkit.AttachmentEditor.UploadFailedEventArgs e)
    {
      MessageBox.Show(e.Result.Message);
    }

    private void MyMap_Loaded(object sender, RoutedEventArgs e)
    {
      attachmentEditor.FeatureLayer=featureLayer;
    }
  }
}