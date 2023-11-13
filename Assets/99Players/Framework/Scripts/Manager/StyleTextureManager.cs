using UnityEngine;
public class StyleTextureManager : SingletonCustom<StyleTextureManager> {
    public enum GenderType {
        BOY,
        GIRL
    }
    public enum TexType {
        PLAIN_CLOTH,
        GYM_SUIT,
        JERSEY,
        RANDOM
    }
    public enum FaceType {
        TYPE_A,
        TYPE_B,
        TYPE_C,
        TYPE_D
    }
    public enum FacialType {
        DEFAULT,
        JOY,
        SORROW
    }
    public enum HairColorType {
        BLACK,
        BROWN,
        DARK_BROWN
    }
    public enum MainCharacterFaceType {
        NORMAL,
        HAPPY,
        SAD,
        SMILE
    }
    [SerializeField]
    [Header("テクスチャ[私服：男の子]")]
    private Texture[] arrayTexPlainCloth_Boy;
    [SerializeField]
    [Header("テクスチャ[私服：女の子]")]
    private Texture[] arrayTexPlainCloth_Girl;
    [SerializeField]
    [Header("テクスチャ[体操服]")]
    private Texture[] arrayTexGymSuit;
    [SerializeField]
    [Header("テクスチャ[ジャ\u30fcジ]")]
    private Texture[] arrayTexJersey;
    [SerializeField]
    [Header("テクスチャ[体操服：帽子]")]
    private Texture[] arrayTexGymSuitCap;
    [SerializeField]
    [Header("テクスチャ[髪型：ボ\u30fcイ]")]
    private Texture hairBoy;
    [SerializeField]
    [Header("テクスチャ[髪型：ガ\u30fcル]")]
    private Texture hairGirl;
    [SerializeField]
    [Header("テクスチャ[通常表情:ボ\u30fcイ]")]
    private Texture[] arrayTexFaceNormalBoy;
    [SerializeField]
    [Header("テクスチャ[喜び表情:ボ\u30fcイ]")]
    private Texture[] arrayTexFaceJoyBoy;
    [SerializeField]
    [Header("テクスチャ[悲しみ表情:ボ\u30fcイ]")]
    private Texture[] arrayTexFaceSorrowBoy;
    [SerializeField]
    [Header("テクスチャ[通常表情:ガ\u30fcル]")]
    private Texture[] arrayTexFaceNormalGirl;
    [SerializeField]
    [Header("テクスチャ[喜び表情:ガ\u30fcル]")]
    private Texture[] arrayTexFaceJoyGirl;
    [SerializeField]
    [Header("テクスチャ[悲しみ表情:ガ\u30fcル]")]
    private Texture[] arrayTexFaceSorrowGirl;
    [SerializeField]
    [Header("メインキャラ表情差分マテリアル1")]
    private Material[] arrayMainCharacter1FaceDiff;
    [SerializeField]
    [Header("メインキャラ表情差分マテリアル2")]
    private Material[] arrayMainCharacter2FaceDiff;
    [SerializeField]
    [Header("メインキャラ表情差分マテリアル3")]
    private Material[] arrayMainCharacter3FaceDiff;
    [SerializeField]
    [Header("メインキャラ表情差分マテリアル4")]
    private Material[] arrayMainCharacter4FaceDiff;
    [SerializeField]
    [Header("メインキャラ表情差分マテリアル5")]
    private Material[] arrayMainCharacter5FaceDiff;
    [SerializeField]
    [Header("メインキャラ表情差分マテリアル6")]
    private Material[] arrayMainCharacter6FaceDiff;
    [SerializeField]
    [Header("メインキャラ表情差分マテリアル7")]
    private Material[] arrayMainCharacter7FaceDiff;
    [SerializeField]
    [Header("メインキャラ表情差分マテリアル8")]
    private Material[] arrayMainCharacter8FaceDiff;
    private int[] arrayBoyTexId;
    private int[] arrayGirlTexId;
    private int texBoyUseIdx;
    private int texGirlUseIdx;
    private int tempIdx;
    private TexType texType;
    private TexType randTexType;
    private void Awake() {
        Init();
    }
    public void Init() {
        InitTex(GenderType.BOY);
        InitTex(GenderType.GIRL);
        randTexType = (TexType)Random.Range(0, 3);
    }
    private void InitTex(GenderType _genderType) {
        switch (_genderType) {
            case GenderType.BOY:
                if (arrayBoyTexId == null || arrayBoyTexId.Length == 0) {
                    arrayBoyTexId = new int[arrayTexPlainCloth_Boy.Length];
                }
                for (int j = 0; j < arrayBoyTexId.Length; j++) {
                    arrayBoyTexId[j] = j;
                }
                Shuffle(arrayBoyTexId);
                texBoyUseIdx = 0;
                break;
            case GenderType.GIRL:
                if (arrayGirlTexId == null || arrayGirlTexId.Length == 0) {
                    arrayGirlTexId = new int[arrayTexPlainCloth_Girl.Length];
                }
                for (int i = 0; i < arrayGirlTexId.Length; i++) {
                    arrayGirlTexId[i] = i;
                }
                Shuffle(arrayGirlTexId);
                texGirlUseIdx = 0;
                break;
        }
    }
    public Texture GetBaseTexture(GenderType _genderType, int _texIdx = -1) {
        texType = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.GetStyleSetting();
        if (texType == TexType.RANDOM) {
            texType = randTexType;
        }
        texType = TexType.PLAIN_CLOTH;
        switch (texType) {
            case TexType.PLAIN_CLOTH:
                if (_texIdx == -1) {
                    switch (_genderType) {
                        case GenderType.BOY:
                            if (texBoyUseIdx >= arrayBoyTexId.Length) {
                                InitTex(GenderType.BOY);
                            }
                            tempIdx = arrayBoyTexId[texBoyUseIdx];
                            texBoyUseIdx++;
                            return arrayTexPlainCloth_Boy[tempIdx];
                        case GenderType.GIRL:
                            if (texGirlUseIdx >= arrayGirlTexId.Length) {
                                InitTex(GenderType.GIRL);
                            }
                            tempIdx = arrayGirlTexId[texGirlUseIdx];
                            texGirlUseIdx++;
                            return arrayTexPlainCloth_Girl[tempIdx];
                    }
                } else {
                    switch (_genderType) {
                        case GenderType.BOY:
                            return arrayTexPlainCloth_Boy[Mathf.Clamp(_texIdx, 0, arrayTexPlainCloth_Boy.Length - 1)];
                        case GenderType.GIRL:
                            return arrayTexPlainCloth_Girl[Mathf.Clamp(_texIdx, 0, arrayTexPlainCloth_Girl.Length - 1)];
                    }
                }
                break;
            case TexType.GYM_SUIT:
                return arrayTexGymSuit[Random.Range(0, arrayTexGymSuit.Length)];
            case TexType.JERSEY:
                return arrayTexJersey[(_genderType != 0) ? 1 : 0];
        }
        return null;
    }
    public Texture GetFaceTex(GenderType _genderType, FaceType _faceType) {
        switch (_genderType) {
            case GenderType.BOY:
                return arrayTexFaceNormalBoy[(int)_faceType];
            case GenderType.GIRL:
                return arrayTexFaceNormalGirl[(int)_faceType];
            default:
                return null;
        }
    }
    public Texture GetHairTex(GenderType _genderType) {
        switch (_genderType) {
            case GenderType.BOY:
                return hairBoy;
            case GenderType.GIRL:
                return hairGirl;
            default:
                return null;
        }
    }
    public Texture GetFacialTex(GenderType _genderType, FaceType _faceType, FacialType _facialType) {
        switch (_genderType) {
            case GenderType.BOY:
                switch (_facialType) {
                    case FacialType.DEFAULT:
                        return arrayTexFaceNormalBoy[(int)_faceType];
                    case FacialType.JOY:
                        return arrayTexFaceJoyBoy[(int)_faceType];
                    case FacialType.SORROW:
                        return arrayTexFaceSorrowBoy[(int)_faceType];
                }
                break;
            case GenderType.GIRL:
                switch (_facialType) {
                    case FacialType.DEFAULT:
                        return arrayTexFaceNormalGirl[(int)_faceType];
                    case FacialType.JOY:
                        return arrayTexFaceJoyGirl[(int)_faceType];
                    case FacialType.SORROW:
                        return arrayTexFaceSorrowGirl[(int)_faceType];
                }
                break;
        }
        return null;
    }
    public Material GetMainCharacterFaceMat(int _charIdx, MainCharacterFaceType _type) {
        switch (_charIdx) {
            case 0:
                return arrayMainCharacter1FaceDiff[(int)_type];
            case 1:
                return arrayMainCharacter2FaceDiff[(int)_type];
            case 2:
                return arrayMainCharacter3FaceDiff[(int)_type];
            case 3:
                return arrayMainCharacter4FaceDiff[(int)_type];
            case 4:
                return arrayMainCharacter5FaceDiff[(int)_type];
            case 5:
                return arrayMainCharacter6FaceDiff[(int)_type];
            case 6:
                return arrayMainCharacter7FaceDiff[(int)_type];
            case 7:
                return arrayMainCharacter8FaceDiff[(int)_type];
            default:
                return arrayMainCharacter1FaceDiff[(int)_type];
        }
    }
    public Texture GetCapTex(int _teamNo) {
        return arrayTexGymSuitCap[_teamNo];
    }
    public int GetBaseTextureTotal(GenderType _genderType) {
        texType = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.GetStyleSetting();
        if (texType == TexType.RANDOM) {
            texType = randTexType;
        }
        texType = TexType.PLAIN_CLOTH;
        switch (texType) {
            case TexType.PLAIN_CLOTH:
                switch (_genderType) {
                    case GenderType.BOY:
                        return arrayTexPlainCloth_Boy.Length;
                    case GenderType.GIRL:
                        return arrayTexPlainCloth_Girl.Length;
                }
                break;
            case TexType.GYM_SUIT:
                return arrayTexGymSuit.Length;
            case TexType.JERSEY:
                return arrayTexJersey.Length;
        }
        return arrayTexGymSuit.Length;
    }
    public void Shuffle(int[] arr) {
        for (int num = arr.Length - 1; num > 0; num--) {
            int num2 = Random.Range(0, num + 1);
            int num3 = arr[num];
            arr[num] = arr[num2];
            arr[num2] = num3;
        }
    }
    public TexType GetUseTexType() {
        texType = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.GetStyleSetting();
        if (texType == TexType.RANDOM) {
            texType = randTexType;
        }
        return texType;
    }
}
