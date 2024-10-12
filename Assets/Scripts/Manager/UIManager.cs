using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] Transform centerTrans;
    [SerializeField] List<PanelBase> panels = new List<PanelBase>();
    [SerializeField] List<LineRenderer> lineRenderers;
    [SerializeField] PanelBase initlizePanel;

    Vector3 centerNormal = Vector3.zero;
    PanelBase currentPanel;

    protected override void Awake()
    {
        
    }

    public void Init()
    {
        for (int i = 0; i < panels.Count; i++)
        {
            panels[i].FirstLoad();
        }

        OpenPanel(initlizePanel);
    }

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

        centerNormal = (lineRenderers[0].GetPosition(1) - lineRenderers[0].GetPosition(0)).normalized;
        GameManager.Instance.BubbleShoot(centerNormal);
        centerNormal = Vector3.zero;
    }

    void DrawLine(Vector3 start,Vector3 normal,int lineIndex = 0)
    {
        lineRenderers[lineIndex].SetPosition(0, start);

        int layerMask = (1 << LayerMask.NameToLayer("Wall")) | (1 << LayerMask.NameToLayer("TopWall"));
        float radius = GameManager.Instance.CurrentBubbleRadius;
        RaycastHit2D hit = Physics2D.CircleCast(new Vector2(start.x, start.y), radius, new Vector2(normal.x, normal.y), 100f, layerMask);

        if (hit.collider != null)
        {
            Vector3 point = hit.point + (hit.normal * radius);
            lineRenderers[lineIndex].SetPosition(1, point);
            lineRenderers[lineIndex].gameObject.SetActive(true);

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall") && lineIndex + 1 < lineRenderers.Count)
            {
                normal.x *= -1;
                DrawLine(point, normal, lineIndex + 1);
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

    public void OpenPanel(PanelBase panel)
    {
        if (currentPanel != null)
        {
            ClosePanel();
        }

        currentPanel = panel;
        currentPanel.Open();
        currentPanel.gameObject.SetActive(true);
    }

    public void ClosePanel()
    {
        currentPanel.Close();
        currentPanel.gameObject.SetActive(false);
        currentPanel = null;
    }
}
