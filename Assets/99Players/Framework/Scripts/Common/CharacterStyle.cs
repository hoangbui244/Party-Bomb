using System.Collections.Generic;
using UnityEngine;
public class CharacterStyle : MonoBehaviour {
    public class StyleData {
        public StyleTextureManager.GenderType gender;
        public StyleTextureManager.FaceType face;
        public StyleTextureManager.HairColorType hairColor;
        public StyleTextureManager.FacialType facial;
        public ShapeType shape;
        public int texId;
        public StyleData() {
            gender = (StyleTextureManager.GenderType)UnityEngine.Random.Range(0, 2);
            face = StyleTextureManager.FaceType.TYPE_A;
            if (UnityEngine.Random.Range(0, 100) < 60) {
                face = (StyleTextureManager.FaceType)UnityEngine.Random.Range(1, 4);
            }
            hairColor = (StyleTextureManager.HairColorType)UnityEngine.Random.Range(0, 3);
            shape = (ShapeType)UnityEngine.Random.Range(0, 3);
            facial = StyleTextureManager.FacialType.DEFAULT;
            switch (gender) {
                case StyleTextureManager.GenderType.BOY:
                    texId = UnityEngine.Random.Range(0, SingletonCustom<StyleTextureManager>.Instance.GetBaseTextureTotal(StyleTextureManager.GenderType.BOY));
                    break;
                case StyleTextureManager.GenderType.GIRL:
                    texId = UnityEngine.Random.Range(0, SingletonCustom<StyleTextureManager>.Instance.GetBaseTextureTotal(StyleTextureManager.GenderType.GIRL));
                    break;
            }
        }
        public StyleData(StyleData _copy) {
            gender = _copy.gender;
            face = _copy.face;
            hairColor = _copy.hairColor;
            shape = _copy.shape;
            facial = _copy.facial;
            texId = _copy.texId;
        }
    }
    public enum ShapeType {
        NORMAL,
        FAT,
        SLIM
    }
    private enum MeshType {
        Hip,
        Body,
        Leg_L,
        Leg_R,
        Shoulder_L,
        Shoulder_R,
        Arm_L,
        Arm_R
    }
    public enum CapType {
        CAP,
        SUNVISOR,
        HAT
    }
    public enum TeacherType {
        DATE,
        IEYASU,
        HIME,
        HIMEBU,
        HIDEYOSHI,
        HOUJYOU,
        MOURI
    }
    public enum MainCharacterType {
        BOY1,
        GIAL,
        BOY2,
        BOY3,
        BOY4,
        GIAL2,
        BOY5,
        BOY6,
        AUTO
    }
    [SerializeField]
    [Header("基本マテリアル（※FaceAddOn対応Shader）")]
    private Material baseMat;
    [SerializeField]
    [Header("マテリアル適用対象のMeshRenderer")]
    private MeshRenderer[] arrayTargetMeshRenderer;
    [SerializeField]
    [Header("体格メッシュ（Normal）")]
    private Mesh[] arrayNormalMesh;
    [SerializeField]
    [Header("体格メッシュ（Fat）")]
    private Mesh[] arrayFatMesh;
    [SerializeField]
    [Header("体格メッシュ（Slim）")]
    private Mesh[] arraySlimMesh;
    [SerializeField]
    [Header("帽子パ\u30fcツ")]
    private GameObject[] arrayCapParts;
    [SerializeField]
    [Header("先生用のマテリアル")]
    private Material[] teacherMaterial;
    [SerializeField]
    [Header("メインキャラ用のマテリアル")]
    private Material[] mainCharactersMaterial;
    [SerializeField]
    [Header("ビブス用のマテリアル")]
    private Material[] arrayBibsMat;
    [SerializeField]
    [Header("ゲ\u30fcム別プリセット")]
    private CharacterStyleModelSet[] arrayPriset;
    [SerializeField]
    [Header("ビ\u30fcチサッカ\u30fc：キ\u30fcパ\u30fc用マテリアル")]
    private Material[] arrayBeachSoccerKeeperMat;
    [SerializeField]
    [Header("観客設定")]
    private bool isAudienceSetup;
    [SerializeField]
    [Header("衣装固定")]
    private bool isFixCostume;
    private Transform capParent;
    private Material styleMat;
    private StyleData currentStyle;
    private Color hairColorBlack = new Color32(0, 0, 0, byte.MaxValue);
    private Color hairColorBrown = new Color32(169, 110, 22, byte.MaxValue);
    private Color hairColorDarkBrown = new Color32(115, 55, 14, byte.MaxValue);
    private Material[] mats;
    private static MaterialPropertyBlock block;
    private static int mainTexId;
    private static int faceTexId;
    private static int hairTexId;
    private static int hairColorId;
    public StyleData CurrentData => currentStyle;
    public Material StyleMat => arrayTargetMeshRenderer[0].material;
    private void Awake() {
        if (isAudienceSetup) {
            StyleData styleData = new StyleData();
            SetStyle(styleData.gender, styleData.face, styleData.hairColor, styleData.shape, styleData.texId, UnityEngine.Random.Range(0, SingletonCustom<GameSettingManager>.Instance.TeamNum));
        }
    }
    public void ChnageStyle(StyleData _data) {
        if (currentStyle == null) {
            return;
        }
        currentStyle = new StyleData(_data);
        for (int i = 0; i < arrayTargetMeshRenderer.Length; i++) {
            mats = arrayTargetMeshRenderer[i].materials;
            for (int j = 0; j < mats.Length; j++) {
                mats[j] = styleMat;
            }
            arrayTargetMeshRenderer[i].materials = mats;
        }
        if (block == null) {
            block = new MaterialPropertyBlock();
            mainTexId = Shader.PropertyToID("_MainTex");
            faceTexId = Shader.PropertyToID("_FaceTex");
            hairTexId = Shader.PropertyToID("_HairTex");
            hairColorId = Shader.PropertyToID("_HairColor");
        }
        block.SetTexture(mainTexId, SingletonCustom<StyleTextureManager>.Instance.GetBaseTexture(currentStyle.gender, currentStyle.texId));
        block.SetTexture(faceTexId, SingletonCustom<StyleTextureManager>.Instance.GetFaceTex(currentStyle.gender, currentStyle.face));
        block.SetTexture(hairTexId, SingletonCustom<StyleTextureManager>.Instance.GetHairTex(currentStyle.gender));
        switch (currentStyle.hairColor) {
            case StyleTextureManager.HairColorType.BLACK:
                block.SetColor(hairTexId, hairColorBlack);
                break;
            case StyleTextureManager.HairColorType.BROWN:
                block.SetColor(hairTexId, hairColorBrown);
                break;
            case StyleTextureManager.HairColorType.DARK_BROWN:
                block.SetColor(hairTexId, hairColorDarkBrown);
                break;
        }
        for (int k = 0; k < arrayTargetMeshRenderer.Length; k++) {
            arrayTargetMeshRenderer[k].SetPropertyBlock(block);
        }
        for (int l = 0; l < arrayTargetMeshRenderer.Length; l++) {
            if (arrayTargetMeshRenderer[l].name.Contains("Hip")) {
                switch (currentStyle.shape) {
                    case ShapeType.NORMAL:
                        arrayTargetMeshRenderer[l].GetComponent<MeshFilter>().mesh = arrayNormalMesh[0];
                        break;
                    case ShapeType.FAT:
                        arrayTargetMeshRenderer[l].GetComponent<MeshFilter>().mesh = arrayFatMesh[0];
                        break;
                    case ShapeType.SLIM:
                        arrayTargetMeshRenderer[l].GetComponent<MeshFilter>().mesh = arraySlimMesh[0];
                        break;
                }
            }
            if (arrayTargetMeshRenderer[l].name.Contains("Body")) {
                switch (currentStyle.shape) {
                    case ShapeType.NORMAL:
                        arrayTargetMeshRenderer[l].GetComponent<MeshFilter>().mesh = arrayNormalMesh[1];
                        break;
                    case ShapeType.FAT:
                        arrayTargetMeshRenderer[l].GetComponent<MeshFilter>().mesh = arrayFatMesh[1];
                        break;
                    case ShapeType.SLIM:
                        arrayTargetMeshRenderer[l].GetComponent<MeshFilter>().mesh = arraySlimMesh[1];
                        break;
                }
            }
            if (arrayTargetMeshRenderer[l].name.Contains("Leg_L")) {
                switch (currentStyle.shape) {
                    case ShapeType.NORMAL:
                        arrayTargetMeshRenderer[l].GetComponent<MeshFilter>().mesh = arrayNormalMesh[2];
                        break;
                    case ShapeType.FAT:
                        arrayTargetMeshRenderer[l].GetComponent<MeshFilter>().mesh = arrayFatMesh[2];
                        break;
                    case ShapeType.SLIM:
                        arrayTargetMeshRenderer[l].GetComponent<MeshFilter>().mesh = arraySlimMesh[2];
                        break;
                }
            }
            if (arrayTargetMeshRenderer[l].name.Contains("Leg_R")) {
                switch (currentStyle.shape) {
                    case ShapeType.NORMAL:
                        arrayTargetMeshRenderer[l].GetComponent<MeshFilter>().mesh = arrayNormalMesh[3];
                        break;
                    case ShapeType.FAT:
                        arrayTargetMeshRenderer[l].GetComponent<MeshFilter>().mesh = arrayFatMesh[3];
                        break;
                    case ShapeType.SLIM:
                        arrayTargetMeshRenderer[l].GetComponent<MeshFilter>().mesh = arraySlimMesh[3];
                        break;
                }
            }
            if (arrayTargetMeshRenderer[l].name.Contains("Shoulder_L")) {
                switch (currentStyle.shape) {
                    case ShapeType.NORMAL:
                        arrayTargetMeshRenderer[l].GetComponent<MeshFilter>().mesh = arrayNormalMesh[4];
                        break;
                    case ShapeType.FAT:
                        arrayTargetMeshRenderer[l].GetComponent<MeshFilter>().mesh = arrayFatMesh[4];
                        break;
                    case ShapeType.SLIM:
                        arrayTargetMeshRenderer[l].GetComponent<MeshFilter>().mesh = arraySlimMesh[4];
                        break;
                }
            }
            if (arrayTargetMeshRenderer[l].name.Contains("Shoulder_R")) {
                switch (currentStyle.shape) {
                    case ShapeType.NORMAL:
                        arrayTargetMeshRenderer[l].GetComponent<MeshFilter>().mesh = arrayNormalMesh[5];
                        break;
                    case ShapeType.FAT:
                        arrayTargetMeshRenderer[l].GetComponent<MeshFilter>().mesh = arrayFatMesh[5];
                        break;
                    case ShapeType.SLIM:
                        arrayTargetMeshRenderer[l].GetComponent<MeshFilter>().mesh = arraySlimMesh[5];
                        break;
                }
            }
            if (arrayTargetMeshRenderer[l].name.Contains("Arm_L")) {
                switch (currentStyle.shape) {
                    case ShapeType.NORMAL:
                        arrayTargetMeshRenderer[l].GetComponent<MeshFilter>().mesh = arrayNormalMesh[6];
                        break;
                    case ShapeType.FAT:
                        arrayTargetMeshRenderer[l].GetComponent<MeshFilter>().mesh = arrayFatMesh[6];
                        break;
                    case ShapeType.SLIM:
                        arrayTargetMeshRenderer[l].GetComponent<MeshFilter>().mesh = arraySlimMesh[6];
                        break;
                }
            }
            if (arrayTargetMeshRenderer[l].name.Contains("Arm_R")) {
                switch (currentStyle.shape) {
                    case ShapeType.NORMAL:
                        arrayTargetMeshRenderer[l].GetComponent<MeshFilter>().mesh = arrayNormalMesh[7];
                        break;
                    case ShapeType.FAT:
                        arrayTargetMeshRenderer[l].GetComponent<MeshFilter>().mesh = arrayFatMesh[7];
                        break;
                    case ShapeType.SLIM:
                        arrayTargetMeshRenderer[l].GetComponent<MeshFilter>().mesh = arraySlimMesh[7];
                        break;
                }
            }
        }
    }
    public void SetStyle(StyleTextureManager.GenderType _genderType, StyleTextureManager.FaceType _faceType, StyleTextureManager.HairColorType _hairColor, ShapeType _shape, int _texIdx = -1, int _teamNo = 0) {
        currentStyle = new StyleData();
        currentStyle.gender = _genderType;
        currentStyle.face = _faceType;
        currentStyle.hairColor = _hairColor;
        currentStyle.shape = _shape;
        currentStyle.texId = _texIdx;
        styleMat = new Material(baseMat);
        if (block == null) {
            block = new MaterialPropertyBlock();
            mainTexId = Shader.PropertyToID("_MainTex");
            faceTexId = Shader.PropertyToID("_FaceTex");
            hairTexId = Shader.PropertyToID("_HairTex");
            hairColorId = Shader.PropertyToID("_HairColor");
        }
        styleMat.SetTexture(mainTexId, SingletonCustom<StyleTextureManager>.Instance.GetBaseTexture(currentStyle.gender, currentStyle.texId));
        styleMat.SetTexture(faceTexId, SingletonCustom<StyleTextureManager>.Instance.GetFaceTex(currentStyle.gender, currentStyle.face));
        styleMat.SetTexture(hairTexId, SingletonCustom<StyleTextureManager>.Instance.GetHairTex(currentStyle.gender));
        switch (currentStyle.hairColor) {
            case StyleTextureManager.HairColorType.BLACK:
                styleMat.SetColor(hairColorId, hairColorBlack);
                break;
            case StyleTextureManager.HairColorType.BROWN:
                styleMat.SetColor(hairColorId, hairColorBrown);
                break;
            case StyleTextureManager.HairColorType.DARK_BROWN:
                styleMat.SetColor(hairColorId, hairColorDarkBrown);
                break;
        }
        for (int i = 0; i < arrayTargetMeshRenderer.Length; i++) {
            mats = arrayTargetMeshRenderer[i].materials;
            for (int j = 0; j < mats.Length; j++) {
                if (isAudienceSetup && isFixCostume) {
                    if (arrayTargetMeshRenderer[i].name.Contains("Head")) {
                        mats[j] = styleMat;
                    }
                } else {
                    mats[j] = styleMat;
                }
            }
            arrayTargetMeshRenderer[i].materials = mats;
        }
        for (int k = 0; k < arrayTargetMeshRenderer.Length; k++) {
            if (arrayTargetMeshRenderer[k].name.Contains("Hip")) {
                switch (currentStyle.shape) {
                    case ShapeType.NORMAL:
                        arrayTargetMeshRenderer[k].GetComponent<MeshFilter>().mesh = arrayNormalMesh[0];
                        break;
                    case ShapeType.FAT:
                        arrayTargetMeshRenderer[k].GetComponent<MeshFilter>().mesh = arrayFatMesh[0];
                        break;
                    case ShapeType.SLIM:
                        arrayTargetMeshRenderer[k].GetComponent<MeshFilter>().mesh = arraySlimMesh[0];
                        break;
                }
            }
            if (arrayTargetMeshRenderer[k].name.Contains("Body")) {
                switch (currentStyle.shape) {
                    case ShapeType.NORMAL:
                        arrayTargetMeshRenderer[k].GetComponent<MeshFilter>().mesh = arrayNormalMesh[1];
                        break;
                    case ShapeType.FAT:
                        arrayTargetMeshRenderer[k].GetComponent<MeshFilter>().mesh = arrayFatMesh[1];
                        break;
                    case ShapeType.SLIM:
                        arrayTargetMeshRenderer[k].GetComponent<MeshFilter>().mesh = arraySlimMesh[1];
                        break;
                }
            }
            if (arrayTargetMeshRenderer[k].name.Contains("Shoulder_L")) {
                switch (currentStyle.shape) {
                    case ShapeType.NORMAL:
                        arrayTargetMeshRenderer[k].GetComponent<MeshFilter>().mesh = arrayNormalMesh[4];
                        break;
                    case ShapeType.FAT:
                        arrayTargetMeshRenderer[k].GetComponent<MeshFilter>().mesh = arrayFatMesh[4];
                        break;
                    case ShapeType.SLIM:
                        arrayTargetMeshRenderer[k].GetComponent<MeshFilter>().mesh = arraySlimMesh[4];
                        break;
                }
            }
            if (arrayTargetMeshRenderer[k].name.Contains("Shoulder_R")) {
                switch (currentStyle.shape) {
                    case ShapeType.NORMAL:
                        arrayTargetMeshRenderer[k].GetComponent<MeshFilter>().mesh = arrayNormalMesh[5];
                        break;
                    case ShapeType.FAT:
                        arrayTargetMeshRenderer[k].GetComponent<MeshFilter>().mesh = arrayFatMesh[5];
                        break;
                    case ShapeType.SLIM:
                        arrayTargetMeshRenderer[k].GetComponent<MeshFilter>().mesh = arraySlimMesh[5];
                        break;
                }
            }
            if (arrayTargetMeshRenderer[k].name.Contains("Arm_L")) {
                switch (currentStyle.shape) {
                    case ShapeType.NORMAL:
                        arrayTargetMeshRenderer[k].GetComponent<MeshFilter>().mesh = arrayNormalMesh[6];
                        break;
                    case ShapeType.FAT:
                        arrayTargetMeshRenderer[k].GetComponent<MeshFilter>().mesh = arrayFatMesh[6];
                        break;
                    case ShapeType.SLIM:
                        arrayTargetMeshRenderer[k].GetComponent<MeshFilter>().mesh = arraySlimMesh[6];
                        break;
                }
            }
            if (arrayTargetMeshRenderer[k].name.Contains("Arm_R")) {
                switch (currentStyle.shape) {
                    case ShapeType.NORMAL:
                        arrayTargetMeshRenderer[k].GetComponent<MeshFilter>().mesh = arrayNormalMesh[7];
                        break;
                    case ShapeType.FAT:
                        arrayTargetMeshRenderer[k].GetComponent<MeshFilter>().mesh = arrayFatMesh[7];
                        break;
                    case ShapeType.SLIM:
                        arrayTargetMeshRenderer[k].GetComponent<MeshFilter>().mesh = arraySlimMesh[7];
                        break;
                }
            }
        }
        for (int l = 0; l < arrayPriset.Length; l++) {
            GameObject[] addDefaultModel = arrayPriset[l].addDefaultModel;
            for (int m = 0; m < addDefaultModel.Length; m++) {
                addDefaultModel[m].SetActive(value: false);
            }
        }
    }
    public void SetTeacherStyle(TeacherType _teacherType) {
        currentStyle = new StyleData();
        switch (_teacherType) {
            case TeacherType.DATE:
                currentStyle.shape = ShapeType.NORMAL;
                for (int n = 0; n < arrayCapParts.Length; n++) {
                    arrayCapParts[n].SetActive(value: false);
                }
                break;
            case TeacherType.IEYASU:
                currentStyle.shape = ShapeType.FAT;
                for (int j = 0; j < arrayCapParts.Length; j++) {
                    arrayCapParts[j].SetActive(value: false);
                }
                break;
            case TeacherType.HIME:
                currentStyle.shape = ShapeType.SLIM;
                for (int l = 0; l < arrayCapParts.Length; l++) {
                    arrayCapParts[l].SetActive(value: false);
                }
                break;
            case TeacherType.HIMEBU:
                currentStyle.shape = ShapeType.NORMAL;
                for (int num = 0; num < arrayCapParts.Length; num++) {
                    arrayCapParts[num].SetActive(num == 1);
                }
                break;
            case TeacherType.HIDEYOSHI:
                currentStyle.shape = ShapeType.NORMAL;
                for (int m = 0; m < arrayCapParts.Length; m++) {
                    arrayCapParts[m].SetActive(value: false);
                }
                break;
            case TeacherType.HOUJYOU:
                currentStyle.shape = ShapeType.SLIM;
                for (int k = 0; k < arrayCapParts.Length; k++) {
                    arrayCapParts[k].SetActive(k == 2);
                }
                break;
            case TeacherType.MOURI:
                currentStyle.shape = ShapeType.NORMAL;
                for (int i = 0; i < arrayCapParts.Length; i++) {
                    arrayCapParts[i].SetActive(value: false);
                }
                break;
        }
        for (int num2 = 0; num2 < arrayTargetMeshRenderer.Length; num2++) {
            mats = arrayTargetMeshRenderer[num2].materials;
            for (int num3 = 0; num3 < mats.Length; num3++) {
                mats[num3] = teacherMaterial[(int)_teacherType];
            }
            arrayTargetMeshRenderer[num2].materials = mats;
        }
        for (int num4 = 0; num4 < arrayTargetMeshRenderer.Length; num4++) {
            if (arrayTargetMeshRenderer[num4].name.Contains("Hip")) {
                switch (currentStyle.shape) {
                    case ShapeType.NORMAL:
                        arrayTargetMeshRenderer[num4].GetComponent<MeshFilter>().mesh = arrayNormalMesh[0];
                        break;
                    case ShapeType.FAT:
                        arrayTargetMeshRenderer[num4].GetComponent<MeshFilter>().mesh = arrayFatMesh[0];
                        break;
                    case ShapeType.SLIM:
                        arrayTargetMeshRenderer[num4].GetComponent<MeshFilter>().mesh = arraySlimMesh[0];
                        break;
                }
            }
            if (arrayTargetMeshRenderer[num4].name.Contains("Body")) {
                switch (currentStyle.shape) {
                    case ShapeType.NORMAL:
                        arrayTargetMeshRenderer[num4].GetComponent<MeshFilter>().mesh = arrayNormalMesh[1];
                        break;
                    case ShapeType.FAT:
                        arrayTargetMeshRenderer[num4].GetComponent<MeshFilter>().mesh = arrayFatMesh[1];
                        break;
                    case ShapeType.SLIM:
                        arrayTargetMeshRenderer[num4].GetComponent<MeshFilter>().mesh = arraySlimMesh[1];
                        break;
                }
            }
            if (arrayTargetMeshRenderer[num4].name.Contains("Leg_L")) {
                switch (currentStyle.shape) {
                    case ShapeType.NORMAL:
                        arrayTargetMeshRenderer[num4].GetComponent<MeshFilter>().mesh = arrayNormalMesh[2];
                        break;
                    case ShapeType.FAT:
                        arrayTargetMeshRenderer[num4].GetComponent<MeshFilter>().mesh = arrayFatMesh[2];
                        break;
                    case ShapeType.SLIM:
                        arrayTargetMeshRenderer[num4].GetComponent<MeshFilter>().mesh = arraySlimMesh[2];
                        break;
                }
            }
            if (arrayTargetMeshRenderer[num4].name.Contains("Leg_R")) {
                switch (currentStyle.shape) {
                    case ShapeType.NORMAL:
                        arrayTargetMeshRenderer[num4].GetComponent<MeshFilter>().mesh = arrayNormalMesh[3];
                        break;
                    case ShapeType.FAT:
                        arrayTargetMeshRenderer[num4].GetComponent<MeshFilter>().mesh = arrayFatMesh[3];
                        break;
                    case ShapeType.SLIM:
                        arrayTargetMeshRenderer[num4].GetComponent<MeshFilter>().mesh = arraySlimMesh[3];
                        break;
                }
            }
            if (arrayTargetMeshRenderer[num4].name.Contains("Shoulder_L")) {
                switch (currentStyle.shape) {
                    case ShapeType.NORMAL:
                        arrayTargetMeshRenderer[num4].GetComponent<MeshFilter>().mesh = arrayNormalMesh[4];
                        break;
                    case ShapeType.FAT:
                        arrayTargetMeshRenderer[num4].GetComponent<MeshFilter>().mesh = arrayFatMesh[4];
                        break;
                    case ShapeType.SLIM:
                        arrayTargetMeshRenderer[num4].GetComponent<MeshFilter>().mesh = arraySlimMesh[4];
                        break;
                }
            }
            if (arrayTargetMeshRenderer[num4].name.Contains("Shoulder_R")) {
                switch (currentStyle.shape) {
                    case ShapeType.NORMAL:
                        arrayTargetMeshRenderer[num4].GetComponent<MeshFilter>().mesh = arrayNormalMesh[5];
                        break;
                    case ShapeType.FAT:
                        arrayTargetMeshRenderer[num4].GetComponent<MeshFilter>().mesh = arrayFatMesh[5];
                        break;
                    case ShapeType.SLIM:
                        arrayTargetMeshRenderer[num4].GetComponent<MeshFilter>().mesh = arraySlimMesh[5];
                        break;
                }
            }
            if (arrayTargetMeshRenderer[num4].name.Contains("Arm_L")) {
                switch (currentStyle.shape) {
                    case ShapeType.NORMAL:
                        arrayTargetMeshRenderer[num4].GetComponent<MeshFilter>().mesh = arrayNormalMesh[6];
                        break;
                    case ShapeType.FAT:
                        arrayTargetMeshRenderer[num4].GetComponent<MeshFilter>().mesh = arrayFatMesh[6];
                        break;
                    case ShapeType.SLIM:
                        arrayTargetMeshRenderer[num4].GetComponent<MeshFilter>().mesh = arraySlimMesh[6];
                        break;
                }
            }
            if (arrayTargetMeshRenderer[num4].name.Contains("Arm_R")) {
                switch (currentStyle.shape) {
                    case ShapeType.NORMAL:
                        arrayTargetMeshRenderer[num4].GetComponent<MeshFilter>().mesh = arrayNormalMesh[7];
                        break;
                    case ShapeType.FAT:
                        arrayTargetMeshRenderer[num4].GetComponent<MeshFilter>().mesh = arrayFatMesh[7];
                        break;
                    case ShapeType.SLIM:
                        arrayTargetMeshRenderer[num4].GetComponent<MeshFilter>().mesh = arraySlimMesh[7];
                        break;
                }
            }
        }
    }
    public void GameStyleModelReset() {
        for (int i = 0; i < GS_Define.CHARACTER_MAX; i++) {
            for (int j = 0; j < arrayPriset[i].addMakingPotionModel.Length; j++) {
                arrayPriset[i].addMakingPotionModel[j].SetActive(value: false);
            }
        }
    }
    public void SetGameStyle(GS_Define.GameType _type, int _playerNo, int _teamNo = -1) {
        int num = 0;
        UnityEngine.Debug.Log("_playerNo:" + _playerNo.ToString());
        if (_playerNo < SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx.Length) {
            num = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[_playerNo];
        } else {
            num = GS_Define.CHARACTER_MAX;
            UnityEngine.Debug.Log("!max:");
        }
        GameObject[] array = null;
        UnityEngine.Debug.Log("charIdx:" + num.ToString());
        switch (_type) {
            case GS_Define.GameType.GET_BALL:
                array = arrayPriset[num].addSpearBattleModel;
                break;
            case GS_Define.GameType.CANNON_SHOT:
                array = arrayPriset[num].addArcherBattleModel;
                break;
            case GS_Define.GameType.BLOCK_WIPER:
                array = arrayPriset[num].addLegendarySwordModel;
                break;
            case GS_Define.GameType.MOLE_HAMMER:
                array = arrayPriset[num].addMonsterRaceModel;
                break;
            case GS_Define.GameType.BOMB_ROULETTE:
                array = arrayPriset[num].addMakingPotionModel;
                break;
            case GS_Define.GameType.RECEIVE_PON:
                array = arrayPriset[num].addMonsterKillModel;
                break;
            case GS_Define.GameType.DELIVERY_ORDER:
                array = arrayPriset[num].addBlackSmithModel;
                break;
            case GS_Define.GameType.ARCHER_BATTLE:
                array = arrayPriset[num].addBigMerchantModel;
                break;
            case GS_Define.GameType.ATTACK_BALL:
                array = arrayPriset[num].addDragonBattleModel;
                break;
            case GS_Define.GameType.BLOW_AWAY_TANK:
                array = arrayPriset[num].addArenaBattleModel;
                break;
        }
        GameObject[] array2 = array;
        foreach (GameObject gameObject in array2) {
            gameObject.SetActive(value: true);
            MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
            if (!component) {
                continue;
            }
            mats = component.materials;
            for (int j = 0; j < mats.Length; j++) {
                if (gameObject.name.Contains("Bibs")) {
                    mats[j] = arrayBibsMat[num];
                } else {
                    mats[j] = arrayPriset[num].arrayGameTypeMat[(int)_type];
                }
            }
        }
        MeshRenderer[] array3 = arrayTargetMeshRenderer;
        foreach (MeshRenderer meshRenderer in array3) {
            mats = meshRenderer.materials;
            for (int k = 0; k < mats.Length; k++) {
                mats[k] = arrayPriset[num].arrayGameTypeMat[(int)_type];
            }
            meshRenderer.materials = mats;
        }
    }
    public MeshRenderer[] GetMeshList(GS_Define.GameType _type, int _playerNo) {
        int num = 0;
        num = ((_playerNo >= SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx.Length) ? GS_Define.CHARACTER_MAX : SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[_playerNo]);
        GameObject[] array = null;
        List<MeshRenderer> list = new List<MeshRenderer>();
        switch (_type) {
            case GS_Define.GameType.GET_BALL:
                array = arrayPriset[num].addSpearBattleModel;
                break;
            case GS_Define.GameType.CANNON_SHOT:
                array = arrayPriset[num].addArcherBattleModel;
                break;
            case GS_Define.GameType.BLOCK_WIPER:
                array = arrayPriset[num].addLegendarySwordModel;
                break;
            case GS_Define.GameType.MOLE_HAMMER:
                array = arrayPriset[num].addMonsterRaceModel;
                break;
            case GS_Define.GameType.BOMB_ROULETTE:
                array = arrayPriset[num].addMakingPotionModel;
                break;
            case GS_Define.GameType.RECEIVE_PON:
                array = arrayPriset[num].addMonsterKillModel;
                break;
            case GS_Define.GameType.DELIVERY_ORDER:
                array = arrayPriset[num].addBlackSmithModel;
                break;
            case GS_Define.GameType.ARCHER_BATTLE:
                array = arrayPriset[num].addBigMerchantModel;
                break;
            case GS_Define.GameType.ATTACK_BALL:
                array = arrayPriset[num].addDragonBattleModel;
                break;
            case GS_Define.GameType.BLOW_AWAY_TANK:
                array = arrayPriset[num].addArenaBattleModel;
                break;
        }
        GameObject[] array2 = array;
        foreach (GameObject obj in array2) {
            obj.SetActive(value: true);
            MeshRenderer component = obj.GetComponent<MeshRenderer>();
            if ((bool)component) {
                list.Add(component);
            }
        }
        MeshRenderer[] array3 = arrayTargetMeshRenderer;
        foreach (MeshRenderer item in array3) {
            list.Add(item);
        }
        return list.ToArray();
    }
    public void SetKeeper(int _type) {
        for (int i = 0; i < arrayTargetMeshRenderer.Length; i++) {
            mats = arrayTargetMeshRenderer[i].materials;
            for (int j = 0; j < mats.Length; j++) {
                mats[j] = arrayBeachSoccerKeeperMat[_type];
            }
            arrayTargetMeshRenderer[i].materials = mats;
        }
    }
    public void SetBeachSoccerColor(int _colorNo) {
        for (int i = 0; i < arrayTargetMeshRenderer.Length; i++) {
            mats = arrayTargetMeshRenderer[i].materials;
            if (!arrayTargetMeshRenderer[i].name.Contains("Head")) {
                for (int j = 0; j < mats.Length; j++) {
                    mats[j] = arrayPriset[_colorNo].arrayGameTypeMat[8];
                }
            }
            arrayTargetMeshRenderer[i].materials = mats;
        }
    }
    public void SetMainCharacterStyle(int _playerNo, int _teamNo = -1, bool _isAddModel = true) {
        currentStyle = new StyleData();
        int num = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[_playerNo];
        UnityEngine.Debug.Log("☆chariDx:" + num.ToString() + " playerNo:" + _playerNo.ToString());
        if (_teamNo != -1 && !SingletonCustom<GameSettingManager>.Instance.IsSinglePlay && SingletonCustom<GameSettingManager>.Instance.SelectGameFormat != GS_Define.GameFormat.COOP && SingletonCustom<GameSettingManager>.Instance.PlayerNum != 2 && SingletonCustom<GameSettingManager>.Instance.PlayerNum != 3 && SingletonCustom<GameSettingManager>.Instance.PlayerNum == 4) {
            GS_Define.GameFormat selectGameFormat = SingletonCustom<GameSettingManager>.Instance.SelectGameFormat;
        }
        for (int i = 0; i < arrayTargetMeshRenderer.Length; i++) {
            mats = arrayTargetMeshRenderer[i].materials;
            for (int j = 0; j < mats.Length; j++) {
                mats[j] = mainCharactersMaterial[num];
            }
            arrayTargetMeshRenderer[i].materials = mats;
        }
        DisableAddModel();
        if (_isAddModel) {
            GameObject[] addDefaultModel = arrayPriset[num].addDefaultModel;
            foreach (GameObject obj in addDefaultModel) {
                obj.SetActive(value: true);
                MeshRenderer component = obj.GetComponent<MeshRenderer>();
                if ((bool)component) {
                    mats = component.materials;
                    for (int l = 0; l < mats.Length; l++) {
                        mats[l] = mainCharactersMaterial[num];
                    }
                    component.materials = mats;
                }
            }
        }
        currentStyle.shape = ShapeType.NORMAL;
        for (int m = 0; m < arrayTargetMeshRenderer.Length; m++) {
            if (arrayTargetMeshRenderer[m].name.Contains("Hip")) {
                switch (currentStyle.shape) {
                    case ShapeType.NORMAL:
                        arrayTargetMeshRenderer[m].GetComponent<MeshFilter>().mesh = arrayNormalMesh[0];
                        break;
                    case ShapeType.FAT:
                        arrayTargetMeshRenderer[m].GetComponent<MeshFilter>().mesh = arrayFatMesh[0];
                        break;
                    case ShapeType.SLIM:
                        arrayTargetMeshRenderer[m].GetComponent<MeshFilter>().mesh = arraySlimMesh[0];
                        break;
                }
            }
            if (arrayTargetMeshRenderer[m].name.Contains("Body")) {
                switch (currentStyle.shape) {
                    case ShapeType.NORMAL:
                        arrayTargetMeshRenderer[m].GetComponent<MeshFilter>().mesh = arrayNormalMesh[1];
                        break;
                    case ShapeType.FAT:
                        arrayTargetMeshRenderer[m].GetComponent<MeshFilter>().mesh = arrayFatMesh[1];
                        break;
                    case ShapeType.SLIM:
                        arrayTargetMeshRenderer[m].GetComponent<MeshFilter>().mesh = arraySlimMesh[1];
                        break;
                }
            }
            if (arrayTargetMeshRenderer[m].name.Contains("Leg_L")) {
                switch (currentStyle.shape) {
                    case ShapeType.NORMAL:
                        arrayTargetMeshRenderer[m].GetComponent<MeshFilter>().mesh = arrayNormalMesh[2];
                        break;
                    case ShapeType.FAT:
                        arrayTargetMeshRenderer[m].GetComponent<MeshFilter>().mesh = arrayFatMesh[2];
                        break;
                    case ShapeType.SLIM:
                        arrayTargetMeshRenderer[m].GetComponent<MeshFilter>().mesh = arraySlimMesh[2];
                        break;
                }
            }
            if (arrayTargetMeshRenderer[m].name.Contains("Leg_R")) {
                switch (currentStyle.shape) {
                    case ShapeType.NORMAL:
                        arrayTargetMeshRenderer[m].GetComponent<MeshFilter>().mesh = arrayNormalMesh[3];
                        break;
                    case ShapeType.FAT:
                        arrayTargetMeshRenderer[m].GetComponent<MeshFilter>().mesh = arrayFatMesh[3];
                        break;
                    case ShapeType.SLIM:
                        arrayTargetMeshRenderer[m].GetComponent<MeshFilter>().mesh = arraySlimMesh[3];
                        break;
                }
            }
            if (arrayTargetMeshRenderer[m].name.Contains("Shoulder_L")) {
                switch (currentStyle.shape) {
                    case ShapeType.NORMAL:
                        arrayTargetMeshRenderer[m].GetComponent<MeshFilter>().mesh = arrayNormalMesh[4];
                        break;
                    case ShapeType.FAT:
                        arrayTargetMeshRenderer[m].GetComponent<MeshFilter>().mesh = arrayFatMesh[4];
                        break;
                    case ShapeType.SLIM:
                        arrayTargetMeshRenderer[m].GetComponent<MeshFilter>().mesh = arraySlimMesh[4];
                        break;
                }
            }
            if (arrayTargetMeshRenderer[m].name.Contains("Shoulder_R")) {
                switch (currentStyle.shape) {
                    case ShapeType.NORMAL:
                        arrayTargetMeshRenderer[m].GetComponent<MeshFilter>().mesh = arrayNormalMesh[5];
                        break;
                    case ShapeType.FAT:
                        arrayTargetMeshRenderer[m].GetComponent<MeshFilter>().mesh = arrayFatMesh[5];
                        break;
                    case ShapeType.SLIM:
                        arrayTargetMeshRenderer[m].GetComponent<MeshFilter>().mesh = arraySlimMesh[5];
                        break;
                }
            }
            if (arrayTargetMeshRenderer[m].name.Contains("Arm_L")) {
                switch (currentStyle.shape) {
                    case ShapeType.NORMAL:
                        arrayTargetMeshRenderer[m].GetComponent<MeshFilter>().mesh = arrayNormalMesh[6];
                        break;
                    case ShapeType.FAT:
                        arrayTargetMeshRenderer[m].GetComponent<MeshFilter>().mesh = arrayFatMesh[6];
                        break;
                    case ShapeType.SLIM:
                        arrayTargetMeshRenderer[m].GetComponent<MeshFilter>().mesh = arraySlimMesh[6];
                        break;
                }
            }
            if (arrayTargetMeshRenderer[m].name.Contains("Arm_R")) {
                switch (currentStyle.shape) {
                    case ShapeType.NORMAL:
                        arrayTargetMeshRenderer[m].GetComponent<MeshFilter>().mesh = arrayNormalMesh[7];
                        break;
                    case ShapeType.FAT:
                        arrayTargetMeshRenderer[m].GetComponent<MeshFilter>().mesh = arrayFatMesh[7];
                        break;
                    case ShapeType.SLIM:
                        arrayTargetMeshRenderer[m].GetComponent<MeshFilter>().mesh = arraySlimMesh[7];
                        break;
                }
            }
        }
        SetMainCharacterFaceDiff(_playerNo, StyleTextureManager.MainCharacterFaceType.NORMAL);
    }
    public void DisableAddModel() {
        for (int i = 0; i < mainCharactersMaterial.Length; i++) {
            GameObject[] addDefaultModel = arrayPriset[i].addDefaultModel;
            for (int j = 0; j < addDefaultModel.Length; j++) {
                addDefaultModel[j].SetActive(value: false);
            }
        }
    }
    public void SetMainCharacterFaceDiff(int _playerNo, StyleTextureManager.MainCharacterFaceType _type) {
        if (SingletonCustom<GameSettingManager>.Instance.PlayerNum == 3 && SingletonCustom<GameSettingManager>.Instance.SelectGameFormat == GS_Define.GameFormat.COOP && _playerNo == 8) {
            _playerNo = 3;
        }
        int charIdx = SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[_playerNo];
        for (int i = 0; i < arrayTargetMeshRenderer.Length; i++) {
            if (arrayTargetMeshRenderer[i].name.Contains("Head")) {
                mats = arrayTargetMeshRenderer[i].materials;
                for (int j = 0; j < mats.Length; j++) {
                    mats[j] = SingletonCustom<StyleTextureManager>.Instance.GetMainCharacterFaceMat(charIdx, _type);
                }
                arrayTargetMeshRenderer[i].materials = mats;
            }
        }
    }
    public void SetStyleFixShoulderPos(StyleTextureManager.GenderType _genderType, StyleTextureManager.FaceType _faceType, StyleTextureManager.HairColorType _hairColor, ShapeType _shape, int _texIdx = -1) {
        SetStyle(_genderType, _faceType, _hairColor, _shape, _texIdx);
        for (int i = 0; i < arrayTargetMeshRenderer.Length; i++) {
            if (arrayTargetMeshRenderer[i].name.Contains("Shoulder_L")) {
                switch (currentStyle.shape) {
                    case ShapeType.NORMAL:
                        arrayTargetMeshRenderer[i].transform.SetLocalPositionX(-0.152f);
                        break;
                    case ShapeType.FAT:
                        arrayTargetMeshRenderer[i].transform.SetLocalPositionX(-0.152f);
                        break;
                    case ShapeType.SLIM:
                        arrayTargetMeshRenderer[i].transform.SetLocalPositionX(-0.135f);
                        break;
                }
            }
            if (arrayTargetMeshRenderer[i].name.Contains("Shoulder_R")) {
                switch (currentStyle.shape) {
                    case ShapeType.NORMAL:
                        arrayTargetMeshRenderer[i].transform.SetLocalPositionX(0.152f);
                        break;
                    case ShapeType.FAT:
                        arrayTargetMeshRenderer[i].transform.SetLocalPositionX(0.152f);
                        break;
                    case ShapeType.SLIM:
                        arrayTargetMeshRenderer[i].transform.SetLocalPositionX(0.135f);
                        break;
                }
            }
        }
    }
    public void SetFacial(StyleTextureManager.FacialType _facialType) {
        if (styleMat == null) {
            return;
        }
        currentStyle.facial = _facialType;
        styleMat.SetTexture("_FaceTex", SingletonCustom<StyleTextureManager>.Instance.GetFacialTex(currentStyle.gender, currentStyle.face, currentStyle.facial));
        for (int i = 0; i < arrayTargetMeshRenderer.Length; i++) {
            mats = arrayTargetMeshRenderer[i].materials;
            for (int j = 0; j < mats.Length; j++) {
                mats[j] = styleMat;
            }
            arrayTargetMeshRenderer[i].materials = mats;
        }
    }
    public void DebugMainCharacterStyle(int _charaNo) {
        currentStyle = new StyleData();
        int num = _charaNo;
        UnityEngine.Debug.Log("☆chariDx:" + num.ToString());
        for (int i = 0; i < arrayTargetMeshRenderer.Length; i++) {
            mats = arrayTargetMeshRenderer[i].materials;
            for (int j = 0; j < mats.Length; j++) {
                mats[j] = mainCharactersMaterial[num];
            }
            arrayTargetMeshRenderer[i].materials = mats;
        }
        for (int k = 0; k < arrayPriset.Length; k++) {
            GameObject[] addDefaultModel = arrayPriset[k].addDefaultModel;
            for (int l = 0; l < addDefaultModel.Length; l++) {
                addDefaultModel[l].SetActive(k == num);
            }
        }
        currentStyle.shape = ShapeType.NORMAL;
    }
    public void DebugCharacterFaceDiff(int _charaNo, StyleTextureManager.MainCharacterFaceType _type) {
        for (int i = 0; i < arrayTargetMeshRenderer.Length; i++) {
            if (arrayTargetMeshRenderer[i].name.Contains("Head")) {
                mats = arrayTargetMeshRenderer[i].materials;
                for (int j = 0; j < mats.Length; j++) {
                    mats[j] = SingletonCustom<StyleTextureManager>.Instance.GetMainCharacterFaceMat(_charaNo, _type);
                }
                arrayTargetMeshRenderer[i].materials = mats;
            }
        }
    }
}
