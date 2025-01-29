using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;

public class PdfViewer
{
    private readonly WebView2 webView;

    public PdfViewer(WebView2 webView)
    {
        this.webView = webView;
        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        await webView.EnsureCoreWebView2Async();

        // Map the application base directory which contains both pdfjs and Resources folder
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
            "app",
            baseDir,
            CoreWebView2HostResourceAccessKind.Allow);

        Debug.WriteLine($"Mapped virtual host 'app' to: {baseDir}");

        ConfigureWebViewSettings();
        ConfigureSecuritySettings();
        LoadDefaultViewer();
        TestPdfViewer();

    }

    private void ConfigureWebViewSettings()
    {
        webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
        webView.CoreWebView2.Settings.AreDevToolsEnabled = true;
        webView.CoreWebView2.Settings.IsZoomControlEnabled = true;
        webView.CoreWebView2.Settings.IsWebMessageEnabled = true;
        webView.CoreWebView2.Settings.AreHostObjectsAllowed = true;
    }

    private void ConfigureSecuritySettings()
    {
        webView.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
        webView.CoreWebView2.WebResourceRequested += HandleWebResourceRequest;
    }

    private void HandleWebResourceRequest(object sender, CoreWebView2WebResourceRequestedEventArgs e)
    {
        if (e.Request.Uri.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
        {
            e.Request.Headers.SetHeader("Cross-Origin-Opener-Policy", "same-origin");
            e.Request.Headers.SetHeader("Cross-Origin-Embedder-Policy", "require-corp");
        }
    }

    /// <summary>
    /// Sets up the PDF.js viewer interface by loading the viewer.html file from pdfjs.
    /// </summary>
    private void LoadDefaultViewer()
    {
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string pdfViewerPath = Path.Combine(baseDirectory, "pdfjs", "web", "viewer.html");
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

        // Use the path relative to your application directory
        string virtualPdfUrl = "http://app/Resources/Documents/Test.pdf";
        string viewerUri = $"http://app/pdfjs/web/viewer.html?file={virtualPdfUrl}";

        Debug.WriteLine($"Loading PDF from: {pdfPath}");
        Debug.WriteLine($"Virtual PDF URL: {virtualPdfUrl}");
        Debug.WriteLine($"Final URI: {viewerUri}");

        webView.CoreWebView2.Navigate(viewerUri);
    }

    public void TestPdfViewer()
    {
        string testPdfPath = @"C:\David\PDF-TEST\Test.pdf";  // Make sure you have a PDF file with this name

        // Log each step to understand what's happening
        Debug.WriteLine($"Testing PDF at path: {testPdfPath}");
        Debug.WriteLine($"File exists: {File.Exists(testPdfPath)}");

        try
        {
            LoadPdf(testPdfPath);
            Debug.WriteLine("Navigation initiated successfully");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading PDF: {ex.Message}");
        }
    }

    public void Dispose()
    {
        if (webView?.CoreWebView2 != null)
        {
            webView.CoreWebView2.WebResourceRequested -= HandleWebResourceRequest;
        }
    }
}