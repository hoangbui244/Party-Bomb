using UnityEngine;
public class CharacterCursorAnimation : MonoBehaviour {
    [SerializeField]
    [Header("カ\u30fcソル画像")]
    private SpriteRenderer cursorCircle;
    [SerializeField]
    [Header("矢印画像")]
    private SpriteRenderer arrow;
    [SerializeField]
    [Header("ベ\u30fcスサイズ")]
    private Vector3 baseScale;
    private const float SCALE_ANI_SPEED = 6f;
    private float scaleAniTime;
    private Transform targetTransform;
    private float groundY;
    private void Update() {
        Vector3 eulerAngle = base.transform.rotation.eulerAngles;
        base.transform.SetEulerAngles(0f, base.transform.rotation.eulerAngles.y, 0f);
        scaleAniTime += Time.deltaTime * 6f;
        cursorCircle.transform.localScale = baseScale * (1.12f + Mathf.Sin(scaleAniTime) * 0.12f);
        if (targetTransform != null) {
            base.transform.SetPosition(targetTransform.position.x, groundY, targetTransform.position.z);
            base.transform.SetLocalEulerAngles(0f, 0f, 0f);
        }
    }
    public void SetCharacter(Transform _chara) {
        if (_chara != null) {
            targetTransform = _chara;
            groundY = _chara.position.y - 0.0535f;
            base.transform.SetPosition(_chara.position.x, groundY, _chara.position.z);
            base.transform.SetLocalEulerAngles(0f, 0f, 0f);
        }
    }
    public void Show(bool _show) {
        base.gameObject.SetActive(_show);
    }
    public void SetArrowPos(bool _haveBall) {
        if (_haveBall) {
            arrow.transform.SetLocalPositionZ(1.5f);
        } else {
            arrow.transform.SetLocalPositionZ(1.3f);
        }
    }
    public void ShowArrow(bool _show) {
        arrow.gameObject.SetActive(_show);
    }
    public void ShowCircle(bool _show) {
        cursorCircle.gameObject.SetActive(_show);
    }
    public void ShowCircleAlpha(bool _haveBall) {
        cursorCircle.color = new Color(cursorCircle.color.r, cursorCircle.color.g, cursorCircle.color.b, _haveBall ? 1f : 0.5f);
    }
    public void SetColor(int _playerNo) {
        arrow.sprite = SingletonCustom<SAManager>.Instance.GetSA(SAType.Common).GetSprite("tex_move_arrow_" + _playerNo.ToString());
        cursorCircle.sprite = SingletonCustom<SAManager>.Instance.GetSA(SAType.Common).GetSprite("tex_cursor_" + _playerNo.ToString());
    }
}
