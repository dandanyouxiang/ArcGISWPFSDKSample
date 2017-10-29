using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Tasks;
using ESRI.ArcGIS.Client.ValueConverters;
using System.ComponentModel;

namespace ArcGISWPFSDK
{
	public partial class SpatialQuery:UserControl
	{
		private Draw MyDrawSurface;

		public SpatialQuery()
		{
			InitializeComponent();

			MyDrawSurface = new Draw(MyMap)
			{
				LineSymbol = LayoutRoot.Resources["DefaultLineSymbol"] as SimpleLineSymbol,
				FillSymbol = LayoutRoot.Resources["DefaultFillSymbol"] as FillSymbol
			};
			MyDrawSurface.DrawComplete += MyDrawSurface_DrawComplete;
		}

		private void esriTools_ToolbarItemClicked(object sender,ESRI.ArcGIS.Client.Toolkit.SelectedToolbarItemArgs e)
		{
			switch (e.Index)
			{
				case 0: // Point
					MyDrawSurface.DrawMode = DrawMode.Point;
					break;
				case 1: // Polyline
					MyDrawSurface.DrawMode = DrawMode.Polyline;
					break;
				case 2: // Polygon
					MyDrawSurface.DrawMode = DrawMode.Polygon;
					break;
				case 3: // Rectangle
					MyDrawSurface.DrawMode = DrawMode.Rectangle;
					break;
				default: // Clear
					MyDrawSurface.DrawMode = DrawMode.None;
					GraphicsLayer selectionGraphicslayer = MyMap.Layers["MySelectionGraphicsLayer"] as GraphicsLayer;
					selectionGraphicslayer.ClearGraphics();
					QueryDetailsDataGrid.ItemsSource = null;
					ResultsDisplay.Visibility = Visibility.Collapsed;
					break;
			}
			MyDrawSurface.IsEnabled = (MyDrawSurface.DrawMode != DrawMode.None);
			StatusTextBlock.Text = e.Item.Text;
		}

		private void MyDrawSurface_DrawComplete(object sender,ESRI.ArcGIS.Client.DrawEventArgs args)
		{
			GraphicsLayer selectionGraphicslayer = MyMap.Layers["MySelectionGraphicsLayer"] as GraphicsLayer;
			selectionGraphicslayer.ClearGraphics();

			QueryTask queryTask = new QueryTask("http://sampleserver1.arcgisonline.com/ArcGIS/rest/services/Demographics/ESRI_Census_USA/MapServer/5");
			queryTask.ExecuteCompleted += QueryTask_ExecuteCompleted;
			queryTask.Failed += QueryTask_Failed;

			// Bind data grid to query results
			Binding resultFeaturesBinding = new Binding("LastResult.Features");
			resultFeaturesBinding.Source = queryTask;
			QueryDetailsDataGrid.SetBinding(DataGrid.ItemsSourceProperty,resultFeaturesBinding);
			Query query = new ESRI.ArcGIS.Client.Tasks.Query();

			// Specify fields to return from query
			query.OutFields.AddRange(new string[] { "STATE_NAME","SUB_REGION","STATE_FIPS","STATE_ABBR","POP2000","POP2007" });
			query.Geometry = args.Geometry;

			// Return geometry with result features
			query.ReturnGeometry = true;
			query.OutSpatialReference = MyMap.SpatialReference;

			queryTask.ExecuteAsync(query);
		}

		private void QueryTask_ExecuteCompleted(object sender,ESRI.ArcGIS.Client.Tasks.QueryEventArgs args)
		{
			FeatureSet featureSet = args.FeatureSet;

			if (featureSet == null || featureSet.Features.Count < 1)
			{
				MessageBox.Show("No features returned from query");
				return;
			}

			GraphicsLayer graphicsLayer = MyMap.Layers["MySelectionGraphicsLayer"] as GraphicsLayer;

			if (featureSet != null && featureSet.Features.Count > 0)
			{
				foreach (Graphic feature in featureSet.Features)
				{
					feature.Symbol = LayoutRoot.Resources["ResultsFillSymbol"] as FillSymbol;
					graphicsLayer.Graphics.Insert(0,feature);
				}
				ResultsDisplay.Visibility = Visibility.Visible;
			}
			MyDrawSurface.IsEnabled = false;
		}

		private void QueryTask_Failed(object sender,TaskFailedEventArgs args)
		{
			MessageBox.Show("Query failed: " + args.Error);
		}

		private void GraphicsLayer_MouseEnter(object sender,GraphicMouseEventArgs args)
		{
			QueryDetailsDataGrid.Focus();
			QueryDetailsDataGrid.SelectedItem = args.Graphic;
			QueryDetailsDataGrid.CurrentColumn = QueryDetailsDataGrid.Columns[0];
			QueryDetailsDataGrid.ScrollIntoView(QueryDetailsDataGrid.SelectedItem,QueryDetailsDataGrid.Columns[0]);
		}

		private void GraphicsLayer_MouseLeave(object sender,GraphicMouseEventArgs args)
		{
			QueryDetailsDataGrid.Focus();
			QueryDetailsDataGrid.SelectedItem = null;
		}

		private void QueryDetailsDataGrid_SelectionChanged(object sender,SelectionChangedEventArgs e)
		{
			foreach (Graphic g in e.AddedItems)
				g.Select();

			foreach (Graphic g in e.RemovedItems)
				g.UnSelect();
		}

		private void QueryDetailsDataGrid_LoadingRow(object sender,DataGridRowEventArgs e)
		{
			e.Row.MouseEnter += Row_MouseEnter;
			e.Row.MouseLeave += Row_MouseLeave;
		}

		void Row_MouseEnter(object sender,MouseEventArgs e)
		{
			(((System.Windows.FrameworkElement)(sender)).DataContext as Graphic).Select();
		}

		void Row_MouseLeave(object sender,MouseEventArgs e)
		{
			DataGridRow row = sender as DataGridRow;
			Graphic g = ((System.Windows.FrameworkElement)(sender)).DataContext as Graphic;

			if (!QueryDetailsDataGrid.SelectedItems.Contains(g))
				g.UnSelect();
		}
	}
}
