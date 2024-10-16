using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TouchInput : MonoBehaviour
{
    [SerializeField] Transform first;
    [SerializeField] Camera screenCamera;

    void Update()
    {
        if (GameManager.Instance.Status != ClientEnum.GameStatus.Play)
        {
            return;
        }

        Vector3 mousePos = screenCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -screenCamera.transform.position.z));
        Vector3 centerPos = first.transform.position;
        centerPos.z = 0;

        if (Input.GetMouseButtonUp(0))
        {
            if (mousePos.y < centerPos.y)
            {
                UIManager.Instance.ReleaseTouch();
                return;
            }

            UIManager.Instance.SetTouchUp();
        }
        else if (Input.GetMouseButton(0))
        {
            if (mousePos.y < centerPos.y)
            {
                UIManager.Instance.ReleaseTouch();
                return;
            }

            Vector3 normal = (mousePos - centerPos).normalized;
            UIManager.Instance.SetTouchDown(normal);
        }
    }


}
