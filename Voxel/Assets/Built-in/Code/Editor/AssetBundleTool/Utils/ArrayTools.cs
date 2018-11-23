using System;
using System.Collections;
using System.Collections.Generic;

namespace Tgame.AssetBundle
{

    /// <summary>
    /// Array's Tools 
    /// </summary>
    public static class ArrayTools
    {
        /// <summary>
        /// 全部过滤一遍
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sources"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static T[] FindAll<T>(this T[] sources, Predicate<T> p)
        {
            if (p == null)
                return sources;

            var list = new List<T>();

            for (int i = 0; i < sources.Length; i++)
            {
                var source = sources[i];

                if (p(source))
                    list.Add(source);
            }

            return list.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Each<T>(this T[] source, Action<T> fn)
        {
            for (int i = 0, len = source.Length; i < len; i++)
            {
                var item = source[i];

                fn(item);
            }
        }

        public static void Each<T>(this T[] source, Action<T, int> fn)
        {
            for (int i = 0, len = source.Length; i < len; i++)
            {
                var item = source[i];
                fn(item, i);
            }
        }

        /// <summary>
        /// 在指定的数组位置插入一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static T[] InsertAt<T>(T[] array, T value, int index)
        {
            T[] tmp = array;
            array = new T[array.Length + 1];
            Array.Copy(tmp, 0, array, 0, index);
            array[index] = value;
            Array.Copy(tmp, index, array, index + 1, tmp.Length - index);

            return array;
            // After 25 tests on 100k calls, this technique takes 43% more time
            //InsertAt( ref array, new T[]{value}, index ); 
        }

        /// <summary>
        /// 在指定的数组位置插入一个数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static T[] InsertAt<T>(T[] array, T[] value, int index)
        {
            T[] tmp = array;
            array = new T[array.Length + value.Length];
            Array.Copy(tmp, 0, array, 0, index);
            Array.Copy(value, 0, array, index, value.Length);
            Array.Copy(tmp, index, array, index + value.Length, tmp.Length - index);

            return array;
        }
        /// <summary>
        /// 在数组的头部添加一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T[] Push<T>(T[] array, T value) { return InsertAt<T>(array, value, 0); }

        /// <summary>
        ///  在数组的结尾添加一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T[] PushLast<T>(T[] array, T value) { return InsertAt<T>(array, value, array.Length); }

        /// <summary>
        /// 在start和end之间移除所有的数组元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static T[] RemoveRange<T>(T[] array, int start, int end) { return RemoveAt<T>(array, start, end - start + 1); }
        /// <summary>
        ///  在指定的位置移除一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static T[] RemoveAt<T>(T[] array, int index) { return RemoveAt<T>(array, index, 1); }
        /// <summary>
        ///  从指定的位置开始移除指定个数的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static T[] RemoveAt<T>(T[] array, int start, int count)
        {
            T[] tmp = array;
            array = new T[array.Length - count >= 0 ? array.Length - count : 0];
            Array.Copy(tmp, array, start);
            int index = start + count;
            if (index < tmp.Length)
                Array.Copy(tmp, index, array, start, tmp.Length - index);

            return array;
        }

        /// <summary>
        ///  移除第一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static T[] Pop<T>(T[] array) { return RemoveAt<T>(array, 0, 1); }
        /// <summary>
        ///  移除指定个数的开头元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static T[] Pop<T>(T[] array, int count) { return RemoveAt<T>(array, 0, count); }
        /// <summary>
        ///  移除结尾的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static T[] PopLast<T>(T[] array) { return RemoveAt<T>(array, array.Length - 1, 1); }
        /// <summary>
        ///  移除指定个数的结尾元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static T[] PopLast<T>(T[] array, int count) { return RemoveAt<T>(array, array.Length - count, count); }

        /// <summary>
        ///  找到指定元素并且移除他
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T[] Remove<T>(T[] array, T value)
        {
            int index = Array.IndexOf<T>(array, value);
            if (index >= 0)
                return RemoveAt<T>(array, index);
            return array;
        }
        /// <summary>
        ///  移除所有和指定元素匹配的数组元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T[] RemoveAll<T>(T[] array, T value)
        {
            int index = 0;
            do
            {
                index = Array.IndexOf<T>(array, value);
                if (index >= 0)
                    array = RemoveAt<T>(array, index);
            }
            while (index >= 0 && array.Length > 0);
            return array;
        }


        /// <summary>
        /// 移动指定count数量数组元素从indice 到 indice+decalage 
        /// 可能存在性能问题的Clone()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="indice"></param>
        /// <param name="count"></param>
        /// <param name="decalage"></param>
        /// <returns></returns>

        public static T[] Shift<T>(T[] array, int indice, int count, int decalage)
        {
            if (array == null) return null;
            T[] result = (T[])array.Clone();

            indice = indice < 0 ? 0 : (indice >= result.Length ? result.Length - 1 : indice);
            count = count < 0 ? 0 : (indice + count >= result.Length ? result.Length - indice - 1 : count);
            decalage = indice + decalage < 0 ? -indice : (indice + count + decalage >= result.Length ? result.Length - indice - count : decalage);

            int absDec = Math.Abs(decalage);
            T[] items = new T[count]; // What we want to move
            T[] dec = new T[absDec]; // What is going to replace the thing we move
            Array.Copy(array, indice, items, 0, count);
            Array.Copy(array, indice + (decalage >= 0 ? count : decalage), dec, 0, absDec);
            Array.Copy(dec, 0, result, indice + (decalage >= 0 ? 0 : decalage + count), absDec);
            Array.Copy(items, 0, result, indice + decalage, count);

            return result;
        }

        /// <summary>
        ///  移动指定元素到尾部
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="indice"></param>
        /// <returns></returns>
        public static T[] Shr<T>(T[] array, int indice) { return Shift<T>(array, indice, 1, 1); }

        /// <summary>
        ///  移动指定元素到头部
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="indice"></param>
        /// <returns></returns>
        public static T[] Shl<T>(T[] array, int indice) { return Shift<T>(array, indice, 1, -1); }

        /// <summary>
        ///  连接所有的数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arrays"></param>
        /// <returns></returns>
        public static T[] Concat<T>(params T[][] arrays)
        {
            int count = 0;
            for (int i = 0, len = arrays.Length; i < len; i++) count += arrays[i].Length;
            T[] result = new T[count];

            count = 0;
            for (int i = 0; i < arrays.Length; i++)
            {
                Array.Copy(arrays[i], 0, result, count, arrays[i].Length);
                count += arrays[i].Length;
            }

            return result;
        }

        /// <summary>
        ///  返回数组的子集 从start到end
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static T[] SubArray<T>(T[] source, int start, int end)
        {
            int count = end - start + 1;
            T[] result = new T[count];
            Array.Copy(source, start, result, 0, count);

            return result;
        }

        /// <summary>
        ///  把数组随机打乱
        ///  http://www.codeproject.com/Articles/35114/Shuffling-arrays-in-C
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static T[] Shuffle<T>(T[] array) { return Shuffle<T>(array, 0, array.Length - 1); }
        /// <summary>
        ///  把指定范围内的数组元素随机打乱
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static T[] Shuffle<T>(T[] array, int start, int end)
        {
            int count = end - start + 1;
            T[] shuffledPart = new T[count];
            Array.Copy(array, start, shuffledPart, 0, count);

            var matrix = new SortedList();
            var r = new System.Random();

            for (var x = 0; x <= shuffledPart.GetUpperBound(0); x++)
            {
                var i = r.Next();
                while (matrix.ContainsKey(i)) { i = r.Next(); }
                matrix.Add(i, shuffledPart[x]);
            }

            matrix.Values.CopyTo(shuffledPart, 0);
            T[] result = (T[])array.Clone();
            Array.Copy(shuffledPart, 0, result, start, count);

            return result;
        }

        /// <summary>
        ///  随机插入指定数组元素指定次数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static T[] Sow<T>(T[] array, T value, int count) { return Sow<T>(array, value, count, 0, array.Length - 1, true); }
        /// <summary>
        ///  随机插入指定数组元素指定次数在指定范围
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <param name="count"></param>
        /// <param name="lowerBound"></param>
        /// <param name="upperBound"></param>
        /// <param name="includeBounds"></param>
        /// <returns></returns>
        public static T[] Sow<T>(T[] array, T value, int count, int lowerBound, int upperBound, bool includeBounds)
        {
            T[] result = (T[])array.Clone();
            var r = new System.Random();
            lowerBound += includeBounds ? 0 : 1;
            upperBound += includeBounds ? 2 : 1;

            for (int i = 0; i < count; i++)
                result = InsertAt<T>(result, value, r.Next(lowerBound, upperBound++));

            return result;
        }

        /// <summary>
        ///  创建一个重复指定次数的数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static T[] CreateRepeat<T>(T value, int count)
        {
            var instance = new T[count];

            for (int i = 0; i < count; i++)
            {
                instance[i] = value;
            }

            return instance;
        }

        /// <summary>
        /// 创建一个int数组指定长度,指定最大值和最小值
        /// </summary>
        /// <param name="count"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int[] CreateRandom(int count, int min, int max)
        {
            Random rand = new Random();

            var instance = new int[count];
            for (int i = 0; i < count; i++) instance[i] = rand.Next(min, max);

            return instance;
        }


        /// <summary>
        /// 通过一个函数创建一个指定长度的数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="count"></param>
        /// <param name="constructor"></param>
        /// <returns></returns>
        public static T[] Create<T>(int count, Func<T> constructor)
        {
            T[] instance = new T[count];
            for (int i = 0; i < count; i++)
                instance[i] = constructor();

            return instance;
        }

        /// <summary>
        /// 通过一个函数创建一个指定长度的数组 函数可以知道当前创建的数组位置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="count"></param>
        /// <param name="constructor"></param>
        /// <returns></returns>
        public static T[] Create<T>(int count, Func<int, T> constructor)
        {
            T[] instance = new T[count];
            for (int i = 0; i < count; i++)
                instance[i] = constructor(i);

            return instance;
        }
        /// <summary>
        /// 对整个数组应用一个更新函数,函数传入一个数组元素 传出一个数组元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="selectFunc"></param>
        /// <returns></returns>
        public static T[] Update<T>(T[] array, Func<T, T> selectFunc) { return Update(array, selectFunc, 0, array.Length - 1); }
        /// <summary>
        ///  对指定区间的数组应用一个更新函数,函数传入一个数组元素 传出一个数组元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="selectFunc"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static T[] Update<T>(T[] array, Func<T, T> selectFunc, int start, int end)
        {
            T[] result = (T[])array.Clone();
            for (int i = start; i <= end; i++)
                result[i] = selectFunc(array[i]);

            return result;
        }

        /// <summary>
        /// 对整个数组应用一个更新函数,函数传入一个数组元素和数组位置 传出一个数组元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="selectFunc"></param>
        /// <returns></returns>
        public static T[] Update<T>(T[] array, Func<T, int, T> selectFunc) { return Update(array, selectFunc, 0, array.Length - 1); }
        /// <summary>
        /// 对指定区间的数组应用一个更新函数,函数传入一个数组元素和数组位置 传出一个数组元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="selectFunc"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static T[] Update<T>(T[] array, Func<T, int, T> selectFunc, int start, int end)
        {
            T[] result = (T[])array.Clone();
            for (int i = start; i <= end; i++)
                result[i] = selectFunc(array[i], i);

            return result;
        }
    }
}