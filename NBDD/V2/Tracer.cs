using System;
using System.Diagnostics;

namespace NBDD.V2
{
    [DebuggerStepThrough, DebuggerNonUserCode]
    public sealed class Tracer
    {
        private readonly Action<string> trace;

        public Tracer(Action<string> trace)
        {
            this.trace = trace;
        }

        [DebuggerHidden]
        public void Trace(string message) => trace.Invoke(message);
    }
}