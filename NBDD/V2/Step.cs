using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NBDD.V2
{
    internal abstract class Unit
    {
        protected Unit(Func<Task> action)
        {
            Action = action;
        }

        public Func<Task> Action { get; }
    }

    [DebuggerDisplay("Bind")]
    internal class Bind : Unit
    {
        public Bind(Func<Task> action) : base(action)
        {
        }
    }

    [DebuggerDisplay("Step: {Title}")]
    internal sealed class Step : Unit
    {
        public Step(string title, Func<Task> action) : base(action)
        {
            Title = title;
        }

        public string Title { get; }
    }
}