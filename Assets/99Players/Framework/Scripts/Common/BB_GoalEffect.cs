using System.Collections;
using TMPro;
using UnityEngine;
public class BB_GoalEffect : MonoBehaviour {
    private float EFFECT_TIME = 3.05f;
    private float effectTime;
    [SerializeField]
    [Header("文字")]
    private ParticleSystem[] text;
    [SerializeField]
    [Header("ゴ\u30fcル文字アンカ\u30fc")]
    private Transform[] goalTextAnchor;
    [SerializeField]
    [Header("文字アンカ\u30fc")]
    private Transform textAnchor;
    [SerializeField]
    [Header("ポジション")]
    private TextMeshPro positionText;
    [SerializeField]
    [Header("背番号")]
    private TextMeshPro uniformNumber;
    [SerializeField]
    [Header("名前")]
    private new TextMeshPro name;
    [SerializeField]
    [Header("オウンゴ\u30fcル")]
    private TextMeshPro ownGoal;
    [SerializeField]
    [Header("シュ\u30fcトキャラ情報アンカ\u30fc")]
    private GameObject shootCharaAnchor;
    [SerializeField]
    [Header("スリ\u30fcポイント")]
    private GameObject threePointShoot;
    private bool isPlay;
    private readonly float GOAL_DEFAULT_POSITION = -138f;
    public void Play(bool _own = false, bool _threePoint = false) {
        if (_own) {
            ownGoal.gameObject.SetActive(value: true);
            shootCharaAnchor.SetActive(value: false);
        }
        LeanTween.moveLocalX(goalTextAnchor[0].gameObject, goalTextAnchor[0].transform.localPosition.x - 1500f, 0.25f).setEaseLinear();
        if (_threePoint) {
            threePointShoot.SetActive(value: true);
            LeanTween.rotate(threePointShoot, new Vector3(1800f, 0f, 0f), 1f).setEaseOutBack();
            for (int i = 0; i < goalTextAnchor.Length; i++) {
                goalTextAnchor[i].AddLocalPositionY(80f);
            }
            textAnchor.AddLocalPositionY(-40f);
        }
        StartCoroutine(_MoveText());
        isPlay = true;
    }
    private IEnumerator _MoveText() {
        float defLocal = textAnchor.localPosition.x;
        LeanTween.moveLocalX(textAnchor.gameObject, 0f, 0.75f).setEaseOutBack();
        yield return new WaitForSeconds(1.75f);
        LeanTween.moveLocalX(textAnchor.gameObject, 0f - defLocal, 0.5f).setEaseOutQuad();
    }
    private void Update() {
        if (isPlay) {
            effectTime += Time.deltaTime;
            if (effectTime >= EFFECT_TIME) {
                UnityEngine.Object.Destroy(base.gameObject);
            }
        }
    }
    private void OnDestroy() {
        LeanTween.cancel(goalTextAnchor[0].gameObject);
    }
}
