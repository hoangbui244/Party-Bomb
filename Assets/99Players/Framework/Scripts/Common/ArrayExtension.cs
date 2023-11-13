using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public static class ArrayExtension {
    public static bool Exists<T>(this T[] array, T item) {
        return Array.Exists(array, (T p) => p.Equals(item));
    }
    public static bool Exists<T>(this T[] array, Predicate<T> match) {
        return Array.Exists(array, match);
    }
    public static T Find<T>(this T[] array, T item) {
        return Array.Find(array, (T p) => p.Equals(item));
    }
    public static T Find<T>(this T[] array, Predicate<T> match) {
        return Array.Find(array, match);
    }
    public static int FindIndex<T>(this T[] array, T item) {
        return Array.FindIndex(array, (T p) => p.Equals(item));
    }
    public static int FindIndex<T>(this T[] array, Predicate<T> match) {
        return Array.FindIndex(array, match);
    }
    public static T[] InsertTop<T>(this T[] array, T value) {
        T[] array2 = new T[array.Length + 1];
        array2[0] = value;
        Array.Copy(array, 0, array2, 1, array.Length);
        return array2;
    }
    public static T[] InsertLast<T>(this T[] array, T value) {
        T[] array2 = new T[array.Length + 1];
        Array.Copy(array, 0, array2, 0, array.Length);
        array2[array2.Length - 1] = value;
        return array2;
    }
    public static T[] Insert<T>(this T[] array, int index, T value) {
        if (array == null) {
            throw new ArgumentNullException("array");
        }
        if (index < 0 || index >= array.Length) {
            throw new ArgumentOutOfRangeException($"index is out of range. index={index}.");
        }
        T[] array2 = new T[array.Length + 1];
        Array.Copy(array, 0, array2, 0, index);
        array2[index] = value;
        Array.Copy(array, index, array2, index + 1, array.Length - index);
        return array2;
    }
    public static T[] InsertRange<T>(this T[] array, int index, IEnumerable<T> collection) {
        if (array == null) {
            throw new ArgumentNullException("array");
        }
        if (index < 0 || index >= array.Length) {
            throw new ArgumentOutOfRangeException($"index is out of range. index={index}.");
        }
        int num = collection.Count();
        T[] array2 = new T[array.Length + num];
        Array.Copy(array, 0, array2, 0, index);
        int num2 = 0;
        foreach (T item in collection) {
            array2[index + num2++] = item;
        }
        Array.Copy(array, index, array2, index + num, array.Length - index);
        return array2;
    }
    public static T[] RemoveAt<T>(this T[] array, int index) {
        if (array == null) {
            throw new ArgumentNullException("array");
        }
        if (index < 0 || index >= array.Length) {
            throw new ArgumentOutOfRangeException($"index is out of range. index={index}.");
        }
        T[] array2 = new T[array.Length - 1];
        Array.Copy(array, 0, array2, 0, index);
        Array.Copy(array, index + 1, array2, index, array.Length - index - 1);
        return array2;
    }
    public static T[] RemoveFirst<T>(this T[] array, T item) {
        int num = array.FindIndex(item);
        if (num == -1) {
            return null;
        }
        return array.RemoveAt(num);
    }
    public static T[] RemoveFirst<T>(this T[] array, Predicate<T> match) {
        int num = array.FindIndex(match);
        if (num == -1) {
            return null;
        }
        return array.RemoveAt(num);
    }
    public static T[] RemoveAll<T>(this T[] array, T item) {
        return array.RemoveAll((T elem) => elem.Equals(item));
    }
    public static T[] RemoveAll<T>(this T[] array, Predicate<T> match) {
        List<T> list = new List<T>();
        for (int i = 0; i < array.Length; i++) {
            if (!match(array[i])) {
                list.Add(array[i]);
            }
        }
        if (list.Count != array.Length) {
            return list.ToArray();
        }
        return null;
    }
    public static T PickupOne<T>(this T[] array) {
        return array[UnityEngine.Random.Range(0, array.Length)];
    }
    public static (T[], T) PickupOneAndRemove<T>(this T[] array) {
        T[] array2 = new T[array.Length - 1];
        int num = UnityEngine.Random.Range(0, array.Length);
        T item = array[num];
        int i = 0;
        int num2 = 0;
        for (; i < array.Length; i++) {
            if (i != num) {
                array2[num2++] = array[i];
            }
        }
        return (array2, item);
    }
    public static void Shuffle<T>(this T[] array) {
        for (int i = 0; i < array.Length; i++) {
            array.Swap(i, UnityEngine.Random.Range(0, array.Length));
        }
    }
    public static T[] GetNewRandomArray<T>(this T[] array) {
        T[] array2 = new T[array.Length];
        Array.Copy(array, array2, array.Length);
        array2.Shuffle();
        return array2;
    }
    public static void Swap<T>(this T[] array, int i, int j) {
        T val = array[i];
        array[i] = array[j];
        array[j] = val;
    }
    public static List<T> ToList<T>(this T[] array) {
        List<T> list = new List<T>();
        for (int i = 0; i < array.Length; i++) {
            list.Add(array[i]);
        }
        return list;
    }
    public static Dest[] Convert<T, Dest>(this T[] array, Func<T, Dest> func) {
        Dest[] array2 = new Dest[array.Length];
        for (int i = 0; i < array.Length; i++) {
            array2[i] = func(array[i]);
        }
        return array2;
    }
    public static void ForEach<T>(this T[] array, Action<T> action) {
        Array.ForEach(array, action);
    }
    public static void Sort<T>(this T[] array) {
        Array.Sort(array);
    }
    public static void Sort<T>(this T[] array, Comparison<T> comparer) {
        Array.Sort(array, comparer);
    }
    public static void Sort<TSource, TResult>(this TSource[] array, Func<TSource, TResult> selector1, Func<TSource, TResult> selector2) where TResult : IComparable {
        Array.Sort(array, delegate (TSource x, TSource y) {
            int num = selector1(y).CompareTo(selector1(x));
            return (num == 0) ? selector2(x).CompareTo(selector2(y)) : num;
        });
    }
}
