using UnityEngine;
using UnityEngine.UI;

public class ScrollViewLimit : MonoBehaviour
{
    public float leftScrollLimit = 200.0f; // Adjust this to set the scrolling limit
    public float rightScrollLimit = 200.0f; // Adjust this to set the scrolling limit
    private ScrollRect scrollRect;

    private void Start()
    {
        scrollRect = GetComponentInParent<ScrollRect>();
    }

    private void LateUpdate()
    {
        Vector2 anchoredPosition = scrollRect.content.anchoredPosition;

        if (anchoredPosition.x > leftScrollLimit)
        {
            anchoredPosition.x = leftScrollLimit;
        }
        else if (anchoredPosition.x < -rightScrollLimit)
        {
            anchoredPosition.x = -rightScrollLimit;
        }

        scrollRect.content.anchoredPosition = anchoredPosition;
    }
}

