using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class ScreenRatio : MonoBehaviour
{
    [SerializeField] RectTransform center;
    [SerializeField] List<RawRender> rawRenders = new List<RawRender>();

    private void Start()
    {
        SetRawSizeRatio();
    }

    public void SetRawSizeRatio()
    {
        if (Screen.width > Screen.height)
        {
            return;
        }


        for (int i = 0; i < rawRenders.Count; i++)
        {
            RectTransform pivot = rawRenders[i].pivot;
            float maxDimension = Mathf.Max(pivot.rect.width, pivot.rect.height);
            float textureWidth = rawRenders[i].texture.rect.width;
            float scaleFactor = maxDimension / textureWidth;

            if (i == 0)
            {
                GameManager.Instance.Horizontal = GameManager.Instance.Horizontal * (textureWidth / maxDimension);
            }

            rawRenders[i].texture.localScale = Vector3.one * scaleFactor;
        }
    }

    [System.Serializable]
    public class RawRender
    {
        public RectTransform texture;
        public RectTransform pivot;
    }
}
