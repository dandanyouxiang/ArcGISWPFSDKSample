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
using ESRI.ArcGIS.Client;

namespace ArcGISWPFSDK
{

    // TODO: Sample does not function.  API needs to accommodate workflow.  

    public partial class EditToolsSelectionOnly : UserControl
    {

        public EditToolsSelectionOnly()
        {
            InitializeComponent();

            (MyMap.Layers["Threat Areas"] as FeatureLayer).Graphics.CollectionChanged += Graphics_CollectionChanged;
        }

        void Graphics_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (object item in e.NewItems)
                        graphicAdded(item as Graphic);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (object item in e.OldItems)
                        graphicRemoved(item as Graphic);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    if (e.OldItems != null)
                    {
                        foreach (object item in e.OldItems)
                            graphicRemoved(item as Graphic);
                    }
                    if (e.NewItems != null)
                    {
                        foreach (object item in e.NewItems)
                            graphicAdded(item as Graphic);
                    }
                    break;
            }
        }

        void graphicAdded(Graphic g)
        {
            if (g != null)
            {
                g.PropertyChanged += g_PropertyChanged;
            }
        }

        void graphicRemoved(Graphic g)
        {
            if (g != null)
            {
                g.PropertyChanged -= g_PropertyChanged;
            }
        }

        void g_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Geometry")
            {
                
            }
        }

        private void FeatureLayer_EndSaveEdits(object sender, ESRI.ArcGIS.Client.Tasks.EndEditEventArgs e)
        {
            (MyMap.Layers["HomelandSecurityMapServiceLayer"] as
                ESRI.ArcGIS.Client.ArcGISDynamicMapServiceLayer).Refresh();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            FeatureLayer featureLayer = MyMap.Layers["Threat Areas"] as FeatureLayer;
            featureLayer.SaveEdits();
        }
    }
}
