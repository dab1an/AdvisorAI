using System.Text;

namespace server.Tests.TestHelpers;

internal static class PdfTestHelper
{
    public static MemoryStream CreateSinglePagePdf(params string[] lines)
    {
        string contentStream = BuildContentStream(lines);

        string[] objects =
        {
            "1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj\n",
            "2 0 obj\n<< /Type /Pages /Kids [3 0 R] /Count 1 >>\nendobj\n",
            "3 0 obj\n<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Resources << /Font << /F1 4 0 R >> >> /Contents 5 0 R >>\nendobj\n",
            "4 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>\nendobj\n",
            $"5 0 obj\n<< /Length {Encoding.ASCII.GetByteCount(contentStream)} >>\nstream\n{contentStream}\nendstream\nendobj\n"
        };

        List<byte[]> objectBytes = objects.Select(obj => Encoding.ASCII.GetBytes(obj)).ToList();
        List<int> offsets = new List<int> { 0 };

        int currentOffset = Encoding.ASCII.GetByteCount("%PDF-1.4\n");
        foreach (byte[] objectBytesItem in objectBytes)
        {
            offsets.Add(currentOffset);
            currentOffset += objectBytesItem.Length;
        }

        StringBuilder xref = new StringBuilder();
        xref.AppendLine("xref");
        xref.AppendLine("0 6");
        xref.AppendLine("0000000000 65535 f ");

        for (int i = 1; i < offsets.Count; i++)
        {
            xref.AppendLine($"{offsets[i]:0000000000} 00000 n ");
        }

        byte[] xrefBytes = Encoding.ASCII.GetBytes(xref.ToString());
        int startXref = currentOffset;

        StringBuilder trailer = new StringBuilder();
        trailer.AppendLine("trailer");
        trailer.AppendLine("<< /Size 6 /Root 1 0 R >>");
        trailer.AppendLine("startxref");
        trailer.AppendLine(startXref.ToString());
        trailer.AppendLine("%%EOF");

        byte[] trailerBytes = Encoding.ASCII.GetBytes(trailer.ToString());

        using MemoryStream stream = new MemoryStream();
        WriteAscii(stream, "%PDF-1.4\n");

        foreach (byte[] objectBytesItem in objectBytes)
            stream.Write(objectBytesItem, 0, objectBytesItem.Length);

        stream.Write(xrefBytes, 0, xrefBytes.Length);
        stream.Write(trailerBytes, 0, trailerBytes.Length);
        stream.Position = 0;

        return new MemoryStream(stream.ToArray());
    }

    private static string BuildContentStream(IEnumerable<string> lines)
    {
        List<string> encodedLines = lines.Select(EscapePdfText).ToList();

        StringBuilder builder = new StringBuilder();
        builder.AppendLine("BT");
        builder.AppendLine("/F1 12 Tf");
        builder.AppendLine("14 TL");
        builder.AppendLine("72 720 Td");

        for (int i = 0; i < encodedLines.Count; i++)
        {
            builder.AppendLine($"({encodedLines[i]}) Tj");

            if (i < encodedLines.Count - 1)
                builder.AppendLine("T*");
        }

        builder.AppendLine("ET");
        return builder.ToString();
    }

    private static string EscapePdfText(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace("(", "\\(")
            .Replace(")", "\\)");
    }

    private static void WriteAscii(Stream stream, string text)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(text);
        stream.Write(bytes, 0, bytes.Length);
    }
}