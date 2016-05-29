using System;

namespace NBDD.V2
{
    public sealed class Tracer
    {
        private readonly Action<string> _trace;

        public Tracer(Action<string> trace)
        {
            _trace = trace;
        }

        public void Trace(string message) => _trace.Invoke(message);
    }
}