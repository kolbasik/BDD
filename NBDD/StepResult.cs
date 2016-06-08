using System;
using System.Diagnostics;

namespace NBDD.V2
{
    [DebuggerStepThrough, DebuggerNonUserCode]
    [DebuggerDisplay("Step: {Success} {Name}")]
    public sealed class StepResult
    {
        public StepResult(bool? success, string name, TimeSpan? elapsed)
        {
            Success = success;
            Name = name;
            Elapsed = elapsed;
        }

        public bool? Success { get; }
        public string Name { get; }
        public TimeSpan? Elapsed { get; }
    }
}