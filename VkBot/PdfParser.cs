using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Reflection.PortableExecutable;

public static class PdfTextExtractor1
{
    public static string pdfText(string path)
    {
        using (PdfReader sr = new PdfReader(path))
        {
            string text = string.Empty;
            for (int page = 1; page <= sr.NumberOfPages; page++)
            {
                text += PdfTextExtractor.GetTextFromPage(sr, page);
            }
            return text;
        }

    }
}