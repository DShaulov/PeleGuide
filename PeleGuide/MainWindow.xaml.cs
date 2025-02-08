using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Window = System.Windows.Window;

namespace PeleGuide
{
    public partial class MainWindow : Window
    {
        private readonly WindowHandler windowHandler;
        private readonly PdfViewer pdfViewer;
        private readonly FolderScanner folderScanner;
        private readonly SearchHandler searchHandler;
        private readonly string mainFolderPath = @"C:\David\PDF-TEST";

        public MainWindow()
        {
            InitializeComponent();
            windowHandler = new WindowHandler(this);
            pdfViewer = new PdfViewer(webView);
            folderScanner = new FolderScanner();
            searchHandler = new SearchHandler(ResultsListView, SearchProgress, mainFolderPath, webView, pdfViewer);

            InitializeUI();
        }

        private void InitializeUI()
        {
            fileTreeView.ItemsSource = folderScanner.TreeItems;
            searchBox.KeyDown += async (s, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    await searchHandler.HandleSearch(searchBox.Text);
                }
            };

            folderScanner.ScanFolder(mainFolderPath);
        }

        /// <summary>
        /// Event handler for when a tree view item is selected.
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleFileSelection(object sender, MouseButtonEventArgs e)
        {
            var treeView = sender as TreeView;
            var item = treeView.SelectedItem;
            if (item is FileTreeItem fileItem)
            {
                if (fileItem.Type == "PDF")
                {
                    ShowWebView();
                    pdfViewer.LoadPdf(fileItem.FullPath);
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        public async void OnSearchBtnClick(object sender, RoutedEventArgs e)
        {
            await searchHandler.HandleSearch(searchBox.Text);
        }

        private void ShowWebView()
        {
            webView.Visibility = Visibility.Visible;
            ResultsListView.Visibility = Visibility.Collapsed;
        }

        private void OnTitleBarMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            windowHandler.HandleTitleBarMouseLeftButtonDown(sender, e);
        }

        private async void ResultsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ResultsListView.SelectedItem is SearchResult result)
            {
                await searchHandler.OpenPdfAtPage(result);
            }
        }
    }
}