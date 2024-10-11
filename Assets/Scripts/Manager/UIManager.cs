using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] Transform centerTrans;
    [SerializeField] List<LineRenderer> lineRenderers;

    Vector3 centerNormal = Vector3.zero;

    public void SetTouchDown(Vector3 normal)
    {
        Vector3 upNormal = centerTrans.up;

        if (Vector3.Dot(upNormal,normal) > 0.2f)
        {
            centerNormal = normal;
        }

        DrawLine(centerTrans.position, centerNormal.normalized);
    }

    public void SetTouchUp()
    {
        for (int i = 0; i < lineRenderers.Count; i++)
        {
            lineRenderers[i].gameObject.SetActive(false);
        }

        centerNormal = Vector3.zero;
    }

    void DrawLine(Vector3 start,Vector3 normal,int lineIndex = 0)
    {
        lineRenderers[lineIndex].SetPosition(0, start);

        int layerMask = (1 << LayerMask.NameToLayer("Wall")) | (1 << LayerMask.NameToLayer("TopWall"));
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(start.x, start.y), new Vector2(normal.x, normal.y), 100f, layerMask);

        if (hit.collider != null)
        {
            lineRenderers[lineIndex].SetPosition(1, hit.point * .95f);
            lineRenderers[lineIndex].gameObject.SetActive(true);

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall") && lineIndex + 1 < lineRenderers.Count)
            {
                normal.x *= -1;
                DrawLine(hit.point * .95f, normal, lineIndex + 1);
            }
            else
            {
                for (int i = lineIndex + 1; i < lineRenderers.Count; i++)
                {
                    lineRenderers[i].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            for (int i = lineIndex + 1; i < lineRenderers.Count; i++)
            {
                lineRenderers[i].gameObject.SetActive(false);
            }
        }
    }
}
