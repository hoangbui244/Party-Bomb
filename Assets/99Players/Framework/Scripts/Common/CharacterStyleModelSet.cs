using UnityEngine;
public class CharacterStyleModelSet : MonoBehaviour {
    [SerializeField]
    [Header("ゲ\u30fcム別マテリアル")]
    public Material[] arrayGameTypeMat;
    [SerializeField]
    [Header("▶[通常]")]
    public GameObject[] addDefaultModel;
    [SerializeField]
    [Header("▶[馬上槍試合]")]
    public GameObject[] addSpearBattleModel;
    [SerializeField]
    [Header("▶[アロ\u30fcディフェンス]")]
    public GameObject[] addArcherBattleModel;
    [SerializeField]
    [Header("▶[伝説の剣]")]
    public GameObject[] addLegendarySwordModel;
    [SerializeField]
    [Header("▶[モンスタ\u30fcレ\u30fcス]")]
    public GameObject[] addMonsterRaceModel;
    [SerializeField]
    [Header("▶[ポ\u30fcションづくり]")]
    public GameObject[] addMakingPotionModel;
    [SerializeField]
    [Header("▶[モンスタ\u30fc討伐]")]
    public GameObject[] addMonsterKillModel;
    [SerializeField]
    [Header("▶[鍛冶職人]")]
    public GameObject[] addBlackSmithModel;
    [SerializeField]
    [Header("▶[運んでアイテム]")]
    public GameObject[] addBigMerchantModel;
    [SerializeField]
    [Header("▶[ドラゴンシュ\u30fcタ\u30fc]")]
    public GameObject[] addDragonBattleModel;
    [SerializeField]
    [Header("▶[バトルコロシアム]")]
    public GameObject[] addArenaBattleModel;
}
