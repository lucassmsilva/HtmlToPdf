using PuppeteerSharp;
using PuppeteerSharp.Media;

public class ChromePdfGenerator
{
    public async Task GeneratePdfAsync(string htmlFilePath, string pdfFilePath, PdfOptions pdfOptions = null)
    {
        if (!File.Exists(htmlFilePath))
        {
            throw new FileNotFoundException("HTML file not found.", htmlFilePath);
        }

        // Set default PDF options if none provided
        pdfOptions ??= new PdfOptions
        {
            Format = PaperFormat.A4,
            PrintBackground = true,
            MarginOptions = new MarginOptions
            {
                Top = "10mm",
                Bottom = "10mm",
                Left = "10mm",
                Right = "10mm"
            }
        };

        // Download Chrome browser if not already installed
        await new BrowserFetcher().DownloadAsync();

        // Launch browser
        var launchOptions = new LaunchOptions
        {
            Headless = true,
            Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
        };

        using var browser = await Puppeteer.LaunchAsync(launchOptions);
        using var page = await browser.NewPageAsync();

        // Navigate to the HTML file
        await page.GoToAsync($"file://{htmlFilePath}", new NavigationOptions
        {
            WaitUntil = new[] { WaitUntilNavigation.Networkidle0 }
        });

        // Generate PDF
        await page.PdfAsync(pdfFilePath, pdfOptions);

        await browser.CloseAsync();
    }
}

// Example usage
class Program
{
    static async Task Main()
    {
        try
        {
            string htmlFilePath = @"C:\Users\lucas.silva\Downloads\doc.html";
            string pdfFilePath = @"D:\Docs\my_custom_name.pdf";

            var pdfGenerator = new ChromePdfGenerator();

            // Optional: Customize PDF options
            var customOptions = new PdfOptions
            {
                Format = PaperFormat.A4,
                PrintBackground = true,
                MarginOptions = new MarginOptions
                {
                    Left = "30mm",
                    Top = "30mm",
                    Right = "20mm",
                    Bottom = "20mm",
                }
            };

            await pdfGenerator.GeneratePdfAsync(htmlFilePath, pdfFilePath, customOptions);
            Console.WriteLine("PDF generated successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating PDF: {ex.Message}");
        }
    }
}