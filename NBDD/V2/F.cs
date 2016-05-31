using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NBDD.V2
{
    [DebuggerStepThrough, DebuggerNonUserCode]
    public static class F
    {
        [DebuggerHidden]
        public static Lazy<TReturn> Lazy<TReturn>(TReturn value) => new Lazy<TReturn>(Identity(value));

        [DebuggerHidden]
        public static Func<TReturn> Identity<TReturn>(TReturn value) => () => value;

        [DebuggerHidden]
        public static Func<T1, T2, Task> AsAsync<T1, T2>(this Action<T1, T2> action)
        {
            return (arg1, arg2) => action.Bind(arg1).Bind(arg2).AsAsync().Invoke();
        }

        [DebuggerHidden]
        public static Func<T1, Task> AsAsync<T1>(this Action<T1> action)
        {
            return arg1 => action.Bind(arg1).AsAsync().Invoke();
        }

        [DebuggerHidden]
        public static Func<Task> AsAsync(this Action action)
        {
            return () => Task.Factory.FromAsync(action.BeginInvoke, action.EndInvoke, null);
        }

        [DebuggerHidden]
        public static Action Bind<T>(this Action<T> action, T arg)
        {
            return () => action.Invoke(arg);
        }

        [DebuggerHidden]
        public static Action<T2> Bind<T1, T2>(this Action<T1, T2> action, T1 arg1)
        {
            return arg2 => action.Invoke(arg1, arg2);
        }

        [DebuggerHidden]
        public static Func<TReturn> Bind<T, TReturn>(this Func<T, TReturn> func, T arg)
        {
            return () => func.Invoke(arg);
        }

        [DebuggerHidden]
        public static Func<TReturn> Bind<T, TReturn>(this Func<T, TReturn> func, Lazy<T> arg)
        {
            return () => func.Invoke(arg.Value);
        }

        [DebuggerHidden]
        public static Func<T2, TReturn> Bind<T1, T2, TReturn>(this Func<T1, T2, TReturn> func, T1 arg1)
        {
            return arg2 => func.Invoke(arg1, arg2);
        }

        [DebuggerHidden]
        public static Func<T2, TReturn> Bind<T1, T2, TReturn>(this Func<T1, T2, TReturn> func, Lazy<T1> arg1)
        {
            return arg2 => func.Invoke(arg1.Value, arg2);
        }
    }
}