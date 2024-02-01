using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace PdfSplitterConsoleApp
{
    class Program
    {
        static void Main()
        {
            // Set input and output directory
            string inputDirectory = @"C:\Users\inputDirectory";
            string outputDirectory = @"C:\Users\outputDirectory";

            // Create the output directory if it doesn't exist
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            //Set output filename
            string outputFileName = Path.Combine(outputDirectory, $"createdOutputPDF.pdf");

            // Move PdfWriter and PdfDocument outside the loop
            using (PdfWriter writer = new PdfWriter(outputFileName))
            {
                using (PdfDocument newPdf = new PdfDocument(writer))
                {
                    // Process each PDF file in the input directory and its subdirectories
                    foreach (var inputFile in Directory.EnumerateFiles(inputDirectory, "*.pdf", SearchOption.AllDirectories))
                    {
                        ProcessPdf(inputFile, newPdf);
                    }
                }
            }

            Console.WriteLine("Processing complete.");
        }

        static void ProcessPdf(string inputFile, PdfDocument newPdf)
        {
            using (PdfReader reader = new PdfReader(inputFile))
            {
                PdfDocument pdfDocument = new PdfDocument(reader);
                int numberOfPages = pdfDocument.GetNumberOfPages();

                if (numberOfPages > 0)
                {
                    try
                    {
                        // Process each page using iTextSharp
                        for (int pageNum = 1; pageNum <= numberOfPages; pageNum++)
                        {
                            var strategy = new LocationTextExtractionStrategy();
                            string text = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(pageNum), strategy);

                            // Check if the page contains the desired text
                            if (text.Contains("Desired text") || text.Contains("Other desired text"))
                            {
                                // Found the desired text on this page
                                Console.WriteLine($"Found desired text on page {pageNum}");

                                // Copy the current page to the new PDF
                                PdfPage page = pdfDocument.GetPage(pageNum).CopyTo(newPdf);
                                newPdf.AddPage(page);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the exception details
                        Console.WriteLine($"Error processing PDF '{inputFile}': {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"The PDF file '{inputFile}' has no pages.");
                }
            }
        }
    }
}
