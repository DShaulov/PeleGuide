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
    using System.Windows;

    public partial class MainWindow : Window
    {
        private readonly PdfViewer pdfViewer;
        private readonly FolderScanner folderScanner;


        public MainWindow()
        {
            InitializeComponent();
            pdfViewer = new PdfViewer(webView);
            folderScanner = new FolderScanner();
            fileTreeView.ItemsSource = folderScanner.TreeItems;

            folderScanner.ScanFolder(@"C:\David\PDF-TEST");
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
                    pdfViewer.LoadPdf(fileItem.FullPath);
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }
    }

}