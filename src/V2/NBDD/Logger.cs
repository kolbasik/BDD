using System;
using System.Diagnostics;

namespace NBDD.V2
{
    [DebuggerStepThrough, DebuggerNonUserCode]
    public sealed class Logger
    {
        private readonly Action<string> _output;

        public Logger(Action<string> output)
        {
            _output = output;
        }

        [DebuggerHidden]
        public void WriteLine(string message) => _output.Invoke(message);
    }
}