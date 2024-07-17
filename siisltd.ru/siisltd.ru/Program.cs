using ProgramNameSpace.Reports;
using ProgramNameSpace.Sessions;
namespace ProgramNameSpace
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: programm.exe <path-to-csv>");
                return;
            }
            if (!File.Exists(args[0]))
            {
                Console.WriteLine($"file {args[0]} not exists");
                return;
            }

            var targetPath = args[0];

            //////////////////////
            ///// Формируем отчёты
            #region
            var reportTask1 = Reporter.CalculateDailyMaxSessionsAsync(targetPath);
            var reportTask2 = Reporter.CalculateOperatorStatesAsync(targetPath);
            #endregion

            //////////////////////
            ///// Ждём отчёты
            #region
            await Task.WhenAll(reportTask1, reportTask2);

            var report1 = reportTask1.Result;
            var report2 = reportTask2.Result;
            #endregion

            //////////////////////
            ///// Печатаем отчёты
            #region
            Reporter.PrintDailyMaxReport(report1);
            Console.WriteLine();
            Reporter.PrintOperatorStatesReport(report2);
            Console.WriteLine();
            Reporter.PrintOperatorStatesReportBeautiful(report2);
            #endregion
        }
    }
}