using System.Windows;
using System.Windows.Controls;

namespace ArcGISWPFSDK
{
    public partial class Bookmarks : UserControl
    {
        public Bookmarks()
        {
            InitializeComponent();
        }

        private void MyBookmarks_Loaded(object sender, RoutedEventArgs e)
        {
            //MyBookmarks.ClearBookmarks(); // Remove all bookmarks
            //MyBookmarks.AddBookmark("Mix - Las Vegas", new ESRI.ArcGIS.Client.Geometry.Envelope(-115.212, 36.083, -115.111, 36.157));
            //MyBookmarks.AddBookmark("DevSummit - Palm Springs", new ESRI.ArcGIS.Client.Geometry.Envelope(-116.55, 33.816, -113.525, 33.834));
            //MyBookmarks.AddBookmark("User Conference - San Diego", new ESRI.ArcGIS.Client.Geometry.Envelope(-117.211, 32.665, -117.11, 32.739));
        }
    }
}
