using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Tasks;
using ESRI.ArcGIS.Client.Symbols;

namespace ArcGISWPFSDK
{
    public partial class ViewShed : UserControl
    {
        Geoprocessor _geoprocessorTask;

        public ViewShed()
        {
            InitializeComponent();
            _geoprocessorTask = new Geoprocessor("http://sampleserver1.arcgisonline.com/ArcGIS/rest/services/" +
                "Elevation/ESRI_Elevation_World/GPServer/Viewshed");
            _geoprocessorTask.ExecuteCompleted += GeoprocessorTask_ExecuteCompleted;
            _geoprocessorTask.Failed += GeoprocessorTask_Failed;        
        }

        private void MyMap_MouseClick(object sender, ESRI.ArcGIS.Client.Map.MouseEventArgs e)
        {
            _geoprocessorTask.CancelAsync();

            GraphicsLayer graphicsLayer = MyMap.Layers["MyGraphicsLayer"] as GraphicsLayer;
            graphicsLayer.ClearGraphics();

            MapPoint mapPoint = e.MapPoint;
            mapPoint.SpatialReference = new SpatialReference(4326);

            Graphic graphic = new Graphic()
            {
                Symbol = LayoutRoot.Resources["StartMarkerSymbol"] as Symbol,
                Geometry = mapPoint
            };
            graphicsLayer.Graphics.Add(graphic);

            MyMap.Cursor = System.Windows.Input.Cursors.Wait;

            List<GPParameter> parameters = new List<GPParameter>();
            parameters.Add(new GPFeatureRecordSetLayer("Input_Observation_Point", mapPoint));
            parameters.Add(new GPLinearUnit("Viewshed_Distance", esriUnits.esriMiles, Convert.ToDouble(MilesTextBox.Text)));

            _geoprocessorTask.OutputSpatialReference = new SpatialReference(4326);
            _geoprocessorTask.ExecuteAsync(parameters);
        }

        private void GeoprocessorTask_ExecuteCompleted(object sender, ESRI.ArcGIS.Client.Tasks.GPExecuteCompleteEventArgs args)
        {
            MyMap.Cursor = System.Windows.Input.Cursors.Hand;
            GraphicsLayer graphicsLayer = MyMap.Layers["MyGraphicsLayer"] as GraphicsLayer;

            foreach (GPParameter gpParameter in args.Results.OutParameters)
            {
                if (gpParameter is GPFeatureRecordSetLayer)
                {
                    GPFeatureRecordSetLayer layer = gpParameter as GPFeatureRecordSetLayer;
                    foreach (Graphic graphic in layer.FeatureSet.Features)
                    {
                        graphic.Symbol =LayoutRoot.Resources["DefaultFillSymbol"] as Symbol;
                        graphicsLayer.Graphics.Add(graphic);
                    }
                }
            }
        }

        private void GeoprocessorTask_Failed(object sender, TaskFailedEventArgs e)
        {
            MyMap.Cursor = System.Windows.Input.Cursors.Hand;
            MessageBox.Show("Geoprocessor service failed: " + e.Error);
        }
    }
}
