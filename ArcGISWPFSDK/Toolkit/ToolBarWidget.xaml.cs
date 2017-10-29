using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;

namespace ArcGISWPFSDK
{
    public partial class ToolBarWidget : UserControl
    {
        string _toolMode = "";
        List<Envelope> _extentHistory = new List<Envelope>();
        int _currentExtentIndex = 0;
        bool _newExtent = true;

        Image _previousExtentImage;
        Image _nextExtentImage;
		private Draw MyDrawObject;
		
        public ToolBarWidget()
        {
            InitializeComponent();
			MyDrawObject = new Draw(MyMap)
			{
				FillSymbol = LayoutRoot.Resources["DefaultFillSymbol"] as ESRI.ArcGIS.Client.Symbols.FillSymbol,
				DrawMode = DrawMode.Rectangle
			};

			MyDrawObject.DrawComplete += myDrawObject_DrawComplete;
        }

        private void MyToolbar_Loaded(object sender, RoutedEventArgs e)
        {
            _previousExtentImage = MyToolbar.Items[3].Content as Image;
            _nextExtentImage = MyToolbar.Items[4].Content as Image;
        }

        private void MyToolbar_ToolbarItemClicked(object sender, ESRI.ArcGIS.Client.Toolkit.SelectedToolbarItemArgs e)
        {
            MyDrawObject.IsEnabled = false;
            _toolMode = "";
            switch (e.Index)
            {
                case 0: // ZoomIn Layers
                    MyDrawObject.IsEnabled = true;
                    _toolMode = "zoomin";
                    break;
                case 1: // Zoom Out
					MyDrawObject.IsEnabled = true;
                    _toolMode = "zoomout";
                    break;
                case 2: // Pan
                    break;
                case 3: // Previous Extent
                    if (_currentExtentIndex != 0)
                    {
                        _currentExtentIndex--;
                        
                        if (_currentExtentIndex == 0)
                        {
                            _previousExtentImage.Opacity = 0.3;
                            _previousExtentImage.IsHitTestVisible = false;
                        }

                        _newExtent = false;

                        MyMap.IsHitTestVisible = false;
                        MyMap.ZoomTo(_extentHistory[_currentExtentIndex]);

                        if (_nextExtentImage.IsHitTestVisible == false)
                        {
                            _nextExtentImage.Opacity = 1;
                            _nextExtentImage.IsHitTestVisible = true;
                        }
                    }
                    break;
                case 4: // Next Extent
                    if (_currentExtentIndex < _extentHistory.Count - 1)
                    {
                        _currentExtentIndex++;

                        if (_currentExtentIndex == (_extentHistory.Count - 1))
                        {
                            _nextExtentImage.Opacity = 0.3;
                            _nextExtentImage.IsHitTestVisible = false;
                        }

                        _newExtent = false;

                        MyMap.IsHitTestVisible = false;
                        MyMap.ZoomTo(_extentHistory[_currentExtentIndex]);

                        if (_previousExtentImage.IsHitTestVisible == false)
                        {
                            _previousExtentImage.Opacity = 1;
                            _previousExtentImage.IsHitTestVisible = true;
                        }
                    }
                    break;
                case 5: // Full Extent
                    MyMap.ZoomTo(MyMap.Layers.GetFullExtent());
                    break;
                case 6: // Full Screen
                    Window window = Window.GetWindow(this);
                    
                    if (window.WindowState == WindowState.Maximized)
                    {
                        window.WindowState = WindowState.Normal;
                        window.WindowStyle = WindowStyle.SingleBorderWindow;
                    }
                    else
                    {
                        window.WindowState = WindowState.Normal;
                        window.SizeToContent = SizeToContent.Manual;
                        window.WindowStyle = WindowStyle.None;
                        window.Topmost = true;
                        window.WindowState = WindowState.Maximized;
                    }                    
                    break;
            }
        }

        private void MyToolbar_ToolbarIndexChanged(object sender, ESRI.ArcGIS.Client.Toolkit.SelectedToolbarItemArgs e)
        {
            StatusTextBlock.Text = e.Item.Text;
        }

        private void myDrawObject_DrawComplete(object sender, DrawEventArgs args)
        {
            if (_toolMode == "zoomin")
            {
                MyMap.ZoomTo(args.Geometry as ESRI.ArcGIS.Client.Geometry.Envelope);
            }
            else if (_toolMode == "zoomout")
            {
                Envelope currentExtent = MyMap.Extent;

                Envelope zoomBoxExtent = args.Geometry as Envelope;
                MapPoint zoomBoxCenter = zoomBoxExtent.GetCenter();

                double whRatioCurrent = currentExtent.Width / currentExtent.Height;
                double whRatioZoomBox = zoomBoxExtent.Width / zoomBoxExtent.Height;

                Envelope newEnv = null;

                if (whRatioZoomBox > whRatioCurrent)
                // use width
                {
                    double mapWidthPixels = MyMap.Width;
                    double multiplier = currentExtent.Width / zoomBoxExtent.Width;
                    double newWidthMapUnits = currentExtent.Width * multiplier;
                    newEnv = new Envelope(new MapPoint(zoomBoxCenter.X - (newWidthMapUnits / 2), zoomBoxCenter.Y),
                                                   new MapPoint(zoomBoxCenter.X + (newWidthMapUnits / 2), zoomBoxCenter.Y));
                }
                else
                // use height
                {
                    double mapHeightPixels = MyMap.Height;
                    double multiplier = currentExtent.Height / zoomBoxExtent.Height;
                    double newHeightMapUnits = currentExtent.Height * multiplier;
                    newEnv = new Envelope(new MapPoint(zoomBoxCenter.X, zoomBoxCenter.Y - (newHeightMapUnits / 2)),
                                                   new MapPoint(zoomBoxCenter.X, zoomBoxCenter.Y + (newHeightMapUnits / 2)));
                }

                if (newEnv != null)
                    MyMap.ZoomTo(newEnv);
            }
        }

        private void MyMap_ExtentChanged(object sender, ExtentEventArgs e)
        {
            if (e.OldExtent == null)
            {
                _extentHistory.Add(e.NewExtent.Clone());
                return;
            }

            if (_newExtent)
            {
                _currentExtentIndex++;

                if (_extentHistory.Count - _currentExtentIndex > 0)
                    _extentHistory.RemoveRange(_currentExtentIndex, (_extentHistory.Count - _currentExtentIndex));

                if (_nextExtentImage.IsHitTestVisible == true)
                {
                    _nextExtentImage.Opacity = 0.3;
                    _nextExtentImage.IsHitTestVisible = false;
                }

                _extentHistory.Add(e.NewExtent.Clone());

                if (_previousExtentImage.IsHitTestVisible == false)
                {
                    _previousExtentImage.Opacity = 1;
                    _previousExtentImage.IsHitTestVisible = true;
                }
            }
            else
            {
                MyMap.IsHitTestVisible = true;
                _newExtent = true;
            }
        }
    }
}
