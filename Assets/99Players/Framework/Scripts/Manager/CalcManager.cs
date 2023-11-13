using System;
using System.Collections.Generic;
using UnityEngine;
public class CalcManager : MonoBehaviour {
    public enum AXIS {
        X,
        Y,
        Z,
        NONE
    }
    public static Vector3 mCalcVector3;
    public static Vector2 mCalcVector2;
    public static int mCalcInt;
    public static float mCalcFloat;
    public static bool mCalcBool;
    public static Color mCalcColor;
    public static Vector3 mVector3Zero = Vector3.zero;
    private static Vector3 mCalcVector3Buf;
    private static Vector2 mCalcVector2Buf;
    private static float mCalcFloatBuf;
    public static Quaternion mCalcQuaternion;
    public static GameObject mCalcGameObject;
    public static List<Vector3> mCalcVector3List;
    private static string[] arrayRecordStirng = new string[11];
    public const float GRAVITY_SCALE = -9.81f;
    private static bool isAscendingOrder = false;
    public static bool IsPerCheck(int _value) {
        return UnityEngine.Random.Range(0, 100) < _value;
    }
    public static bool IsPerCheck(float _value) {
        return (float)UnityEngine.Random.Range(0, 100) < _value;
    }
    public static bool GetHalfProbability() {
        return UnityEngine.Random.Range(0, 2) == 0;
    }
    public static float RandomPlusOrMinus() {
        if (!GetHalfProbability()) {
            return -1f;
        }
        return 1f;
    }
    public static float RandomPlusOrMinus(float _value) {
        if (GetHalfProbability()) {
            return 0f - _value;
        }
        return _value;
    }
    public static bool IsSphereOverlap(Vector3 _pos1, float _rad1, Vector3 _pos2, float _rad2) {
        if (Mathf.Pow(_pos2.x - _pos1.x, 2f) + Mathf.Pow(_pos2.y - _pos2.y, 2f) + Mathf.Pow(_pos2.z - _pos2.z, 2f) <= Mathf.Pow(_rad1 + _rad2, 2f)) {
            return true;
        }
        return false;
    }
    public static Vector2 DirVec2(GameObject obj, GameObject target) {
        mCalcVector2Buf.y = Mathf.Atan2(target.transform.position.y - obj.transform.position.y, target.transform.position.x - obj.transform.position.x);
        mCalcVector2Buf.x = Mathf.Cos(mCalcVector2Buf.y);
        mCalcVector2Buf.y = Mathf.Sin(mCalcVector2Buf.y);
        return mCalcVector2Buf;
    }
    public static Vector2 DirVec2(Vector3 obj, Vector3 target) {
        mCalcVector2Buf.y = Mathf.Atan2(target.y - obj.y, target.x - obj.x);
        mCalcVector2Buf.x = Mathf.Cos(mCalcVector2Buf.y);
        mCalcVector2Buf.y = Mathf.Sin(mCalcVector2Buf.y);
        return mCalcVector2Buf;
    }
    public static float Length(GameObject obj, GameObject target, bool _isCalcX = true, bool _isCalcY = true, bool _isCalcZ = true) {
        return Mathf.Sqrt((_isCalcX ? ((target.transform.position.x - obj.transform.position.x) * (target.transform.position.x - obj.transform.position.x)) : 0f) + (_isCalcY ? ((target.transform.position.y - obj.transform.position.y) * (target.transform.position.y - obj.transform.position.y)) : 0f) + (_isCalcZ ? ((target.transform.position.z - obj.transform.position.z) * (target.transform.position.z - obj.transform.position.z)) : 0f));
    }
    public static float Length(Vector3 obj, Vector3 target, bool _isCalcX = true, bool _isCalcY = true, bool _isCalcZ = true) {
        return Mathf.Sqrt((_isCalcX ? ((target.x - obj.x) * (target.x - obj.x)) : 0f) + (_isCalcY ? ((target.y - obj.y) * (target.y - obj.y)) : 0f) + (_isCalcZ ? ((target.z - obj.z) * (target.z - obj.z)) : 0f));
    }
    public static float Length(Vector2 obj, Vector2 target) {
        return Mathf.Sqrt((target.x - obj.x) * (target.x - obj.x) + (target.y - obj.y) * (target.y - obj.y));
    }
    public static float Vec2Length(Vector3 _obj, Vector3 _target) {
        _obj.z = (_target.z = 0f);
        return Mathf.Sqrt((_target.x - _obj.x) * (_target.x - _obj.x) + (_target.y - _obj.y) * (_target.y - _obj.y) + (_target.z - _obj.z) * (_target.z - _obj.z));
    }
    public static Vector3 Vec3Center(Vector3 _obj, Vector3 _target) {
        return (_obj + _target) * 0.5f;
    }
    public static Vector2 Vec2Center(Vector2 _obj, Vector2 _target) {
        return (_obj + _target) * 0.5f;
    }
    public static float Vec2Cross(Vector2 lhs, Vector2 rhs) {
        return lhs.x * rhs.y - rhs.x * lhs.y;
    }
    public static bool CheckRange(int _value, int _min, int _max, bool _include = true) {
        if (_include) {
            if (_value >= _min) {
                return _value <= _max;
            }
            return false;
        }
        if (_value > _min) {
            return _value < _max;
        }
        return false;
    }
    public static bool CheckRange(float _value, float _min, float _max, bool _include = true) {
        if (_include) {
            if (_value >= _min) {
                return _value <= _max;
            }
            return false;
        }
        if (_value > _min) {
            return _value < _max;
        }
        return false;
    }
    public static bool CheckRotArea(Vector3 _center, Vector3 _traget, float _start, float _end) {
        if (AngleUsingVec(_center, _traget) >= _start) {
            return AngleUsingVec(_center, _traget) <= _end;
        }
        return false;
    }
    public static bool CheckIntersection(float ax, float ay, float bx, float by, float cx, float cy, float dx, float dy) {
        float num = (cx - dx) * (ay - cy) + (cy - dy) * (cx - ax);
        float num2 = (cx - dx) * (by - cy) + (cy - dy) * (cx - bx);
        float num3 = (ax - bx) * (cy - ay) + (ay - by) * (ax - cx);
        float num4 = (ax - bx) * (dy - ay) + (ay - by) * (ax - dx);
        if (num3 * num4 <= 0f) {
            return num * num2 <= 0f;
        }
        return false;
    }
    public static float AngleUsingVec(Vector3 _center, Vector3 _target) {
        return 57.29578f * Mathf.Atan2(_center.y - _target.y, _center.x - _target.x) + 90f;
    }
    public static float Rot(Vector3 _vec, AXIS _axis, bool _deg = true) {
        switch (_axis) {
            case AXIS.X:
                mCalcVector2Buf.y = Mathf.Atan2(_vec.z, _vec.y);
                break;
            case AXIS.Y:
                mCalcVector2Buf.y = Mathf.Atan2(_vec.x, _vec.z);
                break;
            case AXIS.Z:
                mCalcVector2Buf.y = Mathf.Atan2(0f - _vec.x, _vec.y);
                break;
        }
        if (_deg) {
            mCalcVector2Buf.y *= 57.29578f;
            if (mCalcVector2Buf.y < 0f) {
                mCalcVector2Buf.y = 360f + mCalcVector2Buf.y;
            }
        }
        return mCalcVector2Buf.y;
    }
    public static float VecToAngular(Vector3 _vec, AXIS _axis, bool _deg = false) {
        switch (_axis) {
            case AXIS.X:
                mCalcFloatBuf = _vec.z / Mathf.Sqrt(_vec.z * _vec.z + _vec.y * _vec.y);
                break;
            case AXIS.Y:
                mCalcFloatBuf = _vec.x / Mathf.Sqrt(_vec.x * _vec.x + _vec.z * _vec.z);
                break;
            case AXIS.Z:
                mCalcFloatBuf = _vec.x / Mathf.Sqrt(_vec.x * _vec.x + _vec.y * _vec.y);
                break;
        }
        if (!_deg) {
            return mCalcFloatBuf;
        }
        return mCalcFloatBuf * 57.29578f;
    }
    public static float Rot(Vector3 origin, Vector3 fromDirection, Vector3 toDirection, Vector3 axis) {
        fromDirection.Normalize();
        axis.Normalize();
        Vector3 rhs = toDirection - axis * Vector3.Dot(axis, toDirection);
        rhs.Normalize();
        return Mathf.Acos(Mathf.Clamp(Vector3.Dot(fromDirection, rhs), -1f, 1f)) * ((Vector3.Dot(Vector3.Cross(axis, fromDirection), rhs) < 0f) ? (-57.29578f) : 57.29578f);
    }
    public static Vector3 PosRotation2D(Vector3 _pos, Vector3 _center, float _rot, AXIS _axis) {
        switch (_axis) {
            case AXIS.X:
                _pos.y = 0f - _pos.y;
                _center.y = 0f - _center.y;
                mCalcVector3Buf.z = (_pos.z - _center.z) * Mathf.Cos(_rot * ((float)Math.PI / 180f)) - (_pos.y - _center.y) * Mathf.Sin(_rot * ((float)Math.PI / 180f)) + _center.z;
                mCalcVector3Buf.y = -1f * ((_pos.z - _center.z) * Mathf.Sin(_rot * ((float)Math.PI / 180f)) + (_pos.y - _center.y) * Mathf.Cos(_rot * ((float)Math.PI / 180f)) + _center.y);
                mCalcVector3Buf.x = _pos.x;
                break;
            case AXIS.Y:
                _pos.z = 0f - _pos.z;
                _center.z = 0f - _center.z;
                mCalcVector3Buf.x = (_pos.x - _center.x) * Mathf.Cos(_rot * ((float)Math.PI / 180f)) - (_pos.z - _center.z) * Mathf.Sin(_rot * ((float)Math.PI / 180f)) + _center.x;
                mCalcVector3Buf.z = -1f * ((_pos.x - _center.x) * Mathf.Sin(_rot * ((float)Math.PI / 180f)) + (_pos.z - _center.z) * Mathf.Cos(_rot * ((float)Math.PI / 180f)) + _center.z);
                mCalcVector3Buf.y = _pos.y;
                break;
            case AXIS.Z:
                _pos.y = 0f - _pos.y;
                _center.y = 0f - _center.y;
                mCalcVector3Buf.x = (_pos.x - _center.x) * Mathf.Cos(_rot * ((float)Math.PI / 180f)) - (_pos.y - _center.y) * Mathf.Sin(_rot * ((float)Math.PI / 180f)) + _center.x;
                mCalcVector3Buf.y = -1f * ((_pos.x - _center.x) * Mathf.Sin(_rot * ((float)Math.PI / 180f)) + (_pos.y - _center.y) * Mathf.Cos(_rot * ((float)Math.PI / 180f)) + _center.y);
                mCalcVector3Buf.z = _pos.z;
                break;
        }
        return mCalcVector3Buf;
    }
    public Vector3[] GetGridPos(int _row, int _column, float _wGap, float _hGap) {
        mCalcVector3List.Clear();
        for (int i = 0; i < _row * _column; i++) {
            mCalcVector3Buf.x = (float)(i % _row) * _wGap;
            mCalcVector3Buf.y = (float)(i / _row) * _hGap;
            mCalcVector3List.Add(mCalcVector3Buf);
        }
        return mCalcVector3List.ToArray();
    }
    public static float GetVelocityTopTime(float _velocity, float _g = -9.81f, bool _direct = true) {
        if (_direct) {
            _g = 0f - _g;
        }
        return _velocity / _g;
    }
    public static float GetVelocityTopPos(float _velocity, float _startHeight, float _g = -9.81f, bool _direct = true) {
        if (_direct) {
            _g = 0f - _g;
        }
        return Mathf.Pow(_velocity, 2f) / (2f * _g) + _startHeight;
    }
    public static Vector3 GetVelocityTopPos(Vector3 _velocity, Vector3 _startPos, float _g = -9.81f, bool _direct = true) {
        if (_direct) {
            _g = 0f - _g;
        }
        return GetVelocityFallPositionY(_velocity, _startPos, Mathf.Pow(_velocity.y, 2f) / (2f * _g) + _startPos.y, _g, _direct: false);
    }
    public static float GetVelocityFallGroundTime(Vector3 _velocity, Vector3 _startPos, float _groundHeight, float _g = -9.81f, bool _direct = true) {
        if (_direct) {
            _g = 0f - _g;
        }
        float num = GetVelocityTopPos(_velocity.y, _startPos.y, _g, _direct: false) - _groundHeight;
        return Mathf.Sqrt(2f * _g * num) / _g + GetVelocityTopTime(_velocity.y, _g, _direct: false);
    }
    public static Vector3 GetVelocityFallPositionY(Vector3 _velocity, Vector3 _startPos, float _y, float _g = -9.81f, bool _direct = true) {
        if (_direct) {
            _g = 0f - _g;
        }
        float velocityFallGroundTime = GetVelocityFallGroundTime(_velocity, _startPos, _y, _g, _direct: false);
        return new Vector3(_velocity.x * velocityFallGroundTime, _y, _velocity.z * velocityFallGroundTime) + new Vector3(_startPos.x, 0f, _startPos.z);
    }
    public static Vector3 GetVelocityFallPositionX(Vector3 _velocity, Vector3 _startPos, float _x, float _g = -9.81f, bool _direct = true) {
        if (_direct) {
            _g = 0f - _g;
        }
        float time = (_x - _startPos.x) / _velocity.x;
        return GetVelocityTimeToPosition(_velocity, _startPos, time, _g, _direct: false);
    }
    public static Vector3 GetVelocityFallPositionZ(Vector3 _velocity, Vector3 _startPos, float _z, float _g = -9.81f, bool _direct = true) {
        if (_direct) {
            _g = 0f - _g;
        }
        float time = (_z - _startPos.z) / _velocity.z;
        return GetVelocityTimeToPosition(_velocity, _startPos, time, _g, _direct: false);
    }
    public static Vector3 GetVelocityTimeToPosition(Vector3 _velocity, Vector3 _startPos, float _time, float _g = -9.81f, bool _direct = true) {
        if (_direct) {
            _g = 0f - _g;
        }
        float x = _velocity.x * _time;
        float z = _velocity.z * _time;
        float y = _velocity.y * _time - 0.5f * _g * Mathf.Pow(_time, 2f);
        return new Vector3(x, y, z) + _startPos;
    }
    public static Vector3 GetVelocityPositionVec(Vector3 _startPos, Vector3 _targetPos, float _time, float _g = -9.81f, bool _direct = true) {
        if (_direct) {
            _g = 0f - _g;
        }
        float x = (_targetPos.x - _startPos.x) / _time;
        float z = (_targetPos.z - _startPos.z) / _time;
        float y = (_g / 2f * _time * _time + (_targetPos.y - _startPos.y)) / _time;
        return new Vector3(x, y, z);
    }
    public static Vector3 GetVelocityPredictionPos(Vector3 _startPos, Vector3 _targetPos, Vector3 _gravity, float time, float drag = 0f) {
        float num = _gravity.x * 0.5f;
        float num2 = _gravity.y * 0.5f;
        float num3 = _gravity.z * 0.5f;
        Vector3 vector = _targetPos * 0.314159f * drag * Mathf.Pow(time, 2f);
        float x = _targetPos.x * time + num * Mathf.Pow(time, 2f) - vector.x;
        float y = _targetPos.y * time + num2 * Mathf.Pow(time, 2f) - vector.y;
        float z = _targetPos.z * time + num3 * Mathf.Pow(time, 2f) - vector.z;
        return _startPos + new Vector3(x, y, z);
    }
    public static float GetVelocityTopVec(float _targetHeight, float _g = -9.81f, bool _direct = true) {
        if (_direct) {
            _g = 0f - _g;
        }
        return Mathf.Sqrt(2f * _g * _targetHeight);
    }
    public static DateTime RandomDateTime(DateTime _startDateTime, DateTime _endDatetime) {
        if (_startDateTime > _endDatetime) {
            return _startDateTime;
        }
        float num = UnityEngine.Random.Range(0f, (float)(_endDatetime - _startDateTime).TotalMilliseconds);
        return _startDateTime + TimeSpan.FromMilliseconds(num);
    }
    public static int ArraySum(params int[] _array) {
        int num = 0;
        for (int i = 0; i < _array.Length; i++) {
            num += _array[i];
        }
        return num;
    }
    public static float ArraySum(params float[] _array) {
        float num = 0f;
        for (int i = 0; i < _array.Length; i++) {
            num += _array[i];
        }
        return num;
    }
    public static int GetRandomIndex(params int[] _weightTable) {
        float[] array = new float[_weightTable.Length];
        _weightTable.CopyTo(array, 0);
        return GetRandomIndex(array);
    }
    public static int GetRandomIndex(params float[] _weightTable) {
        float maxInclusive = ArraySum(_weightTable);
        float num = UnityEngine.Random.Range(0f, maxInclusive);
        int result = -1;
        for (int i = 0; i < _weightTable.Length; i++) {
            if (_weightTable[i] >= num) {
                result = i;
                break;
            }
            num -= _weightTable[i];
        }
        return result;
    }
    public static int GetListContentCount() {
        return 0;
    }
    public static bool CheckInSight(Vector3 _basePos, Vector3 _targetPos, Vector3 _sightVec, float _sightAngle, float _sightLength, AXIS _axis = AXIS.NONE) {
        switch (_axis) {
            case AXIS.X:
                _basePos.x = 0f;
                _targetPos.x = 0f;
                _sightVec.x = 0f;
                break;
            case AXIS.Y:
                _basePos.y = 0f;
                _targetPos.y = 0f;
                _sightVec.y = 0f;
                break;
            case AXIS.Z:
                _basePos.z = 0f;
                _targetPos.z = 0f;
                _sightVec.z = 0f;
                break;
        }
        if (Vector3.Angle(_targetPos - _basePos, _sightVec) <= _sightAngle * 0.5f && (_targetPos - _basePos).magnitude <= _sightLength) {
            return true;
        }
        return false;
    }
    public static Vector3 CalcCameraViewSize(float _fieldOfView, float _aspect, float _distance) {
        mCalcVector3Buf.y = 2f * _distance * Mathf.Tan(_fieldOfView * 0.5f * ((float)Math.PI / 180f));
        mCalcVector3Buf.x = mCalcVector3Buf.y * _aspect;
        return mCalcVector3Buf;
    }
    public static float CalcCameraDistance(float _frustumHeight, float _fieldOfView) {
        return _frustumHeight * 0.5f / Mathf.Tan(_fieldOfView * 0.5f * ((float)Math.PI / 180f));
    }
    public static bool LineSegmentsIntersection(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, out Vector3 intersection) {
        intersection = Vector3.zero;
        float num = (p2.x - p1.x) * (p4.z - p3.z) - (p2.z - p1.z) * (p4.x - p3.x);
        if (num == 0f) {
            return false;
        }
        float num2 = ((p3.x - p1.x) * (p4.z - p3.z) - (p3.z - p1.z) * (p4.x - p3.x)) / num;
        float num3 = ((p3.x - p1.x) * (p2.z - p1.z) - (p3.z - p1.z) * (p2.x - p1.x)) / num;
        if (num2 < 0f || num2 > 1f || num3 < 0f || num3 > 1f) {
            return false;
        }
        intersection.x = p1.x + num2 * (p2.x - p1.x);
        intersection.z = p1.z + num2 * (p2.z - p1.z);
        return true;
    }
    public static T[] ShuffleList<T>(T[] array) {
        T[] array2 = new T[array.Length];
        for (int i = 0; i < array2.Length; i++) {
            array2[i] = array[i];
        }
        for (int j = 0; j < array2.Length * 2; j++) {
            int num = UnityEngine.Random.Range(0, array2.Length);
            int num2 = UnityEngine.Random.Range(0, array2.Length);
            T val = array2[num];
            array2[num] = array2[num2];
            array2[num2] = val;
        }
        return array2;
    }
    public static void QuickSort(int[] sortArray, bool _isAscendingOrder) {
        isAscendingOrder = _isAscendingOrder;
        QuickSort(sortArray, 0, sortArray.Length - 1);
    }
    private static void QuickSort(int[] sortArray, int startElementNo, int endElementNo) {
        if (startElementNo != endElementNo) {
            int num = Pivot(sortArray, startElementNo, endElementNo);
            if (num != -1) {
                int num2 = Partition(sortArray, startElementNo, endElementNo, sortArray[num]);
                QuickSort(sortArray, startElementNo, num2 - 1);
                QuickSort(sortArray, num2, endElementNo);
            }
        }
    }
    private static int Pivot(int[] a, int i, int j) {
        int k;
        for (k = i + 1; k <= j && a[i] == a[k]; k++) {
        }
        if (k > j) {
            return -1;
        }
        if (a[i] >= a[k]) {
            return i;
        }
        return k;
    }
    private static int Partition(int[] sortArray, int startElementNo, int endElementNo, int pivotElement) {
        int i = startElementNo;
        int num = endElementNo;
        while (i <= num) {
            if (isAscendingOrder) {
                for (; i <= endElementNo && sortArray[i] < pivotElement; i++) {
                }
                while (num >= startElementNo && sortArray[num] >= pivotElement) {
                    num--;
                }
            } else {
                for (; i <= endElementNo && sortArray[i] >= pivotElement; i++) {
                }
                while (num >= startElementNo && sortArray[num] < pivotElement) {
                    num--;
                }
            }
            if (i > num) {
                break;
            }
            int num2 = sortArray[i];
            sortArray[i] = sortArray[num];
            sortArray[num] = num2;
            i++;
            num--;
        }
        return i;
    }
    public static void QuickSort(float[] sortArray, bool _isAscendingOrder) {
        isAscendingOrder = _isAscendingOrder;
        QuickSort(sortArray, 0, sortArray.Length - 1);
    }
    private static void QuickSort(float[] sortArray, int startElementNo, int endElementNo) {
        if (startElementNo != endElementNo) {
            int num = Pivot(sortArray, startElementNo, endElementNo);
            if (num != -1) {
                int num2 = Partition(sortArray, startElementNo, endElementNo, sortArray[num]);
                QuickSort(sortArray, startElementNo, num2 - 1);
                QuickSort(sortArray, num2, endElementNo);
            }
        }
    }
    private static int Pivot(float[] a, int i, int j) {
        int k;
        for (k = i + 1; k <= j && a[i] == a[k]; k++) {
        }
        if (k > j) {
            return -1;
        }
        if (a[i] >= a[k]) {
            return i;
        }
        return k;
    }
    private static int Partition(float[] sortArray, int startElementNo, int endElementNo, float pivotElement) {
        int i = startElementNo;
        int num = endElementNo;
        while (i <= num) {
            if (isAscendingOrder) {
                for (; i <= endElementNo && sortArray[i] < pivotElement; i++) {
                }
                while (num >= startElementNo && sortArray[num] >= pivotElement) {
                    num--;
                }
            } else {
                for (; i <= endElementNo && sortArray[i] >= pivotElement; i++) {
                }
                while (num >= startElementNo && sortArray[num] < pivotElement) {
                    num--;
                }
            }
            if (i > num) {
                break;
            }
            float num2 = sortArray[i];
            sortArray[i] = sortArray[num];
            sortArray[num] = num2;
            i++;
            num--;
        }
        return i;
    }
    public static void QuickSort(string[] sortArray, bool _isAscendingOrder) {
        isAscendingOrder = _isAscendingOrder;
        QuickSort(sortArray, 0, sortArray.Length - 1);
    }
    private static void QuickSort(string[] sortArray, int startElementNo, int endElementNo) {
        if (startElementNo != endElementNo) {
            int num = Pivot(sortArray, startElementNo, endElementNo);
            if (num != -1) {
                int num2 = Partition(sortArray, startElementNo, endElementNo, sortArray[num]);
                QuickSort(sortArray, startElementNo, num2 - 1);
                QuickSort(sortArray, num2, endElementNo);
            }
        }
    }
    private static int Pivot(string[] a, int i, int j) {
        int k;
        for (k = i + 1; k <= j && a[i] == a[k]; k++) {
        }
        if (k > j) {
            return -1;
        }
        if (float.Parse(a[i]) >= float.Parse(a[k])) {
            return i;
        }
        return k;
    }
    private static int Partition(string[] sortArray, int startElementNo, int endElementNo, string pivotElement) {
        int i = startElementNo;
        int num = endElementNo;
        while (i <= num) {
            if (isAscendingOrder) {
                for (; i <= endElementNo && float.Parse(sortArray[i]) < float.Parse(pivotElement); i++) {
                }
                while (num >= startElementNo && float.Parse(sortArray[num]) >= float.Parse(pivotElement)) {
                    num--;
                }
            } else {
                for (; i <= endElementNo && float.Parse(sortArray[i]) >= float.Parse(pivotElement); i++) {
                }
                while (num >= startElementNo && float.Parse(sortArray[num]) < float.Parse(pivotElement)) {
                    num--;
                }
            }
            if (i > num) {
                break;
            }
            string text = sortArray[i];
            sortArray[i] = sortArray[num];
            sortArray[num] = text;
            i++;
            num--;
        }
        return i;
    }
    public static void QuickSort(int[] sortArray, int[] sortNoArray, bool _isAscendingOrder) {
        isAscendingOrder = _isAscendingOrder;
        QuickSort(sortArray, sortNoArray, 0, sortArray.Length - 1);
    }
    private static void QuickSort(int[] sortArray, int[] sortNoArray, int startElementNo, int endElementNo) {
        if (startElementNo != endElementNo) {
            int num = Pivot(sortArray, startElementNo, endElementNo);
            if (num != -1) {
                int num2 = Partition(sortArray, sortNoArray, startElementNo, endElementNo, sortArray[num]);
                QuickSort(sortArray, sortNoArray, startElementNo, num2 - 1);
                QuickSort(sortArray, sortNoArray, num2, endElementNo);
            }
        }
    }
    private static int Partition(int[] sortArray, int[] sortNoArray, int startElementNo, int endElementNo, int pivotElement) {
        int i = startElementNo;
        int num = endElementNo;
        while (i <= num) {
            if (isAscendingOrder) {
                for (; i <= endElementNo && sortArray[i] < pivotElement; i++) {
                }
                while (num >= startElementNo && sortArray[num] >= pivotElement) {
                    num--;
                }
            } else {
                for (; i <= endElementNo && sortArray[i] >= pivotElement; i++) {
                }
                while (num >= startElementNo && sortArray[num] < pivotElement) {
                    num--;
                }
            }
            if (i > num) {
                break;
            }
            int num2 = sortArray[i];
            sortArray[i] = sortArray[num];
            sortArray[num] = num2;
            int num3 = sortNoArray[i];
            sortNoArray[i] = sortNoArray[num];
            sortNoArray[num] = num3;
            i++;
            num--;
        }
        return i;
    }
    public static void QuickSort(float[] sortArray, int[] sortNoArray, bool _isAscendingOrder) {
        isAscendingOrder = _isAscendingOrder;
        QuickSort(sortArray, sortNoArray, 0, sortArray.Length - 1);
    }
    private static void QuickSort(float[] sortArray, int[] sortNoArray, int startElementNo, int endElementNo) {
        if (startElementNo != endElementNo) {
            int num = Pivot(sortArray, startElementNo, endElementNo);
            if (num != -1) {
                int num2 = Partition(sortArray, sortNoArray, startElementNo, endElementNo, sortArray[num]);
                QuickSort(sortArray, sortNoArray, startElementNo, num2 - 1);
                QuickSort(sortArray, sortNoArray, num2, endElementNo);
            }
        }
    }
    private static int Partition(float[] sortArray, int[] sortNoArray, int startElementNo, int endElementNo, float pivotElement) {
        int i = startElementNo;
        int num = endElementNo;
        while (i <= num) {
            if (isAscendingOrder) {
                for (; i <= endElementNo && sortArray[i] < pivotElement; i++) {
                }
                while (num >= startElementNo && sortArray[num] >= pivotElement) {
                    num--;
                }
            } else {
                for (; i <= endElementNo && sortArray[i] >= pivotElement; i++) {
                }
                while (num >= startElementNo && sortArray[num] < pivotElement) {
                    num--;
                }
            }
            if (i > num) {
                break;
            }
            float num2 = sortArray[i];
            sortArray[i] = sortArray[num];
            sortArray[num] = num2;
            int num3 = sortNoArray[i];
            sortNoArray[i] = sortNoArray[num];
            sortNoArray[num] = num3;
            i++;
            num--;
        }
        return i;
    }
    public static void QuickSort(string[] sortArray, int[] sortNoArray, bool _isAscendingOrder) {
        isAscendingOrder = _isAscendingOrder;
        QuickSort(sortArray, sortNoArray, 0, sortArray.Length - 1);
    }
    private static void QuickSort(string[] sortArray, int[] sortNoArray, int startElementNo, int endElementNo) {
        if (startElementNo != endElementNo) {
            int num = Pivot(sortArray, startElementNo, endElementNo);
            if (num != -1) {
                int num2 = Partition(sortArray, sortNoArray, startElementNo, endElementNo, sortArray[num]);
                QuickSort(sortArray, sortNoArray, startElementNo, num2 - 1);
                QuickSort(sortArray, sortNoArray, num2, endElementNo);
            }
        }
    }
    private static int Partition(string[] sortArray, int[] sortNoArray, int startElementNo, int endElementNo, string pivotElement) {
        int i = startElementNo;
        int num = endElementNo;
        while (i <= num) {
            if (isAscendingOrder) {
                for (; i <= endElementNo && float.Parse(sortArray[i]) < float.Parse(pivotElement); i++) {
                }
                while (num >= startElementNo && float.Parse(sortArray[num]) >= float.Parse(pivotElement)) {
                    num--;
                }
            } else {
                for (; i <= endElementNo && float.Parse(sortArray[i]) >= float.Parse(pivotElement); i++) {
                }
                while (num >= startElementNo && float.Parse(sortArray[num]) < float.Parse(pivotElement)) {
                    num--;
                }
            }
            if (i > num) {
                break;
            }
            string text = sortArray[i];
            sortArray[i] = sortArray[num];
            sortArray[num] = text;
            int num2 = sortNoArray[i];
            sortNoArray[i] = sortNoArray[num];
            sortNoArray[num] = num2;
            i++;
            num--;
        }
        return i;
    }
    public static float ConvertRecordStringToTime(string _recordStr) {
        float result = 3599.99f;
        string[] array = _recordStr.Split(':', '.');
        if (array.Length != 3) {
            return result;
        }
        int result2 = 0;
        int result3 = 0;
        int result4 = 0;
        if (!int.TryParse(array[0], out result2)) {
            return result;
        }
        if (!int.TryParse(array[1], out result3)) {
            return result;
        }
        if (!int.TryParse(array[2], out result4)) {
            return result;
        }
        if (result2 > 59 || result3 > 59 || result4 > 99) {
            return result;
        }
        return (float)(result2 * 60 + result3) + (float)result4 * 0.01f;
    }
    public static string ConvertTimeToRecordString(float _secondTime, int _no = 0) {
        if (_secondTime > 599.99f || _secondTime < 0f) {
            return "9:59.99";
        }
        int num = Mathf.FloorToInt(_secondTime) / 60;
        int num2 = Mathf.FloorToInt(_secondTime) % 60;
        int num3 = Mathf.FloorToInt(_secondTime * 100f) % 100;
        arrayRecordStirng[_no] = num.ToString("0") + ":" + num2.ToString("00") + "." + num3.ToString("00");
        return arrayRecordStirng[_no];
    }
    public static string GetConvertRecordString(int _no) {
        return arrayRecordStirng[_no];
    }
    public static float ConvertDecimalFirst(float _time) {
        return Mathf.Floor(_time * 10f) / 10f;
    }
    public static float ConvertDecimalSecond(float _time) {
        return Mathf.Floor(_time * 100f) / 100f;
    }
}
