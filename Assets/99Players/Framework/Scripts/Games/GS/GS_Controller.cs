using UnityEngine;
public class GS_Controller : MonoBehaviour {
    public enum Type {
        GameMode,
        Inning,
        MatchTime,
        Simple
    }
    [SerializeField]
    [Header("表示ル\u30fcト")]
    private GameObject objRoot;
    [SerializeField]
    [Header("フレ\u30fcム画像")]
    private SpriteRenderer frame;
    private readonly float FRAME_SIZE_X_GAMEMODE = 946f;
    private readonly float FRAME_SIZE_X_INNING = 855f;
    private readonly float FRAME_SIZE_X_MATCH_TIME = 822f;
    private readonly float FRAME_SIZE_X_SIMPLE = 501f;
    private readonly float POS_DEFAULT_CONTROLLER = 618f;
    private readonly float POS_DEFAULT_OPERATION = -811f;
    private readonly float POS_OUT_CONTROLLER = 1400f;
    private readonly float POS_OUT_OPERATION = -1200f;
    public void Set(GS_Define.GameType _type) {
        base.gameObject.SetActive(value: true);
        LeanTween.cancel(objRoot);
        objRoot.transform.SetLocalScaleY(0f);
        LeanTween.scaleY(objRoot, 1f, 0.25f).setIgnoreTimeScale(useUnScaledTime: true);
        if (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay) {
            switch (_type) {
                default:
                    SetFrame(Type.Simple);
                    break;
                case GS_Define.GameType.CANNON_SHOT:
                    SetFrame(Type.GameMode);
                    break;
                case GS_Define.GameType.MOLE_HAMMER:
                    SetFrame(Type.Inning);
                    break;
                case GS_Define.GameType.GET_BALL:
                    SetFrame(Type.MatchTime);
                    break;
            }
        } else {
            switch (_type) {
                default:
                    SetFrame(Type.Simple);
                    break;
                case GS_Define.GameType.BLOCK_WIPER:
                case GS_Define.GameType.RECEIVE_PON:
                case GS_Define.GameType.BLOW_AWAY_TANK:
                    SetFrame(Type.GameMode);
                    break;
                case GS_Define.GameType.MOLE_HAMMER:
                    SetFrame(Type.Inning);
                    break;
                case GS_Define.GameType.GET_BALL:
                    SetFrame(Type.MatchTime);
                    break;
            }
        }
    }
    public void Close() {
        base.gameObject.SetActive(value: false);
    }
    private void SetFrame(Type _type) {
    }
    private void OnDestroy() {
        LeanTween.cancel(objRoot);
    }
}
