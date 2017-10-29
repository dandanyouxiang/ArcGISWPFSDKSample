using System;
using System.Windows.Controls;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Toolkit;

namespace ArcGISWPFSDK
{
    public partial class TemporalRendererFeatureLayer : UserControl
    {
        public TemporalRendererFeatureLayer()
        {
            InitializeComponent();
        }

        private void FeatureLayer_Initialized(object sender, EventArgs e)
        {
            TimeExtent extent = new TimeExtent(timeSlider.MinimumValue, timeSlider.MaximumValue);
            timeSlider.Intervals = TimeSlider.CreateTimeStopsByTimeInterval(extent, new TimeSpan(0, 6, 0, 0, 0));
        }
    }
}
