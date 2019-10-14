using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;

/*
Exercise 4
----------

Galaxus or digitec, which shop is funnier? Let's find out!

*/

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
    private static readonly Stopwatch sw = new Stopwatch();

    public static void Main(string[] args)
    {
        if (args.Any(a => a == "--benchmark"))
        {
            var perfResult = RunBenchmark();
            Console.WriteLine($"Loading the data took on average {perfResult.perfDataLoading:0.00} ms");
            Console.WriteLine($"Processing the data took on average {perfResult.perfDataExamination:0.00} ms");
        }
        else
        {
            var data = LoadData();
            var counts = ExamineData(data);
            Console.WriteLine($"Galaxus ha count: {counts.hasGalaxus}");
            Console.WriteLine($"Digitec ha count: {counts.hasDigitec}");
        }
    }

    private static (double perfDataLoading, double perfDataExamination) RunBenchmark()
    {
        var loadingAvg = 0d;
        var examinationAvg = 0d;

        for (var i = 0; i < 20; i++)
        {
            var load = Time(LoadData);
            var examine = Time(() => ExamineData(load.result));
            loadingAvg += load.elapsedMs / 20;
            examinationAvg += examine.elapsedMs / 20;
        }

        return (perfDataLoading: loadingAvg, perfDataExamination: examinationAvg);
    }

    private static (double elapsedMs, T result) Time<T>(Func<T> func)
    {
        sw.Start();
        var result = func();
        sw.Stop();
        var elapsedMs = sw.ElapsedMilliseconds;
        sw.Reset();
        return (elapsedMs: elapsedMs, result: result);
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
        var galaxusHas = CountHas(data.dataGalaxus);
        var digitecHas = CountHas(data.dataDigitec);
        return new HaCounts(galaxusHas, digitecHas);
    }

    private static int CountHas(byte[] data)
    {
        var decoded = Encoding.UTF8.GetString(data);
        return decoded.ToLower().Replace(" ", "").Split("ha").Length - 1;
    }
}