using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenRatio : MonoBehaviour
{
    [SerializeField] List<RawRender> rawRenders = new List<RawRender>();

    private void Start()
    {
        
    }

    public void SetRawSizeRatio()
    {

        for (int i = 0; i < rawRenders.Count; i++)
        {
        }
    }

    [System.Serializable]
    public class RawRender
    {
        [SerializeField] RawImage texture;
        [SerializeField] RectTransform pivot;
    }
}
