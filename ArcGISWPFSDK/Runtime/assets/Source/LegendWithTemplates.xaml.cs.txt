using System.Windows.Controls;
using ESRI.ArcGIS.Client.Toolkit;
using ESRI.ArcGIS.Client.Toolkit.Primitives;

namespace ArcGISWPFSDK
{
    public partial class LegendWithTemplates : UserControl
    {
        public LegendWithTemplates()
        {
            InitializeComponent();
        }

        private void Legend_Refreshed(object sender, Legend.RefreshedEventArgs e)
        {
            LayerItemViewModel removeLayerItemVM = null;

            if (e.LayerItem.LayerItems != null)
            {
                foreach (LayerItemViewModel layerItemVM in e.LayerItem.LayerItems)
                {
                    if (layerItemVM.IsExpanded)
                        layerItemVM.IsExpanded = false;

                    if (layerItemVM.Label == "states")
                        removeLayerItemVM = layerItemVM;
                }

                if (removeLayerItemVM != null)
                    e.LayerItem.LayerItems.Remove(removeLayerItemVM);
            }
            else
            {
                e.LayerItem.IsExpanded = false;
            }
        }
    }
}
