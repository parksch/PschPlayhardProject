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

        if (Input.GetMouseButtonUp(0))
        {
            UIManager.Instance.SetTouchUp();
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = screenCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -screenCamera.transform.position.z));
            Vector3 centerPos = first.transform.position;
            centerPos.z = 0;
            Vector3 normal = (mousePos - centerPos).normalized;
            UIManager.Instance.SetTouchDown(normal);
        }
    }


}
