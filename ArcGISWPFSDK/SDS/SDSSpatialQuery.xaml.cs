using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Tasks;
using System.Windows.Input;
using ESRI.ArcGIS.Client.Symbols;
using System;
using System.ComponentModel;


namespace ArcGISWPFSDK
{
  public partial class SDSSpatialQuery : UserControl
  {
    private Draw _drawSurface;
    QueryTask _queryTask;

    public SDSSpatialQuery()
    {
      InitializeComponent();

      _drawSurface = new Draw(MyMap)
      {
        LineSymbol = LayoutRoot.Resources["DefaultLineSymbol"] as LineSymbol,
        FillSymbol = LayoutRoot.Resources["DefaultFillSymbol"] as FillSymbol
      };
      _drawSurface.DrawComplete += MyDrawSurface_DrawComplete;

      _queryTask = new QueryTask("http://serverapps.esri.com/SDS/databases/Demo/dbo.USStates_Geographic");
      _queryTask.ExecuteCompleted += QueryTask_ExecuteCompleted;
      _queryTask.Failed += QueryTask_Failed;
    }

    private void esriTools_ToolbarItemClicked(object sender, ESRI.ArcGIS.Client.Toolkit.SelectedToolbarItemArgs e)
    {
      switch (e.Index)
      {
        case 0: // Point
          _drawSurface.DrawMode = DrawMode.Point;
          break;
        case 1: // Polyline
          _drawSurface.DrawMode = DrawMode.Polyline;
          break;
        case 2: // Polygon
          _drawSurface.DrawMode = DrawMode.Polygon;
          break;
        case 3: // Rectangle
          _drawSurface.DrawMode = DrawMode.Rectangle;
          break;
        default: // Clear
          _drawSurface.DrawMode = DrawMode.None;
          GraphicsLayer selectionGraphicslayer = MyMap.Layers["MySelectionGraphicsLayer"] as GraphicsLayer;
          selectionGraphicslayer.ClearGraphics();
          QueryDetailsDataGrid.ItemsSource = null;
          ResultsDisplay.Visibility = Visibility.Collapsed;
          break;
      }
      _drawSurface.IsEnabled = (_drawSurface.DrawMode != DrawMode.None);
      StatusTextBlock.Text = e.Item.Text;
    }

    private void MyDrawSurface_DrawComplete(object sender, ESRI.ArcGIS.Client.DrawEventArgs args)
    {
      GraphicsLayer selectionGraphicslayer = MyMap.Layers["MySelectionGraphicsLayer"] as GraphicsLayer;
      selectionGraphicslayer.ClearGraphics();

      // Bind data grid to query results
      Binding resultFeaturesBinding = new Binding("LastResult.Features");
      resultFeaturesBinding.Source = _queryTask;
      QueryDetailsDataGrid.SetBinding(DataGrid.ItemsSourceProperty, resultFeaturesBinding);

      Query query = new ESRI.ArcGIS.Client.Tasks.Query();
      query.OutFields.AddRange(new string[] { "STATE_NAME", "POP2008", "SUB_REGION" });
      query.OutSpatialReference = MyMap.SpatialReference;
      query.Geometry = args.Geometry;
      query.SpatialRelationship = SpatialRelationship.esriSpatialRelIntersects;
      query.ReturnGeometry = true;

      _queryTask.ExecuteAsync(query);
    }

    private void QueryTask_ExecuteCompleted(object sender, ESRI.ArcGIS.Client.Tasks.QueryEventArgs args)
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
          feature.Symbol = LayoutRoot.Resources["ResultsFillSymbol"] as Symbol;
          graphicsLayer.Graphics.Insert(0, feature);
        }
      }

      ResultsDisplay.Visibility = Visibility.Visible;

      _drawSurface.IsEnabled = false;
    }

    private void QueryTask_Failed(object sender, TaskFailedEventArgs args)
    {
      MessageBox.Show("Query failed: " + args.Error);
    }

   public class MySorter : System.Collections.IComparer
    {
      public MySorter(ListSortDirection direction, DataGridColumn column)
      {
        Direction = direction;
        Column = column;
      }

      public ListSortDirection Direction
      {
        get;
        private set;
      }

      public DataGridColumn Column
      {
        get;
        private set;
      }

      public int Compare(object a, object b)
      {
        string str1 = ((Graphic)a).Attributes[Column.SortMemberPath].ToString();
        string str2 = ((Graphic)b).Attributes[Column.SortMemberPath].ToString();

        double db1 = -1;
        double db2 = -1;

        bool isNumber = Double.TryParse(str1, out db1);
        if (isNumber)
          isNumber = Double.TryParse(str2, out db2);

        // compare the numbers
        if (isNumber)
        {
          if (Direction == ListSortDirection.Ascending)
            return db1.CompareTo(db2);
          return db2.CompareTo(db1);
        }
        // compare the strings
        else
        {
          if (Direction == ListSortDirection.Ascending)
            return str1.CompareTo(str2);
          return str2.CompareTo(str1);
        }
      }
    }

    private void GraphicsLayer_MouseEnter(object sender, GraphicMouseEventArgs args)
    {
      QueryDetailsDataGrid.Focus();
      QueryDetailsDataGrid.SelectedItem = args.Graphic;
      QueryDetailsDataGrid.CurrentColumn = QueryDetailsDataGrid.Columns[0];
      QueryDetailsDataGrid.ScrollIntoView(QueryDetailsDataGrid.SelectedItem, QueryDetailsDataGrid.Columns[0]);
    }

    private void GraphicsLayer_MouseLeave(object sender, GraphicMouseEventArgs args)
    {
      QueryDetailsDataGrid.Focus();
      QueryDetailsDataGrid.SelectedItem = null;
    }

    private void QueryDetailsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      foreach (Graphic g in e.AddedItems)
        g.Select();

      foreach (Graphic g in e.RemovedItems)
        g.UnSelect();
    }

    private void QueryDetailsDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
    {
      e.Row.MouseEnter += Row_MouseEnter;
      e.Row.MouseLeave += Row_MouseLeave;
    }

    void Row_MouseEnter(object sender, MouseEventArgs e)
    {
      (((System.Windows.FrameworkElement)(sender)).DataContext as Graphic).Select();
    }

    void Row_MouseLeave(object sender, MouseEventArgs e)
    {
      DataGridRow row = sender as DataGridRow;
      Graphic g = ((System.Windows.FrameworkElement)(sender)).DataContext as Graphic;

      if (!QueryDetailsDataGrid.SelectedItems.Contains(g))
        g.UnSelect();
    }



    private void QueryDetailsDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
    {
      DataGrid myDataGrid = sender as DataGrid;
      e.Handled = true;

      DataGridColumn column = e.Column;
      ListSortDirection direction = (column.SortDirection !=
          ListSortDirection.Ascending) ?
          ListSortDirection.Ascending :
          ListSortDirection.Descending;

      column.SortDirection = direction; ListCollectionView listCollectionView =
          (ListCollectionView)CollectionViewSource.GetDefaultView(myDataGrid.ItemsSource);

      MySorter mySorter = new MySorter(direction, column);
      listCollectionView.CustomSort = mySorter;
    }
  }
}
