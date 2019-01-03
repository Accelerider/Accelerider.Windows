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
    }
}
