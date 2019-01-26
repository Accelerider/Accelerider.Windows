using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Accelerider.Windows.Infrastructure.Guards;


// ReSharper disable once CheckNamespace
namespace System
{
    public static class FunctionalExtensions
    {
        public static Func<TInput, IEnumerable<T>> Then<TInput, TOutput, T>(
            this Func<TInput, IEnumerable<TOutput>> @this,
            Func<TOutput, T> function)
        {
            ThrowIfNull(function);

            return input => @this(input).Select(function);
        }

        public static Func<TInput, T> Then<TInput, TOutput, T>(
            this Func<TInput, TOutput> @this,
            Func<TOutput, T> function)
        {
            ThrowIfNull(function);

            return input => function(@this(input));
        }

        public static Func<TInput, Task<T>> ThenAsync<TInput, TOutput, T>(
            this Func<TInput, Task<TOutput>> @this,
            Func<TOutput, T> function,
            CancellationToken cancellationToken = default)
        {
            ThrowIfNull(function);

            return async input =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var output = await @this(input);

                return function(output);
            };
        }

        public static Func<TInput, Task<IEnumerable<T>>> ThenAsync<TInput, TOutput, T>(
            this Func<TInput, Task<IEnumerable<TOutput>>> @this,
            Func<TOutput, T> function,
            CancellationToken cancellationToken = default)
        {
            ThrowIfNull(function);

            return async input =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var output = await @this(input);

                return output.Select(item =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return function(item);
                });
            };
        }

        public static Func<TInput, Task<T>> ThenAsync<TInput, TOutput, T>(
            this Func<TInput, Task<TOutput>> @this,
            Func<TOutput, Task<T>> function,
            CancellationToken cancellationToken = default)
        {
            ThrowIfNull(function);

            return async input =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var output = await @this(input);

                return await function(output);
            };
        }

        public static Func<TInput, Task<IEnumerable<T>>> ThenAsync<TInput, TOutput, T>(
            this Func<TInput, Task<IEnumerable<TOutput>>> @this,
            Func<TOutput, Task<T>> function,
            CancellationToken cancellationToken = default)
        {
            ThrowIfNull(function);

            return async input =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var output = await @this(input);

                var result = new List<T>();
                foreach (var item in output)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    result.Add(await function(item));
                }

                return result;
            };
        }

        public static Func<T0, Func<T1, TR>> Curry<T0, T1, TR>(this Func<T0, T1, TR> func)
            => a0 => a1 => func(a0, a1);

        public static Func<T0, Func<T1, T2, TR>> Curry<T0, T1, T2, TR>(this Func<T0, T1, T2, TR> func)
            => a0 => (a1, a2) => func(a0, a1, a2);

        public static Func<T0, Func<T1, T2, T3, TR>> Curry<T0, T1, T2, T3, TR>(this Func<T0, T1, T2, T3, TR> func)
            => a0 => (a1, a2, a3) => func(a0, a1, a2, a3);

        public static Func<T0, Func<T1, T2, T3, T4, TR>> Curry<T0, T1, T2, T3, T4, TR>(this Func<T0, T1, T2, T3, T4, TR> func)
            => a0 => (a1, a2, a3, a4) => func(a0, a1, a2, a3, a4);

        public static Func<T0, Func<T1, T2, T3, T4, T5, TR>> Curry<T0, T1, T2, T3, T4, T5, TR>(this Func<T0, T1, T2, T3, T4, T5, TR> func)
            => a0 => (a1, a2, a3, a4, a5) => func(a0, a1, a2, a3, a4, a5);

        public static Func<T0, Func<T1, T2, T3, T4, T5, T6, TR>> Curry<T0, T1, T2, T3, T4, T5, T6, TR>(this Func<T0, T1, T2, T3, T4, T5, T6, TR> func)
            => a0 => (a1, a2, a3, a4, a5, a6) => func(a0, a1, a2, a3, a4, a5, a6);

        public static Func<T0, Func<T1, T2, T3, T4, T5, T6, T7, TR>> Curry<T0, T1, T2, T3, T4, T5, T6, T7, TR>(this Func<T0, T1, T2, T3, T4, T5, T6, T7, TR> func)
            => a0 => (a1, a2, a3, a4, a5, a6, a7) => func(a0, a1, a2, a3, a4, a5, a6, a7);
    }
}
