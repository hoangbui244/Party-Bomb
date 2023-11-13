using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteRendererFillAmount : MonoBehaviour {
    [SerializeField]
    [HideInInspector]
    private SpriteRenderer _spriteRenderer;
    [SerializeField]
    [Range(0f, 1f)]
    private float _fillAmount = 1f;
    private static readonly int PropertyId = Shader.PropertyToID("_FillAmount");
    private Material _material;
    public float FillAmount {
        get {
            return _fillAmount;
        }
        set {
            _fillAmount = Mathf.Clamp01(value);
            if (_material != null) {
                _material.SetFloat(PropertyId, _fillAmount);
            }
        }
    }
    private void OnValidate() {
        FillAmount = _fillAmount;
    }
    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Shader shader = Shader.Find("Hidden/SpriteRendererFillAmount");
        if (shader == null) {
            UnityEngine.Debug.LogWarning("Shader Not Found");
            return;
        }
        _material = new Material(shader);
        _spriteRenderer.material = _material;
        FillAmount = _fillAmount;
    }
}
