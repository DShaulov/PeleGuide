using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

public class PdfViewer
{
    private readonly WebView2 webView;
    private string pdfFilesDir;
    private string pdfJsDir;
    private string virtualPdfFilesUrl;

    public PdfViewer(WebView2 webView)
    {
        this.webView = webView;
        pdfFilesDir = @"C:\David\PDF-TEST\";
        pdfJsDir = AppDomain.CurrentDomain.BaseDirectory;
        virtualPdfFilesUrl = "http://pdf-files/";
        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        await webView.EnsureCoreWebView2Async();
 

        // Map the directory which pdfjs
        webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
            "pdf-viewer",
            pdfJsDir,
            CoreWebView2HostResourceAccessKind.Allow);

        Debug.WriteLine($"Mapped virtual host 'pdf-viewer' to: {pdfJsDir}");

        // Map the directory which pdfjs
        webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
            "pdf-files",
            pdfFilesDir,
            CoreWebView2HostResourceAccessKind.Allow);

        Debug.WriteLine($"Mapped virtual host 'pdf-files' to: {pdfFilesDir}");

        LoadDefaultViewer();
        TestPdfViewer();
    }

    private void ChangeBaseDirectory(string newBaseDir)
    {
        pdfFilesDir = newBaseDir;
        webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
            "app",
            pdfFilesDir,
            CoreWebView2HostResourceAccessKind.Allow);
        Debug.WriteLine($"Mapped virtual host 'app' to: {pdfFilesDir}");
    }

    /// <summary>
    /// Sets up the PDF.js viewer interface by loading the viewer.html file from pdfjs.
    /// </summary>
    private void LoadDefaultViewer()
    {
        string pdfViewerPath = Path.Combine(pdfJsDir, "pdfjs", "web", "viewer.html");
        string fileUrl = new Uri(pdfViewerPath).AbsoluteUri;
        webView.Source = new Uri(fileUrl);
    }

    /// <summary>
    /// Updates the viewer.html to display the specified PDF file.
    /// </summary>
    /// <param name="pdfPath"></param>
    public void LoadPdf(string pdfPath)
    {
        if (webView?.CoreWebView2 == null)
        {
            Debug.WriteLine("WebView2 is not initialized");
            return;
        }

        // Uses the virtual host 'app' to access the PDF file
        string virtualPdfUrl = ConvertToVirtualUrl(pdfPath);
        string viewerUri = $"http://pdf-viewer/pdfjs/web/viewer.html?file={virtualPdfUrl}";

        Debug.WriteLine($"Loading PDF from: {pdfPath}");
        Debug.WriteLine($"Virtual PDF URL: {virtualPdfUrl}");
        Debug.WriteLine($"Final URI: {viewerUri}");
        string script = $@"PDFViewerApplication.open({{
            url: 'https://local.pdfs/Test.pdf'
        }}).then(() => {{
            console.log('PDF loaded successfully');
        }});";
        webView.CoreWebView2.Navigate(viewerUri);
    }

    public string ConvertToVirtualUrl(string pdfPath)
    {
        return pdfPath.Replace($"{pdfFilesDir}", $"{virtualPdfFilesUrl}").Replace("\\", "/");
    }
    public void TestPdfViewer()
    {
        try
        {
            LoadPdf(@"C:\David\PDF-TEST\Resources\Documents\Test.pdf");
            Debug.WriteLine("Navigation initiated successfully");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading PDF: {ex.Message}");
        }
    }
}