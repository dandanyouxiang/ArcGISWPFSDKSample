﻿using System.Windows.Controls;
using System.Windows.Input;
using ESRI.ArcGIS.Client;

namespace ArcGISWPFSDK
{
    public partial class FeatureDataGrid : UserControl
    {
        public FeatureDataGrid()
        {
            InitializeComponent();
        }

        private void FeatureLayer_MouseLeftButtonDown(object sender, GraphicMouseButtonEventArgs args)
        {
            args.Graphic.Selected = !args.Graphic.Selected;
            if (args.Graphic.Selected)
                MyDataGrid.ScrollIntoView(args.Graphic, null);
        }
        private void MyDataGrid_Initialized(object sender, System.EventArgs e)
        {
          MyDataGrid.GraphicsLayer = MyMap.Layers[1] as GraphicsLayer;
        }
    }
}
