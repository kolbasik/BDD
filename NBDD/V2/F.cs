using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NBDD.V2
{
    [DebuggerStepThrough, DebuggerNonUserCode]
    public static class F
    {
        [DebuggerHidden]
        public static Func<TArg1, TArg2, Task> AsAsync<TArg1, TArg2>(this Action<TArg1, TArg2> action)
        {
            return (arg1, arg2) => action.Bind(arg1).Bind(arg2).AsAsync();
        }

        [DebuggerHidden]
        public static Func<TArg1, Task> AsAsync<TArg1>(this Action<TArg1> action)
        {
            return arg1 => action.Bind(arg1).AsAsync();
        }

        [DebuggerHidden]
        public static async Task AsAsync(this Action action)
        {
            await Task.Yield();
            action();
        }

        [DebuggerHidden]
        public static Action Bind<TArg>(this Action<TArg> action, TArg arg)
        {
            return () => action(arg);
        }

        [DebuggerHidden]
        public static Action<TArg2> Bind<TArg1, TArg2>(this Action<TArg1, TArg2> action, TArg1 arg1)
        {
            return arg2 => action(arg1, arg2);
        }

        [DebuggerHidden]
        public static Func<TReturn> Bind<TArg, TReturn>(this Func<TArg, TReturn> func, TArg arg)
        {
            return () => func(arg);
        }

        [DebuggerHidden]
        public static Func<TReturn> Bind<TArg, TReturn>(this Func<TArg, TReturn> func, Lazy<TArg> arg)
        {
            return () => func(arg.Value);
        }

        [DebuggerHidden]
        public static Func<TArg2, TReturn> Bind<TArg1, TArg2, TReturn>(this Func<TArg1, TArg2, TReturn> func, TArg1 arg1)
        {
            return arg2 => func(arg1, arg2);
        }

        [DebuggerHidden]
        public static Func<TArg2, TReturn> Bind<TArg1, TArg2, TReturn>(this Func<TArg1, TArg2, TReturn> func, Lazy<TArg1> arg1)
        {
            return arg2 => func(arg1.Value, arg2);
        }
    }
}