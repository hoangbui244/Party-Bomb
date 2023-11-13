using UnityEngine;
public class CommonAudienceCharacterManager : MonoBehaviour {
    private enum GenderType {
        Boy,
        Girl
    }
    [SerializeField]
    [Header("観客の体のマテリアル（男性）")]
    private Material[] arrayBodyMat_Boy;
    [SerializeField]
    [Header("観客の体のマテリアル（女性）")]
    private Material[] arrayBodyMat_Girl;
    [SerializeField]
    [Header("観客の顔のマテリアル（男性）")]
    private Material[] arrayFaceMat_Boy;
    [SerializeField]
    [Header("観客の顔のマテリアル（女性）")]
    private Material[] arrayFaceMat_Girl;
    public Material[] ArrayBodyMat_Boy => arrayBodyMat_Boy;
    public Material[] ArrayBodyMat_Girl => arrayBodyMat_Girl;
    public Material[] ArrayFaceMat_Boy => arrayFaceMat_Boy;
    public Material[] ArrayFaceMat_Girl => arrayFaceMat_Girl;
    private void Awake() {
        CommonAudienceCharacter[] componentsInChildren = GetComponentsInChildren<CommonAudienceCharacter>();
        if (componentsInChildren == null || componentsInChildren.Length == 0) {
            return;
        }
        GenderType[] array = new GenderType[componentsInChildren.Length];
        for (int i = 0; i < array.Length; i++) {
            array[i] = (GenderType)(i % 2);
        }
        array.Shuffle();
        arrayBodyMat_Boy.Shuffle();
        arrayBodyMat_Girl.Shuffle();
        arrayFaceMat_Boy.Shuffle();
        arrayFaceMat_Girl.Shuffle();
        int num = 0;
        int num2 = 0;
        int num3 = 0;
        int num4 = 0;
        for (int j = 0; j < componentsInChildren.Length; j++) {
            Material bodyMaterial;
            Material faceMaterial;
            if (array[j] == GenderType.Boy) {
                bodyMaterial = arrayBodyMat_Boy[num % arrayBodyMat_Boy.Length];
                num++;
                faceMaterial = arrayFaceMat_Boy[num3 % arrayFaceMat_Boy.Length];
                num3++;
            } else {
                bodyMaterial = arrayBodyMat_Girl[num2 % arrayBodyMat_Girl.Length];
                num2++;
                faceMaterial = arrayFaceMat_Girl[num4 % arrayFaceMat_Girl.Length];
                num4++;
            }
            componentsInChildren[j].SetBodyMaterial(bodyMaterial);
            componentsInChildren[j].SetFaceMaterial(faceMaterial);
        }
    }
}
