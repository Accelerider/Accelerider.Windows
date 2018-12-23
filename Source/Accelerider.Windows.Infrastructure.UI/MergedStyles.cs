using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Accelerider.Windows.Infrastructure
{
    public class MergedStyles
    {
        private static Style MergeStyles(Style left, Style right)
        {
            if (TryGetNonNull(left, right, out var value)) return value;

            var result = new Style(
                GetMergedStyleType(left.TargetType, right.TargetType),
                MergeStyles(left.BasedOn, right.BasedOn))
            {
                // Merge Resources
                Resources = MergeResources(left.Resources, right.Resources)
            };

            // Merge Setters
            result.Setters.AddRange(MergeSetters(left.Setters, right.Setters));

            // Merge Triggers
            result.Triggers.AddRange(MergeTriggers(left.Triggers, result.Triggers));

            return result;
        }

        private static IEnumerable<Setter> MergeSetters(SetterBaseCollection left, SetterBaseCollection right)
        {
            if (TryGetNonNull(left, right, out var value)) return value?.Cast<Setter>();

            var result = new List<Setter>(right.Cast<Setter>());

            foreach (var setter in left.Cast<Setter>())
            {
                if (!result.Any(item => string.IsNullOrEmpty(setter.TargetName)
                    ? Equals(setter.Property, item.Property)
                    : Equals(setter.TargetName, item.TargetName) && Equals(setter.Property, item.Property)))
                {
                    result.Add(setter);
                }
            }

            return result;
        }

        private static ResourceDictionary MergeResources(ResourceDictionary left, ResourceDictionary right)
        {
            if (TryGetNonNull(left, right, out var value)) return value;

            var result = new ResourceDictionary();
            result.MergedDictionaries.Add(left);
            result.MergedDictionaries.Add(right);

            return result;
        }

        private static IEnumerable<TriggerBase> MergeTriggers(TriggerCollection left, TriggerCollection right)
        {
            if (TryGetNonNull(left, right, out var value)) return value;

            var result = new List<TriggerBase>(right);

            foreach (var triggerBase in left)
            {
                // TODO
                switch (triggerBase)
                {
                    case Trigger trigger:
                        //trigger.Setters
                        break;
                    case DataTrigger dataTrigger:
                        //dataTrigger.Setters
                        break;
                    case MultiTrigger multiTrigger:
                        //multiTrigger.Setters
                        break;
                    case MultiDataTrigger multiDataTrigger:
                        //multiDataTrigger.Setters
                        break;
                    case EventTrigger eventTrigger:
                        break;
                }
            }

            return result;
        }

        private static IEnumerable<TriggerAction> MergeActions(TriggerActionCollection left, TriggerActionCollection right)
        {
            if (TryGetNonNull(left, right, out var value)) return value;

            var result = new List<TriggerAction>(right);

            // TODO
            foreach (var triggerAction in left)
            {
            }

            return result;
        }

        private static bool Equals<T>(T left, T right) => EqualityComparer<T>.Default.Equals(left, right);

        private static bool TryGetNonNull<T>(T left, T right, out T result) where T : class
        {
            result = null;

            if (left == null && right == null) return true;
            if (left != null && right == null)
            {
                result = left;
                return true;
            }
            if (left == null)
            {
                result = right;
                return true;
            }

            return false;
        }

        private static Type GetMergedStyleType(Type left, Type right)
        {
            if (left == right) return left;
            if (left.IsAssignableFrom(right)) return right;
            if (right.IsAssignableFrom(left)) return left;

            throw new InvalidOperationException();
        }
    }
}
