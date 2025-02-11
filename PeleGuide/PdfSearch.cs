using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace PeleGuide
{
    public class PdfSearch
    {
        public static async Task<List<SearchResult>> SearchPdfsInFolder(string folderPath, string searchTerm, IProgress<double> progress = null)
        {
            var results = new List<SearchResult>();
            var pdfFiles = Directory.GetFiles(folderPath, "*.pdf", SearchOption.AllDirectories);
            var totalFiles = pdfFiles.Length;
            var processedFiles = 0;

            foreach (var pdfFile in pdfFiles)
            {
                try
                {
                    var fileResults = await SearchPdfFile(pdfFile, searchTerm);
                    results.AddRange(fileResults);

                    processedFiles++;
                    progress?.Report((double)processedFiles / totalFiles * 100);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing file {pdfFile}: {ex.Message}");
                }
            }
            return results;
        }

        private static bool ContainsHebrew(string text)
        {
            return text.Any(c => c >= 0x0590 && c <= 0x05FF);
        }

        private static string ReverseHebrewText(string text)
        {
            return new string(text.Reverse().ToArray());
        }

        private static async Task<List<SearchResult>> SearchPdfFile(string filePath, string searchTerm)
        {
            var results = new List<SearchResult>();

            await Task.Run(() =>
            {
                using (var pdfReader = new PdfReader(filePath))
                using (var pdfDocument = new PdfDocument(pdfReader))
                {
                    for (int pageNum = 1; pageNum <= pdfDocument.GetNumberOfPages(); pageNum++)
                    {
                        var page = pdfDocument.GetPage(pageNum);
                        var strategy = new SimpleTextExtractionStrategy();
                        var pageText = PdfTextExtractor.GetTextFromPage(page, strategy);
                        // Normalize Hebrew text direction
                        pageText = string.Join("\n", pageText.Split('\n').Select(line =>
                            ContainsHebrew(line) ? ReverseHebrewText(line) : line));

                        if (pageText.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            var searchTermPos = 0;
                            while ((searchTermPos = pageText.IndexOf(searchTerm, searchTermPos, StringComparison.OrdinalIgnoreCase)) != -1)
                            {
                                var contextStart = Math.Max(0, searchTermPos - 100);
                                var contextLength = Math.Min(200, pageText.Length - contextStart);
                                var context = pageText.Substring(contextStart, contextLength);
                                if (contextStart > 0)
                                {
                                    context = "...\n" + context;
                                }
                                if (contextStart + contextLength < pageText.Length)
                                {
                                    context += "\n...";
                                }
                                results.Add(new SearchResult
                                {
                                    FilePath = filePath,
                                    PageNumber = pageNum,
                                    MatchedText = searchTerm,
                                    Context = context
                                });

                                searchTermPos += searchTerm.Length;
                            }
                        }
                    }
                }
            });

            return results;
        }
    }
}