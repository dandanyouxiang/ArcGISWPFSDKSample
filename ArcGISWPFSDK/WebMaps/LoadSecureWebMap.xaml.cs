using System.Windows.Controls;
using ESRI.ArcGIS.Client.WebMap;
using System.Net;
using System;
using System.Web.Script.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Windows;

namespace ArcGISWPFSDK
{
	/// <summary>
	/// Interaction logic for LoadSecureWebMap.xaml
	/// </summary>
	public partial class LoadSecureWebMap:UserControl
	{
		JavaScriptSerializer serializer;

		public LoadSecureWebMap()
		{
			InitializeComponent();
			serializer = new JavaScriptSerializer();

		}
		private void LoadWebMapButton_Click(object sender,System.Windows.RoutedEventArgs e)
		{
			WebClient webClient = new WebClient();

			string arcgisComTokenUrl = "https://www.arcgis.com/sharing/generateToken";
			arcgisComTokenUrl += string.Format("?username={0}&password={1}&expiration={2}&TickCount={3}&client=requestip&f=json",
					UsernameTextBox.Text,PasswordTextBox.Password,30,Environment.TickCount);

			webClient.OpenReadCompleted += (s,a) =>
			{
				Document webMap = new Document();

				StreamReader reader = new StreamReader(a.Result);
				string responseString = reader.ReadToEnd();
				IDictionary<string,object> results =
				serializer.DeserializeObject(responseString) as IDictionary<string,object>;

				a.Result.Close();
				string token =null;
				if (results.ContainsKey("token"))
					token = results["token"].ToString();
					
				a.Result.Close();

				webMap.Token = token;

				webMap.GetMapCompleted += webMap_GetMapCompleted;
				webMap.GetMapAsync(WebMapTextBox.Text);
			};

			webClient.OpenReadAsync(new Uri(arcgisComTokenUrl));
		}

		void webMap_GetMapCompleted(object sender,GetMapCompletedEventArgs e)
		{
			if (e.Error != null)
				MessageBox.Show(string.Format("Unable to load webmap. {0}",e.Error.Message));
			else
				LayoutRoot.Children.Insert(0,e.Map);
		}
	}
}
