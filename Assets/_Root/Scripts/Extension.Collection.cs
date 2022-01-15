using System;
using System.Collections.Generic;

namespace Snorlax.Common
{
    public static partial class Util
    {
        /// <summary>
        /// Rounds an int to the closest int in an array (array has to be sorted)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int RoundIntToArray(int value, int[] array)
        {
            int min = 0;
            if (array[min] >= value) return array[min];

            int max = array.Length - 1;
            if (array[max] <= value) return array[max];

            while (max - min > 1)
            {
                int mid = (max + min) / 2;

                if (array[mid] == value)
                {
                    return array[mid];
                }
                else if (array[mid] < value)
                {
                    min = mid;
                }
                else
                {
                    max = mid;
                }
            }

            if (array[max] - value <= value - array[min])
            {
                return array[max];
            }
            else
            {
                return array[min];
            }
        }

        /// <summary>
        /// Rounds a float to the closest float in an array (array has to be sorted)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        public static float RoundFloatToArray(float value, float[] array)
        {
            int min = 0;
            if (array[min] >= value) return array[min];

            int max = array.Length - 1;
            if (array[max] <= value) return array[max];

            while (max - min > 1)
            {
                int mid = (max + min) / 2;

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (array[mid] == value)
                {
                    return array[mid];
                }
                else if (array[mid] < value)
                {
                    min = mid;
                }
                else
                {
                    max = mid;
                }
            }

            if (array[max] - value <= value - array[min])
            {
                return array[max];
            }
            else
            {
                return array[min];
            }
        }

        /// <summary>
        /// Check null for <paramref name="source"/>
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this ICollection<T> source) { return source == null || source.Count == 0; }

        /// <summary>
        /// Check null for <paramref name="source"/>
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this T[] source) { return source == null || source.Length == 0; }

        /// <summary>
        /// Ensure <paramref name="source"/> have data for <paramref name="key"/>
        /// otherwise asign the value return form expression <paramref name="newValue"/> to <paramref name="key"/>
        /// </summary>
        /// <param name="source">dictionary</param>
        /// <param name="key">key exist in <paramref name="source"/>></param>
        /// <param name="newValue">expression func retun value <typeparamref name="TValue"/>></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static TValue Ensure<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, Func<TValue> newValue) where TValue : class
        {
            source.TryGetValue(key, out var result);
            if (result != null) return result;

            result = newValue();
            source[key] = result;
            return result;
        }

        /// <summary>
        /// Ensure <paramref name="source"/> have data for <paramref name="key"/>
        /// do not use Ensure for performance and GC reasons
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static List<TValue> EnsureList<TKey, TValue>(this IDictionary<TKey, List<TValue>> source, TKey key)
        {
            //do not use Ensure for performance and GC reasons
            source.TryGetValue(key, out var result);
            if (result != null) return result;

            result = new List<TValue>();
            source[key] = result;
            return result;
            //return Ensure(source, key, () => new List<TV>());
        }

        /// <summary>
        /// Return value of <paramref name="key"/> in dictionary <paramref name="source"/>
        /// if <paramref name="key"/> dose not data return null
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
        {
            source.TryGetValue(key, out var value);
            return value;
        }

        /// <summary>
        /// Return value of <paramref name="key"/> in dictionary <paramref name="source"/>
        /// if <paramref name="key"/> dose not data return <paramref name="defaultValue"/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, TValue defaultValue)
        {
            return !source.TryGetValue(key, out var value) ? defaultValue : value;
        }

        /// <summary>
        /// Return value of <paramref name="key"/> in dictionary <paramref name="source"/>
        /// if <paramref name="key"/> dose not data return null
        /// </summary>
        /// <param name="source">if null retun default value for <typeparamref name="TValue"/>></param>
        /// <param name="key"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static TValue GetNullable<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key) { return source == null ? default : Get(source, key); }

        /// <summary>
        /// Iterate over all elements in the <paramref name="source"/> and call <paramref name="action"/> for it
        /// </summary>
        /// <param name="source"></param>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        public static void ForEach<T>(this List<T> source, Action<T> action)
        {
            if (source == null) return;
            for (var i = 0; i < source.Count; i++) action(source[i]);
        }

        /// <summary>
        /// equals value of <paramref name="source"/> and <paramref name="target"/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool Equal<T>(this List<T> source, List<T> target)
        {
            if (source == null && target == null) return true;

            if (source == null || target == null) return false;

            if (source.Count != target.Count) return false;

            if (source.Count == 0) return true;

            var comparer = EqualityComparer<T>.Default;
            var equal = true;
            for (var i = 0; i < source.Count; i++)
            {
                var val = source[i];
                if (comparer.Equals(target[i], val)) continue;
                equal = false;
                break;
            }

            return equal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="segment"></param>
        /// <returns></returns>
        public static byte[] ToArray(this ArraySegment<byte> segment)
        {
            if (segment.Count == 0) return new byte[0];
            var result = new byte[segment.Count];
            Buffer.BlockCopy(segment.Array,
                segment.Offset,
                result,
                0,
                segment.Count);
            return result;
        }
    }
}