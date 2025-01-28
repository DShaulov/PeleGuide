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

        public MainWindow()
        {
            InitializeComponent();
            pdfViewer = new PdfViewer(webView);
            LoadPdfDocument("file.pdf");
        }

        private void LoadPdfDocument(string path)
        {
            // Get the application's base directory
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Construct path to the PDF
            string pdfPath = System.IO.Path.Combine(baseDirectory, "Resource", "Documents", "Test File.pdf");

            // Convert to URI format
            string pdfUrl = new Uri(pdfPath).AbsoluteUri;
            pdfViewer.LoadPdf(pdfPath);
        }

        protected override void OnClosed(EventArgs e)
        {
            pdfViewer.Dispose();
            base.OnClosed(e);
        }
    }

}