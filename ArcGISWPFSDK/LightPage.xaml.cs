using AurelienRibon.Ui.SyntaxHighlightBox;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;

namespace ArcGISWPFSDK
{
    public partial class LightPage : Window
    {
        private ScaleTransform _scale = new ScaleTransform();
        List<Category> _categoryList;
        string _xmlFile;
        CategoryItem _item;
        UserControl _control = null;
        string _target = null;
        ListBoxItem _targetListBoxItem;

        public LightPage()
        {
            InitializeComponent();
            this.txtSrc.CurrentHighlighter = HighlighterManager.Instance.Highlighters["CSharp"];
        }

        public LightPage(string xmlFile)
            : this()
        {
            _xmlFile = xmlFile;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            OpenFile("/ArcGISWPFSDK;component/runtime/assets/sdkconfig.xml");
        }
        
        void OpenFile(string xmlFile)
        {            
            Uri uri = new Uri(xmlFile, UriKind.Relative);
            System.Windows.Resources.StreamResourceInfo sri = 
                Application.GetResourceStream(uri);
            Stream fs = sri.Stream;
            System.Xml.XmlReader xr = System.Xml.XmlReader.Create(fs);
          
            XDocument doc = XDocument.Load(xr);
            _categoryList = (
                from f in doc.Root.Elements("Category")
                select new Category
                {
                    Name = (string)f.Element("name"),
                    Icon = (string)f.Element("icon"),
                    CategoryItems = (
                        from o in f.Elements("items").Elements("item")
                        select new CategoryItem
                        {
                            ID = (string)o.Element("id"),
                            XAML = (string)o.Element("xaml"),
                            Source = (string)o.Element("source"),
                            Code = (string)o.Element("code"),
                            Icon = (string)o.Element("icon")
                        })
                        .ToArray()
                })
                .ToList();

            CreateList();

            ProcessTarget();
        }

        private void CreateList()
        {
            int listCount = ListOfSamples.Children.Count;
            foreach (Category category in _categoryList)
            {
                Grid g = new Grid()
                {
                    Margin = new Thickness(0, 5, 0, 0)
                };
                Rectangle rect = new Rectangle()
                {                    
                    Stroke = new SolidColorBrush(Color.FromArgb(255, 70, 86, 109)),
                    Margin = new Thickness(0),
                    RadiusX = 5,
                    RadiusY = 5,
                };

                rect.Fill = Resources["ExpanderGradient"] as Brush;
                g.Children.Add(rect);
                Expander exp = new Expander()
                {
                    IsExpanded = false,
                    Name = String.Format("Category_{0}", listCount),
                    Foreground = new SolidColorBrush(Colors.Black),
                    Background = new SolidColorBrush(Colors.Transparent),
                    FontWeight = FontWeights.Bold,
                    FontSize = 11,
                    Header = category.Name,
                    Tag = listCount,
                    Cursor = Cursors.Hand,
                    Margin = new Thickness(4)
                };
                exp.Expanded += new RoutedEventHandler(exp_Expanded);
                ListBox lb = new ListBox()
                {
                    Name = String.Format("List_{0}", listCount),
                    Background = new SolidColorBrush(Colors.Transparent),
                    BorderBrush = new SolidColorBrush(Colors.Transparent),
                    Foreground = new SolidColorBrush(Colors.Black),
                    Margin = new Thickness(5, 0, 5, 5),
                    FontWeight = FontWeights.Normal
                    
                };
                lb.ItemContainerStyle = Resources["SDKListBoxtItemStyle"] as Style;

                int itemCount = 0;
                foreach (CategoryItem ci in category.CategoryItems)
                {
                    StackPanel sp1 = new StackPanel();
                    sp1.Orientation = Orientation.Horizontal;
                    Image img = new Image();
                    img.Source = new BitmapImage(new Uri(ci.Icon, UriKind.RelativeOrAbsolute));
                    sp1.Children.Add(img);
                    TextBlock tb1 = new TextBlock()
                    {
                        FontSize = 11,
                        Text = ci.ID
                    };
                    sp1.Children.Add(tb1);
                    ListBoxItem item = new ListBoxItem()
                    {
                        Content = sp1,
                        Background = new SolidColorBrush(Colors.Transparent),
                        BorderBrush = new SolidColorBrush(Colors.Transparent),
                        Name = String.Format("Item_{0}_{1}", listCount, itemCount),
                        Tag = ci,
                        Cursor = Cursors.Hand
                    };
                    lb.Items.Add(item);
                    itemCount++;
                }
                lb.SelectionChanged += lb_SelectionChanged;
                exp.Content = lb;
                listCount++;
                g.Children.Add(exp);
                ListOfSamples.Children.Add(g);

                Expander expww = FindName("Category_0") as Expander;
            }
        }

        void exp_Expanded(object sender, RoutedEventArgs e)
        {
            Expander exp1 = sender as Expander;
            Expander exp;
            int listCount = ListOfSamples.Children.Count;
            for (int i = 0; i < listCount; i++)
            {
                exp = FindByName(String.Format("Category_{0}", i), this) as Expander;
                if (exp.Name != exp1.Name) exp.IsExpanded = false;
            }
            int listIndex = Convert.ToInt32(exp1.Tag);
            string listName = String.Format("List_{0}", listIndex);
            ListBox lb = FindByName(listName, this) as ListBox;
            if (lb != null) lb.SelectedIndex = -1;
            //SampleCaption.Text = String.Format("{0}", Convert.ToString(exp1.Header));
        }


        //WPF find framework element
        private FrameworkElement FindByName(string name, FrameworkElement root)
        {
            Stack<FrameworkElement> tree = new Stack<FrameworkElement>();
            tree.Push(root);
            while (tree.Count > 0)
            {
                FrameworkElement current = tree.Pop();
                if (current.Name == name)
                    return current;
                int count = VisualTreeHelper.GetChildrenCount(current);
                for (int i = 0; i < count; ++i)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(current, i);
                    if (child is FrameworkElement)
                        tree.Push((FrameworkElement)child);
                }
            }
            return null;
        }

        void pline_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Polygon pline = sender as Polygon;
            string index = Convert.ToString(pline.Tag);
            string name = _categoryList[Convert.ToInt32(index)].Name;
            //ToggleCategoryVisibility(index, name);
        }

        void lb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_targetListBoxItem != null)
            {
                _targetListBoxItem.IsSelected = false;
                _targetListBoxItem = null;
            }

            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                ListBoxItem item = e.AddedItems[0] as ListBoxItem;
                if (item != null)
                {
                    CategoryItem ci = item.Tag as CategoryItem;
                    _item = ci;
                    processitem(ci);
                }
            }
        }

        private void processitem(CategoryItem item)
        {
            _control = null;
            tabSample.Content = null;
            //tabXamlScrollView.Content = null;
            txtSrc.Text = "";
            txtXaml.xmlDocument = null;
            //tabSrcScrollView.Content = null;
            tabPanel.SelectedIndex = 0;

            Type t = Type.GetType(item.XAML);
            if (t != null)
            {
                _control = System.Activator.CreateInstance(t) as UserControl;
                tabSample.Content = _control;
                SampleCaption.Text = item.ID;
            }
        }

        private void sideBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double height = sideBar.ActualHeight - 10;
            double width = sideBar.ActualWidth - 10;
            //treeSamples.Width = width > 0 ? width : 1;
            //treeSamples.Height = height > 0 ? height : 1;
        }

        private void sourceViewer(string srcFile)
        {
            //    WebClient client = new WebClient();
            //    client.OpenReadCompleted += new OpenReadCompletedEventHandler(sourceView_OpenReadCompleted);
            //    client.OpenReadAsync(new Uri(srcFile, UriKind.RelativeOrAbsolute));
            //}

            //void sourceView_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
            //{
            //    if (e.Error != null)
            //        return;

            //    string src = null;
            //    using (Stream s = e.Result)
            //    {
            //        StreamReader sr = new StreamReader(s);
            //        src = sr.ReadToEnd();
            //    }

            StreamReader sr = new StreamReader(srcFile);
            string src = sr.ReadToEnd();
            XmlDocument xdoc = null;
            if (tabPanel.SelectedIndex == 1)
            {
                xdoc = new XmlDocument();
                xdoc.LoadXml(src);
            }
            switch (tabPanel.SelectedIndex)
            {
                case 1:
                    if (txtXaml.xmlDocument == null)
                    {
                        txtXaml.xmlDocument = xdoc;
                    }
                    break;
                case 2:
                    txtSrc.Text = src;
                    //force redraw
                    txtSrc.InvalidateVisual();
                    break;
                case 3:
                    txtXaml2.Text = src;
                    break;
            }
            //tabSample.Content = null;
        }

        private void tabPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabPanel == null) return;
            if (_item != null)
            {
                switch (tabPanel.SelectedIndex)
                {
                    case 1:
                        sourceViewer(_item.Source);
                        //copyToClipboard.Visibility = Visibility.Visible;
                        break;
                    case 2:
                        sourceViewer(_item.Code);
                        //copyToClipboard.Visibility = Visibility.Visible;
                        break;
                    case 3:
                        sourceViewer(_item.Source);
                        break;
                    default:
                        
                        // WPF: Added to keep application from freezing
                        if (tabSample.Content is UserControl)
                            tabSample.Focus();
                        
                        //copyToClipboard.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }

        public void copyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            string text = String.Empty;
            switch (tabPanel.SelectedIndex)
            {
                case 1:
                    if (txtXaml == null || txtXaml.xmlDocument == null)
                        return;
                    text = txtXaml.xmlDocument.OuterXml;
                    break;
                case 2:
                    if (txtSrc == null)
                        return;
                    if (string.IsNullOrEmpty(txtSrc.SelectedText))
                        txtSrc.SelectAll();
                    text = txtSrc.SelectedText;
                    break;
            }

            Clipboard.SetText(text);
        }

        private void InitializeMouseWheel()
        {
            //HtmlPage.Window.AttachEvent("DOMMouseScroll", this.OnMouseWheel); // Mozilla
            //HtmlPage.Window.AttachEvent("onmousewheel", this.OnMouseWheel);
            //HtmlPage.Document.AttachEvent("onmousewheel", this.OnMouseWheel); // IE
        }

        //protected void OnMouseWheel(object sender, EventArgs e)
        //{
        //    double mouseDelta = 0;
        //    ScriptObject eventObject = e.EventObject;

        //    // Mozilla and Safari
        //    if (eventObject.GetProperty("detail") != null)
        //        mouseDelta = ((double)eventObject.GetProperty("detail")) * (-1);

        //    // IE and Opera
        //    else if (eventObject.GetProperty("wheelDelta") != null)
        //        mouseDelta = ((double)eventObject.GetProperty("wheelDelta"));

        //    mouseDelta = Math.Sign(mouseDelta);

        //    if (mouseDelta > 0)
        //    {
        //        if (tabPanel.SelectedIndex == 1)
        //            tabXamlScrollView.ScrollToVerticalOffset(tabXamlScrollView.VerticalOffset - 50);
        //        if (tabPanel.SelectedIndex == 2)
        //            tabSrcScrollView.ScrollToVerticalOffset(tabSrcScrollView.VerticalOffset - 50);
        //    }

        //    if (mouseDelta < 0)
        //    {
        //        if (tabPanel.SelectedIndex == 1)
        //            tabXamlScrollView.ScrollToVerticalOffset(tabXamlScrollView.VerticalOffset + 50);
        //        if (tabPanel.SelectedIndex == 2)
        //            tabSrcScrollView.ScrollToVerticalOffset(tabSrcScrollView.VerticalOffset + 50);

        //    }
        //}

        //public void SetTargetSample(string hash)
        //{
        //    string targetValue = hash.Trim().TrimStart('#');
        //    if (targetValue.Length > 0 && targetValue.Length < 30)
        //        _target = targetValue;
        //}

        public void ProcessTarget()
        {
            if (string.IsNullOrEmpty(_target))
                return;

            string targetXAML = "ArcGISWPFSDK." + _target;

            int categoryIndex = 0;
            foreach (Category category in _categoryList)
            {
                int categoryItemIndex = 0;
                foreach (CategoryItem categoryItem in category.CategoryItems)
                {
                    if (categoryItem.XAML == targetXAML)
                    {
                        _item = categoryItem;
                        processitem(categoryItem);

                        Expander exp = FindName(String.Format("Category_{0}", categoryIndex)) as Expander;
                        exp.IsExpanded = true;

                        _targetListBoxItem = FindName(String.Format("Item_{0}_{1}", categoryIndex, categoryItemIndex)) as ListBoxItem;
                        _targetListBoxItem.IsSelected = true;

                        return;
                    }
                    categoryItemIndex++;
                }
                categoryIndex++;
            }
        }

        private void TopText_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            Assembly assembly = typeof(ESRI.ArcGIS.Client.Map).Assembly;
            AssemblyName assemblyName = new AssemblyName(assembly.FullName);
            string version = assemblyName.Version.ToString();
            MessageBox.Show(assemblyName.Version.ToString(), "ArcGIS WPF Assembly Version", MessageBoxButton.OK);
        }
    }
}
