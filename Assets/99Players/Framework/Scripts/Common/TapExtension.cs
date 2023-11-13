using System;
public static class TapExtension {
    public static ref T Tap<T>(ref T obj, Action action) where T : struct {
        action?.Invoke();
        return ref obj;
    }
    public static T Tap<T>(this T obj, Action action) where T : class {
        action?.Invoke();
        return obj;
    }
    public static ref T Tap<T>(ref T obj, Func<T, T> func) where T : struct {
        if (func == null) {
            return ref obj;
        }
        obj = func(obj);
        return ref obj;
    }
    public static T Tap<T>(this T obj, Func<T, T> func) where T : class {
        obj = ((func != null) ? func(obj) : null);
        return obj;
    }
    public static ref T Tap<T>(ref T obj, Action<T> action) where T : struct {
        action?.Invoke(obj);
        return ref obj;
    }
    public static T Tap<T>(this T obj, Action<T> action) where T : class {
        action?.Invoke(obj);
        return obj;
    }
    public static ref T1 Tap<T1, T2>(ref T1 obj, T2 value, Func<T1, T2, T1> func) where T1 : struct {
        if (func == null) {
            return ref obj;
        }
        obj = func(obj, value);
        return ref obj;
    }
    public static T1 Tap<T1, T2>(this T1 obj, T2 value, Action<T1, T2> action) where T1 : class {
        action?.Invoke(obj, value);
        return obj;
    }
}
