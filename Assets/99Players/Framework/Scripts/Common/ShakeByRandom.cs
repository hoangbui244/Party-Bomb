using UnityEngine;
public class ShakeByRandom : MonoBehaviour {
    private struct ShakeInfo {
        public float Duration {
            get;
        }
        public float Strength {
            get;
        }
        public float Vibrato {
            get;
        }
        public ShakeInfo(float duration, float strength, float vibrato) {
            Duration = duration;
            Strength = strength;
            Vibrato = vibrato;
        }
    }
    private ShakeInfo _shakeInfo;
    private Vector3 _initPosition;
    private bool _isDoShake;
    private float _totalShakeTime;
    private void Start() {
        _initPosition = base.gameObject.transform.position;
    }
    private void Update() {
        if (_isDoShake) {
            base.gameObject.transform.position = UpdateShakePosition(base.gameObject.transform.position, _shakeInfo, _totalShakeTime, _initPosition);
            _totalShakeTime += Time.deltaTime;
            if (_totalShakeTime >= _shakeInfo.Duration) {
                _isDoShake = false;
                _totalShakeTime = 0f;
                base.gameObject.transform.position = _initPosition;
            }
        }
    }
    private Vector3 UpdateShakePosition(Vector3 currentPosition, ShakeInfo shakeInfo, float totalTime, Vector3 initPosition) {
        float strength = shakeInfo.Strength;
        float num = UnityEngine.Random.Range(-1f * strength, strength);
        float num2 = UnityEngine.Random.Range(-1f * strength, strength);
        Vector3 vector = currentPosition;
        vector.x += num;
        vector.y += num2;
        float vibrato = shakeInfo.Vibrato;
        float num3 = 1f - totalTime / shakeInfo.Duration;
        vibrato *= num3;
        vector.x = Mathf.Clamp(vector.x, initPosition.x - vibrato, initPosition.x + vibrato);
        vector.y = Mathf.Clamp(vector.y, initPosition.y - vibrato, initPosition.y + vibrato);
        return vector;
    }
    public void StartShake(float duration, float strength, float vibrato) {
        _shakeInfo = new ShakeInfo(duration, strength, vibrato);
        _isDoShake = true;
        _totalShakeTime = 0f;
    }
}
