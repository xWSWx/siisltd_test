using ProgramNameSpace.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace siisltd.ru
{
    public static class Reporter
    {
        static async Task<List<Tuple<DateTime, int>>> CalculateDailyMaxSessionsAsync(string targetPath, bool isSkipHeader)
        {
            using (var reader = new StreamReader(targetPath))
            {
                List<Tuple<DateTime, int>> result = new();
                string? line;
                
                Session session;
                int maxOverlap = 0;
                int currentOverlap = 0;
                DateTime currentFinish = DateTime.MinValue;
                DateTime start;
                DateTime finish;
                DateTime tomorrow = DateTime.MinValue.AddDays(1).Date;
                if (isSkipHeader)
                {
                    await reader.ReadLineAsync();
                }
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    try
                    {
                        session = Session.CreateSession(line);
                    }
                    //TODO: куда то складировать инфу о необработанных строках. Какой нибудь ErrorReporter...
                    catch(Exception ex) { Console.WriteLine(ex.Message); continue;  }
                    start = session.StartTime.Date;
                    finish = session.FinishTime.Date;

                    if (start > currentFinish)
                    {
                        if (start >= tomorrow && currentFinish != DateTime.MinValue)
                        {
                            var today = tomorrow.AddHours(-1).Date;
                            result.Add(new(today, maxOverlap));
                            tomorrow = start.AddDays(1).Date;
                        }

                        currentOverlap = 1;
                        currentFinish = finish;
                    }
                    else
                    {
                        currentOverlap++;
                        currentFinish = currentFinish > finish ? currentFinish : finish;
                    }
                }
                return result;
            }
        }

    }
}
