	using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Tasks;

namespace ArcGISWPFSDK
{
    public partial class BufferPoint : UserControl
    {
        public BufferPoint()
        {
            InitializeComponent();
        }

        private void MyMap_MouseClick(object sender, ESRI.ArcGIS.Client.Map.MouseEventArgs e)
        {
            GraphicsLayer graphicsLayer = MyMap.Layers["MyGraphicsLayer"] as GraphicsLayer;
            graphicsLayer.ClearGraphics();

						e.MapPoint.SpatialReference = MyMap.SpatialReference;
            Graphic graphic = new ESRI.ArcGIS.Client.Graphic()
            {
                Geometry = e.MapPoint,
                Symbol = LayoutRoot.Resources["DefaultClickSymbol"] as ESRI.ArcGIS.Client.Symbols.Symbol
            };
            graphic.SetZIndex(1);
            graphicsLayer.Graphics.Add(graphic);

						GeometryService geometryService =
							new GeometryService("http://tasks.arcgisonline.com/ArcGIS/rest/services/Geometry/GeometryServer");
						geometryService.BufferCompleted += GeometryService_BufferCompleted;
						geometryService.Failed += GeometryService_Failed;

						// If buffer spatial reference is GCS and unit is linear, geometry service will do geodesic buffering
						BufferParameters bufferParams = new BufferParameters()
						{
							Unit = LinearUnit.StatuteMile,
							BufferSpatialReference = new SpatialReference(4326),
							OutSpatialReference = MyMap.SpatialReference
						};
						bufferParams.Features.Add(graphic);
						bufferParams.Distances.Add(5);

            geometryService.BufferAsync(bufferParams);
        }

        void GeometryService_BufferCompleted(object sender, GraphicsEventArgs args)
        {
            IList<Graphic> results = args.Results;
            GraphicsLayer graphicsLayer = MyMap.Layers["MyGraphicsLayer"] as GraphicsLayer;

            foreach (Graphic graphic in results)
            {
              graphic.Symbol = LayoutRoot.Resources["DefaultBufferSymbol"] as ESRI.ArcGIS.Client.Symbols.Symbol;
                graphicsLayer.Graphics.Add(graphic);
            }
        }

        private void GeometryService_Failed(object sender, TaskFailedEventArgs e)
        {
            MessageBox.Show("Geometry Service error: " + e.Error);
        }
    }
}
