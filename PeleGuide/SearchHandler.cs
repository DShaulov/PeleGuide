using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Web.WebView2.Core;

namespace PeleGuide
{
    public class SearchHandler
    {
        private readonly ListView resultsListView;
        private readonly ProgressBar searchProgress;
        private readonly string folderPath;
        private readonly WebView2 webView;
        private readonly PdfViewer pdfViewer;

        public SearchHandler(ListView resultsListView, ProgressBar searchProgress, string folderPath, WebView2 webView, PdfViewer pdfViewer)
        {
            this.resultsListView = resultsListView;
            this.searchProgress = searchProgress;
            this.folderPath = folderPath;
            this.webView = webView;
            this.pdfViewer = pdfViewer;

        }

        public async Task HandleSearch(string searchText)
        {
            searchProgress.Visibility = Visibility.Visible;

            var progress = new Progress<double>(value => searchProgress.Value = value);
            var results = await PdfSearch.SearchPdfsInFolder(folderPath, searchText, progress);
            resultsListView.ItemsSource = results;
            webView.Visibility = Visibility.Collapsed;
            resultsListView.Visibility = Visibility.Visible;

            searchProgress.Visibility = Visibility.Collapsed;
            searchProgress.Value = 0;
        }

        public async Task OpenPdfAtPage(SearchResult result)
        {
            try
            {
                // Show the WebView and hide the results
                webView.Visibility = Visibility.Visible;
                resultsListView.Visibility = Visibility.Collapsed;

                // Load the PDF if it's not already loaded
                pdfViewer.LoadPdf(result.FilePath);

                // Wait for the viewer to be ready
                // await WaitForViewerReady();

                // Navigate to the specific page
                string pageScript = $@"
                const viewer = document.getElementById('pdfViewer');
                if (viewer) {{
                    viewer.setPageNumber({result.PageNumber});
                }}";
                await webView.CoreWebView2.ExecuteScriptAsync(pageScript);

                // Highlight the search term
                string searchScript = $@"
                const viewer = document.getElementById('pdfViewer');
                if (viewer) {{
                    viewer.postMessage({{
                        type: 'find',
                        query: '{result.MatchedText.Replace("'", "\\'")}',
                        highlightAll: true,
                        caseSensitive: false,
                        findPrevious: false
                    }});
                }}";
                await webView.CoreWebView2.ExecuteScriptAsync(searchScript);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening PDF: {ex.Message}");
            }
        }

        //private Task WaitForViewerReady()
        //{
        //    var tcs = new TaskCompletionSource<bool>();

        //    if (isViewerReady)
        //    {
        //        tcs.SetResult(true);
        //    }
        //    else
        //    {
        //        EventHandler<CoreWebView2NavigationCompletedEventArgs> handler = null;
        //        handler = (s, e) =>
        //        {
        //            webView.CoreWebView2.NavigationCompleted -= handler;
        //            isViewerReady = true;
        //            tcs.SetResult(true);
        //        };
        //        webView.CoreWebView2.NavigationCompleted += handler;
        //    }

        //    return tcs.Task;
        //}
    }

}
