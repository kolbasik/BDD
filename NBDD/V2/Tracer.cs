using System;

namespace NBDD.V2
{
    public sealed class Tracer
    {
        private readonly Action<string> trace;

        public Tracer(Action<string> trace)
        {
            this.trace = trace;
        }

        public void Trace(string message) => trace.Invoke(message);
    }
}