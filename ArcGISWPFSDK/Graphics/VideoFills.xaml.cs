using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Tasks;
using ESRI.ArcGIS.Client.Symbols;
using System.Windows.Shapes;

namespace ArcGISWPFSDK
{
	public partial class VideoFills:UserControl
	{
		List<Graphic> _lastActiveGraphics;

		public VideoFills()
		{
			InitializeComponent();
			_lastActiveGraphics = new List<Graphic>();

			MyMap.Layers.LayersInitialized += Layers_LayersInitialized;
		}

		void Layers_LayersInitialized(object sender,EventArgs args)
		{
			ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query()
			{
				OutSpatialReference = MyMap.SpatialReference,
				ReturnGeometry = true
			};
			query.OutFields.Add("STATE_NAME");
			query.Where = "STATE_NAME IN ('Alaska', 'Hawaii', 'Washington', 'Oregon', 'Arizona', 'Nevada', 'Idaho', 'Montana', 'Utah', 'Wyoming', 'Colorado', 'New Mexico')";

			QueryTask myQueryTask = new QueryTask("http://sampleserver1.arcgisonline.com/ArcGIS/rest/services/Demographics/ESRI_Census_USA/MapServer/5");
			myQueryTask.ExecuteCompleted += myQueryTask_ExecuteCompleted;
			myQueryTask.ExecuteAsync(query);
		}


		void myQueryTask_ExecuteCompleted(object sender,QueryEventArgs queryArgs)
		{
			if (queryArgs.FeatureSet == null)
				return;

			FeatureSet resultFeatureSet = queryArgs.FeatureSet;
			ESRI.ArcGIS.Client.GraphicsLayer graphicsLayer =
					MyMap.Layers["MyGraphicsLayer"] as ESRI.ArcGIS.Client.GraphicsLayer;

			if (resultFeatureSet != null && resultFeatureSet.Features.Count > 0)
			{
				foreach (ESRI.ArcGIS.Client.Graphic graphicFeature in resultFeatureSet.Features)
				{
					//graphicFeature.Symbol = LayoutRoot.Resources["TransparentFillSymbol"] as Symbol;
					graphicFeature.Symbol = LayoutRoot.Resources["videoFill"] as Symbol;
					string stateName = Convert.ToString(graphicFeature.Attributes["STATE_NAME"]);
					graphicFeature.Attributes.Add("VideoSource",String.Format("http://serverapps.esri.com/media/{0}_small.wmv",stateName));
					graphicsLayer.Graphics.Add(graphicFeature);
				}
			}
		}
		
		private void ClearVideoSymbol(Graphic graphic)
		{
			Grid videoGrid = FindName("MediaGrid") as Grid;
			if (videoGrid.Children != null && videoGrid.Children.Count > 0)
			{
				MediaElement m = videoGrid.Children[0] as MediaElement;
				if (m != null)
				{
					m.MediaEnded -= State_Media_MediaEnded;
					m.UnloadedBehavior = MediaState.Manual;
					m.Stop();
				}
				videoGrid.Children.Clear();
			}

			graphic.Symbol = LayoutRoot.Resources["TransparentFillSymbol"] as Symbol;
			_lastActiveGraphics.Remove(graphic);
		}

		private void State_Media_MediaEnded(object sender,RoutedEventArgs e)
		{
			// Repeat play of the video
			MediaElement media = sender as MediaElement;
			media.Position = TimeSpan.FromSeconds(0);
			media.LoadedBehavior = MediaState.Manual;
			media.Play();
		}

		private void Element_MouseEnter(object sender,MouseEventArgs e)
		{
			Path path = sender as Path;
			((path.Fill as VisualBrush).Visual as MediaElement).Play();

		}

		private void Element_MouseLeave(object sender,MouseEventArgs e)
		{
			Path path = sender as Path;
			((path.Fill as VisualBrush).Visual as MediaElement).Pause();
		}
	}
}

