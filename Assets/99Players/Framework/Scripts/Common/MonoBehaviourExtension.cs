using System;
using System.Collections;
using UnityEngine;
public class MonoBehaviourExtension : MonoBehaviour {
    public void WaitAfterExec(float _wait, Action _act, bool _isTimeIgnore = false) {
        if (_isTimeIgnore) {
            StartCoroutine(_WaitAfterExecTimeIgnore(_wait, _act));
        } else {
            StartCoroutine(_WaitAfterExec(_wait, _act));
        }
    }
    private static IEnumerator _WaitAfterExec(float _wait, Action _act) {
        yield return new WaitForSeconds(_wait);
        _act?.Invoke();
    }
    private static IEnumerator _WaitAfterExecTimeIgnore(float _wait, Action _act) {
        yield return new WaitForSecondsRealtime(_wait);
        _act?.Invoke();
    }
    public void StopExec() {
        StopAllCoroutines();
    }
}
