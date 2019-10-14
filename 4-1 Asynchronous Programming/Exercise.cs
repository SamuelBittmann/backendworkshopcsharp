using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net;
using System.Text;

#region Internal
internal class Data
{
    public readonly byte[] dataGalaxus;
    public readonly byte[] dataDigitec;

    public Data(byte[] dataGalaxus, byte[] dataDigitec)
    {
        this.dataGalaxus = dataGalaxus;
        this.dataDigitec = dataDigitec;
    }
}

internal class HaCounts
{
    public readonly int hasGalaxus;
    public readonly int hasDigitec;

    public HaCounts(int hasGalaxus, int hasDigitec)
    {
        this.hasGalaxus = hasGalaxus;
        this.hasDigitec = hasDigitec;
    }
}
#endregion

public static class Program
{
    public static void Main(string[] args)
    {
        var sw = new Stopwatch();
        sw.Start();
        var data = LoadData();
        var counts = ExamineData(data);

        sw.Stop();
        Console.WriteLine($"Reading the files took {sw.Elapsed.TotalMilliseconds} ms.");
        Console.WriteLine($"Galaxus ha count: {counts.hasGalaxus}");
        Console.WriteLine($"Digitec ha count: {counts.hasDigitec}");
    }

    private static Data LoadData()
    {
        using (var client1 = new WebClient())
        using (var client2 = new WebClient())
        {
            var dataGalaxus = client1.DownloadData("https://www.galaxus.ch/");
            var dataDigitec = client2.DownloadData("https://www.digitec.ch/");

            return new Data(dataGalaxus, dataDigitec);
        }
    }

    private static HaCounts ExamineData(Data data)
    {
        var dataGalaxus = Encoding.UTF8.GetString(data.dataGalaxus);
        var galaxusHaCount = dataGalaxus.ToLower().Replace(" ", "").Split("ha").Length - 1;
        var dataDigitec = Encoding.UTF8.GetString(data.dataDigitec);
        var digitecHaCount = dataDigitec.ToLower().Replace(" ", "").Split("ha").Length - 1;

        return new HaCounts(galaxusHaCount, digitecHaCount);
    }

    private static string ReadFile(string fileName)
    {
        return File.ReadAllText($".{Path.PathSeparator}{fileName}");
    }
}