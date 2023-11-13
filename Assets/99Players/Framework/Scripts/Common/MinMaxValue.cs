using System;
using UnityEngine;
[Serializable]
public class MinMaxValue {
    public float min;
    public float max;
    public MinMaxValue(float _min, float _max) {
        min = _min;
        max = _max;
    }
    public void SetRange(float _base, float _range) {
        min = _base - _range * 0.5f;
        max = _base + _range * 0.5f;
    }
    public float Random() {
        return UnityEngine.Random.Range(min, max);
    }
}
