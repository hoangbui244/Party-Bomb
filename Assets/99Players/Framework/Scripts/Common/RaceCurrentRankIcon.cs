using UnityEngine;
public class RaceCurrentRankIcon : MonoBehaviour {
    [SerializeField]
    [Header("順位アイコン")]
    private SpriteRenderer rankIcon;
    private Vector3 rankIconOriginScale;
    public void Init() {
        rankIconOriginScale = rankIcon.transform.localScale;
        rankIcon.transform.SetLocalScale(0f, 0f, 1f);
    }
    public void SetRankIcon(int _rankNum) {
        rankIcon.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Common, "_common_rank_s_" + _rankNum.ToString());
    }
    public void SetRankIconScaling(float _scaleTime) {
        float x = rankIcon.transform.localScale.x;
        float value = Mathf.Clamp(rankIcon.transform.localScale.x + _scaleTime, 0f, 1f);
        rankIcon.transform.SetLocalScale(Mathf.Clamp(value, 0f, x), Mathf.Clamp(value, 0f, x), 1f);
    }
    public void HideRankIcon() {
        rankIcon.gameObject.SetActive(value: false);
    }
}
