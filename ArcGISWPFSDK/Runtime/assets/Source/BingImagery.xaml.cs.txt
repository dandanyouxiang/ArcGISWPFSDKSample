using System.Windows;
using System.Windows.Controls;
using ESRI.ArcGIS.Client.Bing;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Diagnostics;

namespace ArcGISWPFSDK
{
	public partial class BingImagery:UserControl
	{
		public BingImagery()
		{
			InitializeComponent();
		}

		private void RadioButton_Click(object sender,RoutedEventArgs e)
		{
			ESRI.ArcGIS.Client.Bing.TileLayer tileLayer = MyMap.Layers["BingLayer"] as TileLayer;
			string layerTypeTag = (string)((RadioButton)sender).Tag;
			TileLayer.LayerType newLayerType = (TileLayer.LayerType)System.Enum.Parse(typeof(TileLayer.LayerType),layerTypeTag,true);
			tileLayer.LayerStyle = newLayerType;
		}

		private void BingKeyTextBox_TextChanged(object sender,TextChangedEventArgs e)
		{
			if ((sender as TextBox).Text.Length >= 64)
				LoadMapButton.IsEnabled = true;
			else
				LoadMapButton.IsEnabled = false;
		}

		private void LoadMapButton_Click(object sender,RoutedEventArgs e)
		{
			WebClient webClient = new WebClient();
			string uri = string.Format("http://dev.virtualearth.net/REST/v1/Imagery/Metadata/Aerial?supressStatus=true&key={0}",BingKeyTextBox.Text);

			webClient.OpenReadCompleted += (s,a) =>
			{
				if (a.Error == null)
				{
					DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(BingAuthentication));
					BingAuthentication bingAuthentication = serializer.ReadObject(a.Result) as BingAuthentication;
					a.Result.Close();
					string authenticationResult = bingAuthentication.AuthenticationResultCode.ToString();

					if (authenticationResult == "ValidCredentials")
					{
						ESRI.ArcGIS.Client.Bing.TileLayer tileLayer = new TileLayer()
						{
							ID = "BingLayer",
							LayerStyle = TileLayer.LayerType.Road,
							ServerType = ServerType.Production,
							Token = BingKeyTextBox.Text
						};

						MyMap.Layers.Add(tileLayer);

						BingKeyGrid.Visibility = System.Windows.Visibility.Collapsed;
						LayerStyleGrid.Visibility = System.Windows.Visibility.Visible;

						InvalidBingKeyTextBlock.Visibility = System.Windows.Visibility.Collapsed;

					}
					else InvalidBingKeyTextBlock.Visibility = System.Windows.Visibility.Visible;
				}
				else InvalidBingKeyTextBlock.Visibility = System.Windows.Visibility.Visible;
			};

			webClient.OpenReadAsync(new System.Uri(uri));
		}

		[DataContract]
		public class BingAuthentication
		{
			[DataMember(Name = "authenticationResultCode")]
			public string AuthenticationResultCode { get; set; }
		}

		private void Button_Click(object sender,RoutedEventArgs e)
		{
			Process.Start("IExplore.exe","https://www.bingmapsportal.com");
		}
	}
}
