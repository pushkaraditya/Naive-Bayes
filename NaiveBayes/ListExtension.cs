using System;
using System.Collections.Generic;
using System.Linq;

namespace NaiveBayes
{
    public static class ListExtension
    {
        public static double StdDev<T>(this IList<T> list, Func<T, double> selector, double? mean = null)
        {
            double avg = 0;
            if (mean.HasValue)
                avg = mean.Value;
            else
                avg = list.Average(selector);
            var variance = list.Sum(x => Math.Pow(selector(x) - avg, 2)) / (list.Count - 1);
            return Math.Sqrt(variance);
        }

        public static void SplitRandomly<T>(this IList<T> list, out IList<T> split, out IList<T> remaining, double splitRatio = .5)
        {
            if (list.Count == 0)
                split = remaining = null;
            split = list.ToList();
            remaining = new List<T>();
            var random = new Random();
            while (split.Count * splitRatio > remaining.Count)
            {
                var index = random.Next(0, split.Count - 1);
                var item = split[index];
                remaining.Add(item);
                split.Remove(item);
            }
        }
    }
}