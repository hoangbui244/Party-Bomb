using UnityEngine;
public class CommonPlayerNo : MonoBehaviour {
    [SerializeField]
    [Header("プレイヤ\u30fc番号画像")]
    public SpriteRenderer spPlayerNo;
    public void Init(int _playerNo, bool _isSingle) {
        if (_isSingle) {
            spPlayerNo.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_c_you");
        } else {
            spPlayerNo.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_c_" + (_playerNo + 1).ToString() + "P");
        }
    }
}
