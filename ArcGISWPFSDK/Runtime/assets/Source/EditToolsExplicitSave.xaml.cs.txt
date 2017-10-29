using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;

using ESRI.ArcGIS.Client;

namespace ArcGISWPFSDK
{
    public partial class EditToolsExplicitSave : UserControl
    {
        public EditToolsExplicitSave()
        {
            InitializeComponent();
        }

        private void CancelEditsButton_Click(object sender, RoutedEventArgs e)
        {
            Editor editor = LayoutRoot.Resources["MyEditor"] as Editor;
            foreach (GraphicsLayer graphicsLayer in editor.GraphicsLayers)
            {
                if (graphicsLayer is FeatureLayer)
                {
                    FeatureLayer featureLayer = graphicsLayer as FeatureLayer;
                    if (featureLayer.HasEdits)
                        featureLayer.Update();

                }
            }
        }

        private void MyMap_Loaded(object sender, RoutedEventArgs e)
        {
          ESRI.ArcGIS.Client.Editor myEditor = LayoutRoot.Resources["MyEditor"] as ESRI.ArcGIS.Client.Editor;
          myEditor.Map = MyMap;
        }
    }
}
