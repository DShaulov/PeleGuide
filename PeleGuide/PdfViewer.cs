using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
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

        ConfigureWebViewSettings();
        ConfigureSecuritySettings();
        LoadDefaultViewer();
    }

    private void ConfigureWebViewSettings()
    {
        webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
        webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
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
        if (webView?.CoreWebView2 == null) return;

        string pdfUrl = new Uri(pdfPath).AbsoluteUri;
        string viewerUrl = $"{webView.Source.AbsoluteUri}?file={Uri.EscapeDataString(pdfUrl)}";
        webView.CoreWebView2.Navigate(viewerUrl);
    }

    public void Dispose()
    {
        if (webView?.CoreWebView2 != null)
        {
            webView.CoreWebView2.WebResourceRequested -= HandleWebResourceRequest;
        }
    }
}