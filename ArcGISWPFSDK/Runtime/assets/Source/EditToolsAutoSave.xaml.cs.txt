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
  public partial class EditToolsAutoSave : UserControl
  {
    public EditToolsAutoSave()
    {
      InitializeComponent();
    }

    private void MyMap_Loaded(object sender, RoutedEventArgs e)
    {
      ESRI.ArcGIS.Client.Editor myEditor = LayoutRoot.Resources["MyEditor"] as ESRI.ArcGIS.Client.Editor;
      myEditor.Map = MyMap;      
    }
  }
}
