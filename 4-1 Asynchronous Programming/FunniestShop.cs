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

Galaxus or digitec, which shop is funnier? Let's find out! I have written a little
tool for comparing the two shops. Unfortunately, somehow this little tool became critical
to the business and is now running 24/7 on a server. Thus, the requirements regarding its
performance have changed. You have now been tasked with improving the speed of the
application. To help you a bit, I have added a benchmark, which can be run by providing
the --benchmark flag. This should allow you to verify your progress along the way.

Use the appropriate techniques for optimizing I/O and CPU bound operatoins using async/await
and tasks.

HINT: Since C# 7.1 it is possible to declare the entry point, i.e. the Main(...) method, 
async.

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

    //(1) Improve this method by loading both pages asynchronously. For this, use 
    //DownloadDataTaskAsync(...) instead of DownloadData(...).
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

    //(2) Process the data for both pages in parallel.
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