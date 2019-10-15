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

    public static async Task Main(string[] args)
    {
        if (args.Any(a => a == "--benchmark"))
        {
            var perfResult = await RunBenchmark();
            Console.WriteLine($"Loading the data took on average {perfResult.perfDataLoading:0.00} ms");
            Console.WriteLine($"Processing the data took on average {perfResult.perfDataExamination:0.00} ms");
        }
        else
        {
            var data = await LoadData();
            var counts = await ExamineData(data);
            Console.WriteLine($"Galaxus ha count: {counts.hasGalaxus}");
            Console.WriteLine($"Digitec ha count: {counts.hasDigitec}");
        }
    }

    private static async Task<(double perfDataLoading, double perfDataExamination)> RunBenchmark()
    {
        var loadingAvg = 0d;
        var examinationAvg = 0d;

        for (var i = 0; i < 20; i++)
        {
            var load = await Time(LoadData);
            var examine = await Time(() => ExamineData(load.result));
            loadingAvg += load.elapsedMs / 20;
            examinationAvg += examine.elapsedMs / 20;
        }

        return (perfDataLoading: loadingAvg, perfDataExamination: examinationAvg);
    }

    private static async Task<(double elapsedMs, T result)> Time<T>(Func<Task<T>> func)
    {
        sw.Start();
        var result = await func();
        sw.Stop();
        var elapsedMs = sw.ElapsedMilliseconds;
        sw.Reset();
        return (elapsedMs: elapsedMs, result: result);
    }

    private static async Task<Data> LoadData()
    {
        using (var client1 = new WebClient())
        using (var client2 = new WebClient())
        {
            var taskGalaxus = client1.DownloadDataTaskAsync("https://www.galaxus.ch/");
            var taskDigitec = client2.DownloadDataTaskAsync("https://www.digitec.ch/");

            return new Data(dataGalaxus: await taskGalaxus, dataDigitec: await taskDigitec);
        }
    }

    private static async Task<HaCounts> ExamineData(Data data)
    {
        var galaxusTask = CountHas(data.dataGalaxus);
        var digitecTask = CountHas(data.dataDigitec);
        return new HaCounts(await galaxusTask, await digitecTask);
    }

    private static Task<int> CountHas(byte[] data)
    {
        return Task.Run(() =>
        {
            var decoded = Encoding.UTF8.GetString(data);
            return decoded.ToLower().Replace(" ", "").Split("ha").Length - 1;
        });
    }
}