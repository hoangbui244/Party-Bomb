using UnityEngine;
using UnityEngine.Rendering;
public class LightingSettings {
    public static void ChangeDefaultLighting() {
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = new Color32(150, 150, 150, byte.MaxValue);
        SetSkybox(null);
        SetQualityLevelByName("SchoolBattle");
    }
    public static void ChangeSceneLighting() {
        ChangeSceneLighting(SingletonCustom<SceneManager>.Instance.GetNowScene());
    }
    public static void ChangeSceneLighting(SceneManager.SceneType _type) {
        Time.fixedDeltaTime = 0.01666f;
        Time.maximumDeltaTime = 0.3333333f;
        Physics.bounceThreshold = 0.3f;
        QualitySettings.vSyncCount = 1;
        switch (_type) {
            case SceneManager.SceneType.TITLE:
            case SceneManager.SceneType.MAIN:
                ChangeDefaultLighting();
                break;
            case SceneManager.SceneType.GET_BALL:
                ChangeDefaultLighting();
                break;
            case SceneManager.SceneType.BLOCK_WIPER:
                ChangeDefaultLighting();
                SetQualityLevelByName("BlockWiper");
                break;
            case SceneManager.SceneType.MOLE_HAMMER:
                ChangeDefaultLighting();
                break;
            case SceneManager.SceneType.BOMB_ROULETTE:
                ChangeDefaultLighting();
                break;
            case SceneManager.SceneType.RECEIVE_PON:
                SetQualityLevelByName("WaterSlider");
                break;
            case SceneManager.SceneType.BLACKSMITH:
                SetQualityLevelByName((SingletonCustom<GameSettingManager>.Instance.PlayerNum >= 2) ? "Biathlon_Low" : "Oni");
                break;
            case SceneManager.SceneType.CANNON_SHOT:
                ChangeDefaultLighting();
                break;
            case SceneManager.SceneType.ARCHER_BATTLE:
            case SceneManager.SceneType.SCROLL_JUMP:
            case SceneManager.SceneType.CLIMB_BLOCK:
            case SceneManager.SceneType.MAKE_SAME_DOT:
                ChangeDefaultLighting();
                break;
            case SceneManager.SceneType.ATTACK_BALL:
            case SceneManager.SceneType.BLOW_AWAY_TANK:
            case SceneManager.SceneType.TIMING_STAMPING:
            case SceneManager.SceneType.BOMB_LIFTING:
            case SceneManager.SceneType.HOME_RUN_DERBY:
            case SceneManager.SceneType.HUNDRED_CHALLENGE:
                SetQualityLevelByName("TimingStamping");
                break;
            case SceneManager.SceneType.JANJAN_FISHING:
                SetQualityLevelByName("JanjanFishing");
                break;
            case SceneManager.SceneType.RESULT_ANNOUNCEMENT:
                SetQualityLevelByName("ResultAnnounce");
                Time.fixedDeltaTime = 0.01666f;
                Time.maximumDeltaTime = 0.01666f;
                break;
            case SceneManager.SceneType.BLOCK_SLICER:
                Time.fixedDeltaTime = 0.01666f;
                Time.maximumDeltaTime = 0.01666f;
                break;
        }
    }
    public static void SetSkybox(Material _skybox) {
        RenderSettings.skybox = _skybox;
    }
    public static void SetFog() {
        Color color = default(Color);
        if (SingletonCustom<SceneManager>.Instance.GetNowScene() == SceneManager.SceneType.BLACKSMITH) {
            RenderSettings.fog = true;
            ColorUtility.TryParseHtmlString("#FF9400", out color);
            RenderSettings.fogColor = color;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.05f;
        } else {
            RenderSettings.fog = false;
        }
    }
    private static void SetQualityLevelByName(string _name) {
        int i = 0;
        string[] names;
        for (names = QualitySettings.names; i < names.Length && _name != names[i]; i++) {
        }
        if (i != names.Length) {
            QualitySettings.SetQualityLevel(i);
        }
    }
}
