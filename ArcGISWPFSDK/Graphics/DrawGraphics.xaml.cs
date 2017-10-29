using System.Windows.Controls;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Symbols;

namespace ArcGISWPFSDK
{
    public partial class DrawGraphics : UserControl
    {
        private Draw MyDrawObject;
        private Symbol _activeSymbol = null;

        public DrawGraphics()
        {
            InitializeComponent();

            MyDrawObject = new Draw(MyMap)
            {
                LineSymbol = LayoutRoot.Resources["DrawLineSymbol"] as LineSymbol,
                FillSymbol = LayoutRoot.Resources["DrawFillSymbol"] as FillSymbol
            };

            MyDrawObject.DrawComplete += MyDrawObject_DrawComplete;
        }

        private void MyToolbar_ToolbarItemClicked(object sender, ESRI.ArcGIS.Client.Toolkit.SelectedToolbarItemArgs e)
        {
            switch (e.Index)
            {
                case 0: // Point
                    MyDrawObject.DrawMode = DrawMode.Point;
                    _activeSymbol = LayoutRoot.Resources["DefaultMarkerSymbol"] as Symbol;
                    break;
                case 1: // Polyline
                    MyDrawObject.DrawMode = DrawMode.Polyline;
                    _activeSymbol = LayoutRoot.Resources["DefaultLineSymbol"] as Symbol;
                    break;
                case 2: // Polygon
                    MyDrawObject.DrawMode = DrawMode.Polygon;
                    _activeSymbol = LayoutRoot.Resources["DefaultFillSymbol"] as Symbol;
                    break;
                case 3: // Rectangle
                    MyDrawObject.DrawMode = DrawMode.Rectangle;
                    _activeSymbol = LayoutRoot.Resources["DefaultFillSymbol"] as Symbol;
                    break;
                case 4: // Freehand
                    MyDrawObject.DrawMode = DrawMode.Freehand;
                    _activeSymbol = LayoutRoot.Resources["DefaultLineSymbol"] as Symbol;
                    break;
								case 5: // Arrow
										MyDrawObject.DrawMode = DrawMode.Arrow;
										_activeSymbol = LayoutRoot.Resources["DefaultFillSymbol"] as Symbol;
										break;
								case 6: // Triangle
										MyDrawObject.DrawMode = DrawMode.Triangle;
										_activeSymbol = LayoutRoot.Resources["DefaultFillSymbol"] as Symbol;
										break;
								case 7: // Circle
										MyDrawObject.DrawMode = DrawMode.Circle;
										_activeSymbol = LayoutRoot.Resources["DefaultFillSymbol"] as Symbol;
										break;
								case 8: // Ellipse
										MyDrawObject.DrawMode = DrawMode.Ellipse;
										_activeSymbol = LayoutRoot.Resources["DefaultFillSymbol"] as Symbol;
										break;
                default: // Clear Graphics
                    MyDrawObject.DrawMode = DrawMode.None;
                    GraphicsLayer graphicsLayer = MyMap.Layers["MyGraphicsLayer"] as GraphicsLayer;
                    graphicsLayer.ClearGraphics();
                    break;
            }
            MyDrawObject.IsEnabled = (MyDrawObject.DrawMode != DrawMode.None);
        }

        private void MyToolbar_ToolbarIndexChanged(object sender, ESRI.ArcGIS.Client.Toolkit.SelectedToolbarItemArgs e)
        {
            StatusTextBlock.Text = e.Item.Text;
        }


        private void MyDrawObject_DrawComplete(object sender, ESRI.ArcGIS.Client.DrawEventArgs args)
        {
            GraphicsLayer graphicsLayer = MyMap.Layers["MyGraphicsLayer"] as GraphicsLayer;
            ESRI.ArcGIS.Client.Graphic graphic = new ESRI.ArcGIS.Client.Graphic()
            {
                Geometry = args.Geometry,
                Symbol = _activeSymbol,
            };
            graphicsLayer.Graphics.Add(graphic);
        }

				private void MyMap_Loaded(object sender,System.Windows.RoutedEventArgs e)
				{
					Editor ed = LayoutRoot.Resources["MyEditor"] as Editor;
					ed.Map=MyMap;
				}
				private void GraphicsLayer_MouseLeftButtonUp(object sender,GraphicMouseButtonEventArgs e)
				{
					if (EnableEditVerticesScaleRotate.IsChecked.Value)
					{
						Editor editor = LayoutRoot.Resources["MyEditor"] as Editor;
						if (e.Graphic != null && !(e.Graphic.Geometry is ESRI.ArcGIS.Client.Geometry.MapPoint))
						{
							editor.EditVertices.Execute(e.Graphic);
						}
					}
				}


    }
}
