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

    public PdfViewer(WebView2 webView)
    {
        this.webView = webView;
        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        await webView.EnsureCoreWebView2Async();
    }

    /// <summary>
    /// Updates the webViewer's source to the specified PDF file.
    /// </summary>
    /// <param name="pdfPath"></param>
    public void LoadPdf(string pdfPath)
    {
        if (webView?.CoreWebView2 == null)
        {
            Debug.WriteLine("WebView2 is not initialized");
            return;
        }
        
        webView.Source = new Uri(pdfPath);
    }
}