using System.Diagnostics;

namespace NBDD.V2
{
    [DebuggerDisplay("Step: {Success} {Name}")]
    public sealed class StepResult
    {
        public StepResult(bool? success, string name)
        {
            Success = success;
            Name = name;
        }

        public bool? Success { get; }
        public string Name { get; }
    }
}