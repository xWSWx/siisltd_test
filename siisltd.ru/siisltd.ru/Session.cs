using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramNameSpace.Session
{
    public enum SessionState
    {
        Pause, Ready, Talk, Processing, CallBack
    }
    public class Session
    {
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
        public string? ProjectName { get; set; }
        public string? OperatorName { get; set; }
        public SessionState State { get; set; }
        public int Duration { get; set; }
        ////////////////
        //// Скроем пока что
        private Session() { }

        public static Session CreateSession(String source)
        {
            var columns = source.Split(';');

            if (!DateTime.TryParse(columns[0], out DateTime startTime))
            {
                throw new ArgumentException($"cant parse {columns[0]} to DateTime");
            }
            if (!DateTime.TryParse(columns[1], out DateTime finishTime))
            {
                throw new ArgumentException($"cant parse {columns[1]} to DateTime");
            }
            var projectName = columns[2];
            var operatorName = columns[3];
            if (!Enum.TryParse(columns[4], out SessionState state)) 
            {
                throw new ArgumentException($"cant parse {columns[4]} to SessionState");
            }
            if (int.TryParse(columns[5], out int duration)) 
            {
                throw new ArgumentException($"cant parse {columns[5]} to int");
            }

            return new ()
            {
                StartTime = startTime,
                FinishTime = finishTime,
                ProjectName = projectName,
                OperatorName = operatorName,
                State = state,
                Duration = duration
            };
        }
    }
}
