using System;
using System.Windows;
using System.Windows.Controls;
using ESRI.ArcGIS.Client.WebMap;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Net;
using System.Diagnostics;

namespace ArcGISWPFSDK
{
	public partial class LoadWebMapWithBing:UserControl
	{
		public LoadWebMapWithBing()
		{
			InitializeComponent();
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


					BingKeyGrid.Visibility = System.Windows.Visibility.Collapsed;
					InvalidBingKeyTextBlock.Visibility = System.Windows.Visibility.Collapsed;

					if (authenticationResult == "ValidCredentials")
					{
						Document webMap = new Document();
						webMap.BingToken = BingKeyTextBox.Text;
						webMap.GetMapCompleted += (s1,e1) =>
						{
							if (e1.Error == null)
								LayoutRoot.Children.Add(e1.Map);
						};

						webMap.GetMapAsync("75e222ec54b244a5b73aeef40ce76adc");
					}
					else InvalidBingKeyTextBlock.Visibility = System.Windows.Visibility.Visible;
				}
				else InvalidBingKeyTextBlock.Visibility = System.Windows.Visibility.Visible;
			};

			webClient.OpenReadAsync(new System.Uri(uri));
		}

		private void Button_Click(object sender,RoutedEventArgs e)
		{
			Process.Start("IExplore.exe","https://www.bingmapsportal.com");
		}

		[DataContract]
		public class BingAuthentication
		{
			[DataMember(Name = "authenticationResultCode")]
			public string AuthenticationResultCode { get; set; }
		}
	}
}