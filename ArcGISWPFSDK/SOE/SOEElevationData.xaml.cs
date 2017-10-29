using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ESRI.ArcGIS.Client;
using System.Windows.Media.Imaging;
using ESRI.ArcGIS.Client.Symbols;
using System.Web.Script.Serialization;
using System.IO;


namespace ArcGISWPFSDK
{
	public partial class SOEElevationData:UserControl
	{
		Draw myDrawObject;
		WebClient webClient;
		GraphicsLayer graphicsLayer;
		List<Color> colorRanges = new List<Color>();
		JavaScriptSerializer serializer;

		public SOEElevationData()
		{
			InitializeComponent();

			serializer = new JavaScriptSerializer();

			myDrawObject = new Draw(MyMap)
			{
				DrawMode = DrawMode.Rectangle,
				FillSymbol = LayoutRoot.Resources["DrawFillSymbol"] as ESRI.ArcGIS.Client.Symbols.FillSymbol,
				IsEnabled = true
			};
			myDrawObject.DrawComplete += drawObject_DrawComplete;
			myDrawObject.DrawBegin += drawObject_OnDrawBegin;

			webClient = new WebClient();
			webClient.OpenReadCompleted += wc_OpenReadCompleted;

			graphicsLayer = MyMap.Layers["MyGraphicsLayer"] as GraphicsLayer;

			colorRanges.Add(Colors.Blue);
			colorRanges.Add(Colors.Green);
			colorRanges.Add(Colors.Yellow);
			colorRanges.Add(Colors.Orange);
			colorRanges.Add(Colors.Red);
		}

		private void drawObject_OnDrawBegin(object sender,EventArgs args)
		{
			graphicsLayer.ClearGraphics();
			ElevationView.Visibility = Visibility.Collapsed;
		}

		void drawObject_DrawComplete(object sender,DrawEventArgs e)
		{
			myDrawObject.IsEnabled = false;

			ESRI.ArcGIS.Client.Graphic graphic = new ESRI.ArcGIS.Client.Graphic()
			{
				Geometry = e.Geometry,
				Symbol = LayoutRoot.Resources["DrawFillSymbol"] as Symbol
			};
			graphicsLayer.Graphics.Add(graphic);

			ESRI.ArcGIS.Client.Geometry.Envelope aoiEnvelope = e.Geometry as ESRI.ArcGIS.Client.Geometry.Envelope;

			string SOEurl = "http://sampleserver4.arcgisonline.com/ArcGIS/rest/services/Elevation/ESRI_Elevation_World/MapServer/exts/ElevationsSOE/ElevationLayers/1/GetElevationData?";

			SOEurl += string.Format(System.Globalization.CultureInfo.InvariantCulture, "Extent={{\"xmin\" : {0}, \"ymin\" : {1}, \"xmax\" : {2}, \"ymax\" :{3},\"spatialReference\" : {{\"wkid\" : {4}}}}}&Rows={5}&Columns={6}&f=json",
					aoiEnvelope.XMin,aoiEnvelope.YMin,aoiEnvelope.XMax,aoiEnvelope.YMax,
					MyMap.SpatialReference.WKID,HeightTextBox.Text,WidthTextBox.Text);

			webClient.OpenReadAsync(new Uri(SOEurl));
		}

		void wc_OpenReadCompleted(object sender,OpenReadCompletedEventArgs e)
		{

			StreamReader reader = new StreamReader(e.Result);
			string responseString = reader.ReadToEnd();
			IDictionary<string,object> results =
			serializer.DeserializeObject(responseString) as IDictionary<string,object>;
			
			e.Result.Close();

			if (results.ContainsKey("error"))
			{
				MessageBox.Show("Error");//results["error"]["code"] + ": " + results["error"]["message"]
				myDrawObject.IsEnabled = true;
				return;
			}

			if(results.ContainsKey("data"))
			{
				IEnumerable<object> elevData = results["data"] as IEnumerable<object>;

			int thematicMin,thematicMax;
			thematicMin = thematicMax = (int)elevData.ElementAt(0);

			foreach (int elevValue in elevData)
			{
				if (elevValue < thematicMin) thematicMin = elevValue;
				if (elevValue > thematicMax) thematicMax = elevValue;
			}

			int totalRange = thematicMax - thematicMin;
			int portion = totalRange / 5;

			List<Color> cellColor = new List<Color>();

			foreach (int elevValue in elevData)
			{
				int startValue = thematicMin;
				for (int i = 0;i < 5;i++)
				{
					if (Enumerable.Range(startValue,portion).Contains(elevValue))
					{
						cellColor.Add(colorRanges[i]);
						break;
					}
					else if (i == 4)
						cellColor.Add(colorRanges.Last());

					startValue = startValue + portion;
				}
			}

			int rows = Convert.ToInt32(HeightTextBox.Text);
			int cols = Convert.ToInt32(WidthTextBox.Text);
			int stride = cols * 3 + (cols * 3) % 4;			

			int cell = 0;
			byte[] bits = new byte[rows * stride];
			for (int x = 0;x < rows;x++)
			{				
				for (int y = 0;y < cols;y++)
				{
					SetPixel(ref bits,x,y,stride,cellColor[cell]);
					cell++;
				}
			}

			BitmapSource bs = BitmapSource.Create(rows,cols,300,300,PixelFormats.Rgb24,null,bits,stride);
			ElevationView.Visibility = System.Windows.Visibility.Visible;
			ElevationImage.Source = bs;
			myDrawObject.IsEnabled = true;
			}
		}

		void SetPixel(ref byte[] bits,int x,int y,int stride,Color c)
		{
			bits[x * 3 + y * stride] = c.R;  
			bits[x*3 + y * stride + 1] = c.G;
			bits[x * 3 + y * stride + 2] = c.B;
		}

		private void CloseProfileImage_MouseLeftButtonDown(object sender,MouseButtonEventArgs e)
		{
			ElevationView.Visibility = Visibility.Collapsed;
		}

		private void SizeProfileImage_MouseLeftButtonDown(object sender,MouseButtonEventArgs e)
		{
			if (ElevationImage.Width == 150)
				ElevationImage.Width = ElevationImage.Height = 300;
			else
				ElevationImage.Width = ElevationImage.Height = 150;
		}
	}
}
