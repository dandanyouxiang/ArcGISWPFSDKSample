using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ArcGISWPFSDK
{
    public partial class SubLayerList : UserControl
    {
        Dictionary<string, int> _layerDictionary = new Dictionary<string, int>();

        public SubLayerList()
        {
            InitializeComponent();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox tickedCheckBox = sender as CheckBox;

            string serviceName = tickedCheckBox.Tag.ToString();
            bool visible = (bool)tickedCheckBox.IsChecked;

            string layerName = (string)tickedCheckBox.Content;
            int layerIndex = _layerDictionary[layerName];

            ESRI.ArcGIS.Client.ArcGISDynamicMapServiceLayer dynamicServiceLayer = MyMap.Layers[serviceName] as
                ESRI.ArcGIS.Client.ArcGISDynamicMapServiceLayer;

            List<int> visibleLayerList =
                dynamicServiceLayer.VisibleLayers != null
                ? dynamicServiceLayer.VisibleLayers.ToList() : new List<int>();

            if (visible)
            {
                if (!visibleLayerList.Contains(layerIndex))
                    visibleLayerList.Add(layerIndex);
            }
            else
            {
                if (visibleLayerList.Contains(layerIndex))
                    visibleLayerList.Remove(layerIndex);
            }

            dynamicServiceLayer.VisibleLayers = visibleLayerList.ToArray();
        }

        private void ArcGISDynamicMapServiceLayer_Initialized(object sender, EventArgs e)
        {
            ESRI.ArcGIS.Client.ArcGISDynamicMapServiceLayer dynamicServiceLayer =
                sender as ESRI.ArcGIS.Client.ArcGISDynamicMapServiceLayer;
            if (dynamicServiceLayer.VisibleLayers == null)
                dynamicServiceLayer.VisibleLayers = GetDefaultVisibleLayers(dynamicServiceLayer);
        }

        private int[] GetDefaultVisibleLayers(ESRI.ArcGIS.Client.ArcGISDynamicMapServiceLayer dynamicService)
        {
            List<int> visibleLayerIDList = new List<int>();

            ESRI.ArcGIS.Client.LayerInfo[] layerInfoArray = dynamicService.Layers;

            for (int index = 0; index < layerInfoArray.Length; index++)
            {
                _layerDictionary.Add(layerInfoArray[index].Name, layerInfoArray[index].ID);

                if (layerInfoArray[index].DefaultVisibility)
                    visibleLayerIDList.Add(index);
            }
            return visibleLayerIDList.ToArray();
        }
    }
}
