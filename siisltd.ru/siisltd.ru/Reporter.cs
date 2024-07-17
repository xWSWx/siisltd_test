using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProgramNameSpace.Sessions;
namespace ProgramNameSpace.Reports
{
    public static class Reporter
    {

        //////////////////////
        ///// Report Creator
        #region
        /// <summary>
        /// Формирует отчёт с максимальным кол-вом одновременных звонок за день.
        /// </summary>
        /// <param name="targetPath"></param>
        /// <param name="isSkipHeader"></param>
        /// <returns></returns>
        public static async Task<Dictionary<DateTime, int>> CalculateDailyMaxSessionsAsync(string targetPath, bool isSkipHeader = true)
        {
            Dictionary<DateTime, int> result = new();
            if (!File.Exists(targetPath)) 
            {
                return result;
            }
            using (var fs = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(fs))
            {
                
                string? line;

                Session session;
                int currentOverlap = 0;
                DateTime currentFinish = DateTime.MinValue;
                DateTime start;
                DateTime finish;
                DateTime tomorrow = DateTime.MinValue.AddDays(1).Date;
                DateTime today = DateTime.MinValue;
                if (isSkipHeader)
                {
                    await reader.ReadLineAsync();
                }
                bool isRecordsExists = false;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    //Пока что кастыльная защита от "нет записей"
                    isRecordsExists = true;
                    try
                    {
                        session = Session.CreateSession(line);
                    }
                    //TODO: куда то складировать инфу о необработанных строках. Какой нибудь ErrorReporter...
                    catch (Exception ex) { Console.WriteLine(ex.Message); continue; }
                    start = session.StartTime.Date;
                    finish = session.FinishTime.Date;

                    if (start > currentFinish)
                    {
                        /////////////////////////////////////////////////////////
                        //// Переход на следующий_день провоцирует строку отчёта
                        if (start >= tomorrow)
                        {
                            today = start.Date;
                            tomorrow = today.AddDays(1).Date;
                            try
                            {
                                //TODO: так себе кастыль на крайнее значение (первый вход)
                                if (currentFinish != DateTime.MinValue)
                                {
                                    var yesterday = today.AddDays(-1).Date;
                                    result[yesterday] = currentOverlap;
                                }
                            }
                            //TODO: если исходник не будет упорядочен по дате (диверсия), то код будет выдавать экзепшен на вставке в словарь. Стоит ли что-то делать, ещё не решил.
                            catch (Exception ex) { }
                        }
                        /////////////////////////////////////////////////////////
                        //// Сбрасываем счётчик если был простой
                        currentOverlap = 1;
                        currentFinish = finish;
                    }
                    else
                    {
                        currentOverlap++;
                        currentFinish = currentFinish > finish ? currentFinish : finish;
                    }
                }
                /////////////////////
                //// Крайний день конец записей     
                try
                {
                    if(isRecordsExists)
                        result[today] = currentOverlap;
                }
                catch (Exception ex) { }
            }
            return result;
        }
        /// <summary>
        /// Формирует отчёт с временем каждого состояния для оператора
        /// </summary>
        /// <param name="targetPath"></param>
        /// <param name="isSkipHeader"></param>
        /// <returns></returns>
        public static async Task<Dictionary<string, Dictionary<SessionState, int>>> CalculateOperatorStatesAsync(string targetPath, bool isSkipHeader = true)
        {
            var operatorStates = new Dictionary<string, Dictionary<SessionState, int>>();
            if (!File.Exists(targetPath))
            {
                return operatorStates;
            }
            using (var fs = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(fs))
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
        #endregion

        //////////////////////
        ///// Report Printer
        #region
        static readonly string dailyMaxReportHeader = "День\t\tКоличество сессий";
        static readonly string operatorStatesReportHeader = "ФИО\t\t\tПауза\tГотов\tРазговор\tОбработка\tПерезвон";
        const int ReportStatesPadding = 10;
        const int ReportNamesPadding = 35;
        static readonly string operatorStatesReportHeaderBeautiful = "ФИО".PadRight(ReportNamesPadding) + "Пауза".PadRight(ReportStatesPadding) + "Готов".PadRight(ReportStatesPadding) + "Разговор".PadRight(ReportStatesPadding)+ "Обработка".PadRight(ReportStatesPadding) + "Перезвон".PadRight(ReportStatesPadding);
        /// <summary>
        /// Простая печать в консоль отчёта с максимальным кол-вом одновременных звонок за день.
        /// </summary>
        /// <param name="report"></param>
        public static void PrintDailyMaxReport(Dictionary<DateTime, int> report) 
        {
            //////////////////////
            //// TODO: Если уж это "вывод кудато",  то было бы не плохо его оформить в отдельный ReportPrinter, который на вход получает настройку куда вообще отчёт отправлять.
            Console.WriteLine(dailyMaxReportHeader);
            foreach(var a in report) 
            {
                Console.WriteLine($"{a.Key}\t\t{a.Value}");
            }
        }
        /// <summary>
        /// Простая печать в консоль отчёта с временем каждого состояния для оператора
        /// </summary>
        /// <param name="report"></param>
        public static void PrintOperatorStatesReport(Dictionary<string, Dictionary<SessionState, int>> report) 
        {
            StringBuilder stringBuilder = new StringBuilder();
            Console.WriteLine(operatorStatesReportHeader);
            foreach (var a in report)
            {
                stringBuilder.Append(a.Key);
                stringBuilder.Append("\t\t\t");
                foreach (var states in a.Value)
                {
                    stringBuilder.Append(states.Value);
                    stringBuilder.Append("\t");
                    
                }
                Console.WriteLine(stringBuilder);
                stringBuilder.Clear();
            }
        }
        /// <summary>
        /// Отформатированная печать в консоль отчёта с временем каждого состояния для оператора
        /// </summary>
        /// <param name="report"></param>
        public static void PrintOperatorStatesReportBeautiful(Dictionary<string, Dictionary<SessionState, int>> report) 
        {
            // Определение ширины колонок
            Console.WriteLine(operatorStatesReportHeaderBeautiful);
            foreach (var record in report)
            {               
                Console.WriteLine($"{record.Key,-ReportNamesPadding} {record.Value[SessionState.Pause],-ReportStatesPadding} {record.Value[SessionState.Ready],-ReportStatesPadding} {record.Value[SessionState.Talk],-ReportStatesPadding} {record.Value[SessionState.Processing],-ReportStatesPadding} {record.Value[SessionState.CallBack],-ReportStatesPadding}");
            }

        }
        #endregion
    }
}
