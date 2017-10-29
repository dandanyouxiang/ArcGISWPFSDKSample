using System;
using System.Net;
using System.Windows.Controls;
using ESRI.ArcGIS.Client.Geometry;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

namespace ArcGISWPFSDK
{
	/// <summary>
	/// Interaction logic for SOEElevationJSSerializer.xaml
	/// </summary>
	public partial class SOEElevationJSSerializer:UserControl
	{
		private static ESRI.ArcGIS.Client.Projection.WebMercator _mercator =
						new ESRI.ArcGIS.Client.Projection.WebMercator();

		JavaScriptSerializer serializer;

		public SOEElevationJSSerializer()
		{
			InitializeComponent();
			serializer = new JavaScriptSerializer();
		}
		private void MyMap_MouseClick(object sender,ESRI.ArcGIS.Client.Map.MouseEventArgs e)
		{
			MapPoint geographicPoint = _mercator.ToGeographic(e.MapPoint) as MapPoint;

			string SOEurl = "http://sampleserver4.arcgisonline.com/ArcGIS/rest/services/Elevation/ESRI_Elevation_World/MapServer/exts/ElevationsSOE/ElevationLayers/1/GetElevationAtLonLat";
			SOEurl += string.Format(System.Globalization.CultureInfo.InvariantCulture, "?lon={0}&lat={1}&f=json",geographicPoint.X,geographicPoint.Y);

			WebClient webClient = new WebClient();

			webClient.OpenReadCompleted += (s,a) =>
			{
				StreamReader reader = new StreamReader(a.Result);
			string responseString=reader.ReadToEnd();
			IDictionary<string,object> results =
			serializer.DeserializeObject(responseString) as IDictionary<string,object>;
			
			if (results != null && results.ContainsKey("elevation"))
			{
				double elevation = Math.Round(Convert.ToDouble(results["elevation"]),0);
				a.Result.Close();
				MyInfoWindow.Anchor = e.MapPoint;
				MyInfoWindow.Content = string.Format("Elevation: {0} meters",elevation.ToString());
				MyInfoWindow.IsOpen = true;
			}
			};

			webClient.OpenReadAsync(new Uri(SOEurl));
		}

	}
}
