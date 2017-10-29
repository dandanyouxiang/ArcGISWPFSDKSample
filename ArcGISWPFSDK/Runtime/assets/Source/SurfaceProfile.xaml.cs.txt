using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Tasks;
using ESRI.ArcGIS.Client.Symbols;
namespace ArcGISWPFSDK
{
    public partial class SurfaceProfile : UserControl
    {
        private Draw MyDrawObject;

        public SurfaceProfile()
        {
            InitializeComponent();
            MyDrawObject = new Draw(MyMap)
            {
                LineSymbol = LayoutRoot.Resources["BlueLineSymbol"] as LineSymbol,
                DrawMode = DrawMode.Polyline,
                IsEnabled = true
            };

            MyDrawObject.DrawComplete += MyDrawObject_OnDrawComplete;
            MyDrawObject.DrawBegin += MyDrawObject_OnDrawBegin;
        }

        private void MyDrawObject_OnDrawComplete(object sender, DrawEventArgs args)
        {
            GraphicsLayer graphicsLayer = MyMap.Layers["MyGraphicsLayer"] as GraphicsLayer;

            Graphic graphic = new Graphic()
            {
              Symbol = LayoutRoot.Resources["BlueLineSymbol"] as Symbol,
              Geometry = args.Geometry
            };
            graphicsLayer.Graphics.Add(graphic);

            Geoprocessor geoprocessorTask = new Geoprocessor("http://sampleserver2.arcgisonline.com/ArcGIS/rest/services/" +
                "Elevation/ESRI_Elevation_World/GPServer/ProfileService");
            geoprocessorTask.ExecuteCompleted += GeoprocessorTask_ExecuteCompleted;
            geoprocessorTask.Failed += GeoprocessorTask_Failed;

            List<GPParameter> parameters = new List<GPParameter>();
            parameters.Add(new GPFeatureRecordSetLayer("Input_Polylines", args.Geometry));
            parameters.Add(new GPLong("Image_Width", 600));
            parameters.Add(new GPLong("Image_Height", 250));
            parameters.Add(new GPBoolean("Display_Segments", true));

            geoprocessorTask.ExecuteAsync(parameters);
        }

        private void GeoprocessorTask_ExecuteCompleted(object sender, GPExecuteCompleteEventArgs args)
        {
            GPParameter gpParameter = args.Results.OutParameters[0];
            if (gpParameter is GPFeatureRecordSetLayer)
            {
                GPFeatureRecordSetLayer gpLayer = gpParameter as GPFeatureRecordSetLayer;
                ProfileImage.Source = new BitmapImage(new Uri(gpLayer.FeatureSet.Features[0].Attributes["profileURL"].ToString(), UriKind.Absolute));
                ProfileView.Visibility = Visibility.Visible;
            }
        }

        private void MyDrawObject_OnDrawBegin(object sender, EventArgs args)
        {
            GraphicsLayer graphicsLayer = MyMap.Layers["MyGraphicsLayer"] as GraphicsLayer;
            graphicsLayer.ClearGraphics();
            ProfileView.Visibility = Visibility.Collapsed;
        }

        private void CloseProfileImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ProfileView.Visibility = Visibility.Collapsed;
            GraphicsLayer graphicsLayer = MyMap.Layers["MyGraphicsLayer"] as GraphicsLayer;
            graphicsLayer.ClearGraphics();
        }

        private void SizeProfileImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            double width = ProfileImage.Width;
            if (width != 600)
            {
                ProfileImage.Width = 600;
                ProfileImage.Height = 250;
            }
            else
            {
                ProfileImage.Width = 250;
                ProfileImage.Height = 104;
            }
        }

        private void GeoprocessorTask_Failed(object sender, TaskFailedEventArgs e)
        {
            MessageBox.Show("Geoprocessor failed. Error: " + e.Error);
        }
    }
}
