using System.Collections.Generic;
using UnityEngine;
public class SimpleCircularQueue<T> {
    private readonly T[] array;
    private int enqueueCount;
    private int index;
    public SimpleCircularQueue(int limit) {
        array = new T[limit];
        enqueueCount = 0;
    }
    public void Enqueue(T element) {
        array[index] = element;
        if (enqueueCount < int.MaxValue) {
            enqueueCount++;
        }
        index++;
        if (index >= array.Length) {
            index = 0;
        }
    }
    public bool Contains(T data) {
        for (int i = 0; i < Mathf.Min(array.Length, enqueueCount); i++) {
            if (EqualityComparer<T>.Default.Equals(data, array[i])) {
                return true;
            }
        }
        return false;
    }
}
