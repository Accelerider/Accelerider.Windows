using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class FunctionalExtensions
    {
        public static Func<TInput, TContext, IEnumerable<T>> Then<TInput, TOutput, T, TContext>(
            this Func<TInput, TContext, IEnumerable<TOutput>> @this,
            Func<TOutput, TContext, T> function) 
            where TContext : ICloneable<TContext>
        {
            return (input, context) => @this(input, context).Select(item => function(item, context.Clone()));
        }

        public static Func<TInput, IEnumerable<T>> Then<TInput, TOutput, T>(
            this Func<TInput, IEnumerable<TOutput>> @this,
            Func<TOutput, T> function)
        {
            return input => @this(input).Select(function);
        }

        public static Func<TInput, TContext, T> Then<TInput, TOutput, T, TContext>(
            this Func<TInput, TContext, TOutput> @this,
            Func<TOutput, TContext, T> function)
        {
            return (input, context) => function(@this(input, context), context);
        }

        public static Func<TInput, T> Then<TInput, TOutput, T>(
            this Func<TInput, TOutput> @this,
            Func<TOutput, T> function)
        {
            return input => function(@this(input));
        }

        public static Func<TInput, TNewContext, TOutput> SwitchContext<TInput, TOutput, TContext, TNewContext>(
            this Func<TInput, TContext, TOutput> @this,
            TContext oldContext)
            where TContext : ICloneable<TContext>
            where TNewContext : ICloneable<TNewContext>
        {
            return (input, context) => @this(input, oldContext);
        }
    }
}
