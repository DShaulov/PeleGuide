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
    private string lastPdfPathOpened;

    public PdfViewer(WebView2 webView)
    {
        this.webView = webView;
        InitializeAsync();
        lastPdfPathOpened = string.Empty;
    }

    private async void InitializeAsync()
    {
        await webView.EnsureCoreWebView2Async();
    }

    /// <summary>
    /// Updates the webViewer's source to the specified PDF file.
    /// </summary>
    /// <param name="pdfPath"></param>
    public async void  LoadPdf(string pdfPath, int pageNum = 1)
    {
        if (webView?.CoreWebView2 == null)
        {
            Debug.WriteLine("WebView2 is not initialized");
            return;
        }

        var baseUri = new Uri(pdfPath).AbsoluteUri;
        var uri = new Uri(baseUri + $"#page={pageNum}");
        webView.Source = uri;

        // Handle edge case when trying to jump to a page of a previously loaded PDF - the browser caches the previous page and does not jump.
        if (lastPdfPathOpened == pdfPath)
        {
            webView.CoreWebView2.Navigate("about:blank");
            await Task.Delay(100); // Small delay to ensure blank page loads
            webView.CoreWebView2.Navigate(uri.ToString());
        }
        lastPdfPathOpened = pdfPath;
    }
}