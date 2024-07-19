using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramNameSpace.Wrappers
{
    public class CustomCancellationToken
    {

        public Guid Id = Guid.Empty;
        public bool Cancelled = false;
        public bool Started = false;
        public ManualResetEventSlim EventSlim { get; private set; }
        public void Stop() => Cancelled = true;
        public void Start() => Started = true;
        public void Reset()
        {
            EventSlim.Set();
            Cancelled = Started = false;
        }
        public void Pause() => EventSlim.Reset();
        public void Resume() => EventSlim.Set();
        public void Wait() => EventSlim.Wait();
        public CustomCancellationToken() : base()
        {
            Id = Guid.Empty;
            EventSlim = new ManualResetEventSlim(true);
        }
        public CustomCancellationToken(Guid newId) : this()
        {
            Id = newId;
        }
    }
}
