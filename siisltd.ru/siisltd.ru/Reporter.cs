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
        static async Task<Dictionary<DateTime, int>> CalculateDailyMaxSessionsAsync(string targetPath, bool isSkipHeader)
        {
            using (var reader = new StreamReader(targetPath))
            {
                Dictionary<DateTime, int> result = new();
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
                            try
                            {
                                result[today] = maxOverlap;
                            }
                            //TODO: если исходник не будет упорядочен по дате, то здесь может быть ошибка (да и много где). Стоит ли что-то делать, ещё не решил.
                            catch (Exception ex) {  }
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
        static async Task<Dictionary<string, Dictionary<SessionState, int>>> CalculateOperatorStatesAsync(string targetPath, bool isSkipHeader)
        {
            var operatorStates = new Dictionary<string, Dictionary<SessionState, int>>();
            using (var reader = new StreamReader(targetPath))
            {
                string? line;
                Session session;                
                var operatorName = "";
                if (isSkipHeader)
                {
                    await reader.ReadLineAsync();
                }
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    try
                    {
                        session = Session.CreateSession(line);
                        operatorName = session.OperatorName ?? "";
                    }
                    //TODO: куда то складировать инфу о необработанных строках. Какой нибудь ErrorReporter...
                    catch (Exception ex) { Console.WriteLine(ex.Message); continue; }
                    if (!operatorStates.ContainsKey(operatorName))
                    {
                        operatorStates[operatorName] = new Dictionary<SessionState, int> 
                        { 
                            { SessionState.Ready, 0 },
                            { SessionState.CallBack, 0 },
                            { SessionState.Pause, 0 },
                            { SessionState.Processing, 0 },
                            { SessionState.Talk, 0 },
                        };
                    }
                    operatorStates[operatorName][session.State] += session.Duration;
                }                
             }
            return operatorStates;
        }
    }
}
