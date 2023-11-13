using UnityEngine;

public class AlignSpriteRenderers : MonoBehaviour
{
    public float spacing = 1.0f; // Adjust this to set the spacing between sprites

    void Start()
    {
        AlignSpritesInLine();
    }

    void AlignSpritesInLine()
    {
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        float totalWidth = 0f;

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            totalWidth += spriteRenderer.bounds.size.x;
        }

        totalWidth += (spriteRenderers.Length - 1) * spacing;

        float startX = -totalWidth / 2f;

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            if(spriteRenderer.transform.name != "Square") continue;
            Vector3 newPosition = new Vector3(startX + spriteRenderer.bounds.size.x / 2f, -828f, spriteRenderer.transform.parent.position.z);
            spriteRenderer.transform.parent.localPosition = newPosition;

            startX += spriteRenderer.bounds.size.x + spacing;
        }
    }
}
