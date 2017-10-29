﻿using System.Windows.Controls;
using ESRI.ArcGIS.Client.WebMap;

namespace ArcGISWPFSDK
{
    public partial class WebMapWMS : UserControl
    {
        public WebMapWMS()
        {
            InitializeComponent();

            Document webMap = new Document();
            webMap.GetMapCompleted += webMap_GetMapCompleted;

            webMap.GetMapAsync("b3e11e1d7aac4d6a98fde6b864d3a2b7");
        }

        void webMap_GetMapCompleted(object sender, GetMapCompletedEventArgs e)
        {
            if (e.Error == null)
                LayoutRoot.Children.Add(e.Map);          
        }
    }
}
