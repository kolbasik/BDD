using System;
using System.Diagnostics;

namespace NBDD.V2
{
    [DebuggerStepThrough, DebuggerNonUserCode]
    [DebuggerDisplay("Step: {Success} {Name}")]
    public sealed class StepResult
    {
        private StepResult(bool? success, string name, TimeSpan? elapsed)
        {
            Success = success;
            Name = name;
            Elapsed = elapsed;
        }

        public bool? Success { get; }
        public string Name { get; }
        public TimeSpan? Elapsed { get; }

        public static StepResult Skip(string title)
        {
            return new StepResult(null, title, null);
        }

        public static StepResult Done(string title, TimeSpan elapsed)
        {
            return new StepResult(true, title, elapsed);
        }

        public static StepResult Fail(string title, TimeSpan elapsed)
        {
            return new StepResult(false, title, elapsed);
        }
    }
}