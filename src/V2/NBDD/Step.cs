using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NBDD.V2
{
    [DebuggerStepThrough, DebuggerNonUserCode]
    [DebuggerDisplay("Step: {Title}")]
    public sealed class Step
    {
        public Step(string title, Func<Task> action)
        {
            if (title == null) throw new ArgumentNullException(nameof(title));
            if (action == null) throw new ArgumentNullException(nameof(action));
            Title = title;
            Action = action;
        }

        public string Title { get; }
        public Func<Task> Action { get; }
    }
}