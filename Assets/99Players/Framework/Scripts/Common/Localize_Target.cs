using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Localize_Target : MonoBehaviour {
    public enum TargetType {
        SpriteRenderer,
        TextMeshPro,
        Image,
        Object,
        MeshRenderer
    }
    public TargetType targetType;
    public bool isChangePos;
    public bool isChangePos_World;
    public bool isNoneSetting;
    public Vector3 changePos;
    public bool isChangeSize;
    public bool isChangeScaleSize;
    public Vector3 changeSize_Scale;
    public Vector2 changeSize_SizeData;
    public SpriteRenderer target_Sprite;
    public Sprite changeSprite;
    public Vector2 changeSize_Sliced;
    public TextMeshPro target_Text;
    public float changeSize_Font;
    public bool isChangeSizeData;
    public bool isChangeSpacingOptions;
    public float changeSpacing_Character;
    public float changeSpacing_Word;
    public float changeSpacing_Line;
    public float changeSpacing_Paragraph;
    public bool isChangeAlignment;
    public TextAlignmentOptions changeAlignment;
    public Image target_Image;
    public Sprite changeImageSprite;
    public GameObject target_Object;
    public MeshRenderer target_Mesh;
    public bool isChangeMaterial;
    public Material changeMaterial;
    public bool isChangeTexture;
    public Texture changeTexture;
    public void Set() {
        switch (targetType) {
            case TargetType.SpriteRenderer:
                SetSpriteRenderer();
                break;
            case TargetType.TextMeshPro:
                SetTextMeshPro();
                break;
            case TargetType.Image:
                SetImage();
                break;
            case TargetType.Object:
                SetObject();
                break;
            case TargetType.MeshRenderer:
                SetMeshRenderer();
                break;
        }
    }
    private void SetSpriteRenderer() {
        if (!(target_Sprite != null)) {
            return;
        }
        if (changeSprite != null) {
            target_Sprite.sprite = changeSprite;
        }
        if (isNoneSetting && changeSprite == null) {
            target_Sprite.sprite = null;
        }
        if (isChangePos) {
            if (isChangePos_World) {
                target_Sprite.transform.position = changePos;
            } else {
                target_Sprite.transform.localPosition = changePos;
            }
        }
        if (isChangeSize) {
            if (isChangeScaleSize) {
                target_Sprite.transform.localScale = changeSize_Scale;
            } else {
                target_Sprite.size = changeSize_Sliced;
            }
        }
    }
    private void SetTextMeshPro() {
        if (!(target_Text != null)) {
            return;
        }
        if (isChangePos) {
            if (isChangePos_World) {
                target_Text.transform.position = changePos;
            } else {
                target_Text.transform.localPosition = changePos;
            }
        }
        if (isChangeSize) {
            if (isChangeScaleSize) {
                target_Text.transform.localScale = changeSize_Scale;
            } else {
                target_Text.fontSize = changeSize_Font;
            }
        }
        if (isChangeSizeData) {
            target_Text.rectTransform.sizeDelta = changeSize_SizeData;
        }
        if (isChangeSpacingOptions) {
            target_Text.characterSpacing = changeSpacing_Character;
            target_Text.wordSpacing = changeSpacing_Word;
            target_Text.lineSpacing = changeSpacing_Line;
            target_Text.paragraphSpacing = changeSpacing_Paragraph;
        }
        if (isChangeAlignment) {
            target_Text.alignment = changeAlignment;
        }
    }
    private void SetImage() {
        if (!(target_Image != null)) {
            return;
        }
        if (changeImageSprite != null) {
            target_Image.sprite = changeImageSprite;
        }
        if (isChangePos) {
            if (isChangePos_World) {
                target_Image.transform.position = changePos;
            } else {
                target_Image.transform.localPosition = changePos;
            }
        }
        if (isChangeSize) {
            if (isChangeScaleSize) {
                target_Image.transform.localScale = changeSize_Scale;
            } else {
                target_Image.rectTransform.sizeDelta = changeSize_SizeData;
            }
        }
    }
    private void SetObject() {
        if (!(target_Object != null)) {
            return;
        }
        if (isChangePos) {
            if (isChangePos_World) {
                target_Object.transform.position = changePos;
            } else {
                target_Object.transform.localPosition = changePos;
            }
        }
        if (isChangeSize) {
            target_Object.transform.localScale = changeSize_Scale;
        }
    }
    private void SetMeshRenderer() {
        if (!(target_Mesh != null)) {
            return;
        }
        string text = "en_";
        if (isChangeMaterial && changeMaterial != null) {
            string name = changeMaterial.name;
            name = name.Substring(name.IndexOf(text) + text.Length, name.Length - text.Length);
            Material[] materials = target_Mesh.materials;
            for (int i = 0; i < materials.Length; i++) {
                if (materials[i].name.Contains(name)) {
                    materials[i] = changeMaterial;
                    break;
                }
            }
            target_Mesh.materials = materials;
        }
        if (isChangeTexture && changeTexture != null) {
            string name2 = changeTexture.name;
            name2 = name2.Substring(name2.IndexOf(text) + text.Length, name2.Length - text.Length);
            Material[] materials2 = target_Mesh.materials;
            for (int j = 0; j < materials2.Length; j++) {
                if (materials2[j].mainTexture.name.Contains(name2)) {
                    materials2[j].mainTexture = changeTexture;
                    break;
                }
            }
            target_Mesh.materials = materials2;
        }
        if (isChangePos) {
            if (isChangePos_World) {
                target_Mesh.transform.position = changePos;
            } else {
                target_Mesh.transform.localPosition = changePos;
            }
        }
        if (isChangeSize) {
            target_Mesh.transform.localScale = changeSize_Scale;
        }
    }
}
