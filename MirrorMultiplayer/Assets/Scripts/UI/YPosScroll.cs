using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

public class YPosScroll : MonoBehaviour
{
    [SerializeField]
    Transform m_Transform;
    [SerializeField]
    Scrollbar m_Scrollbar;

    [SerializeField]
    InputActionReference actionReferenceScroll;

    public float DirectConvert;
    public float MiddlePoint;

    public float ScrollOverTime;

    public void UpdateTransformPos()
    {
        m_Transform.localPosition = new Vector3(m_Transform.localPosition.x, DirectConvert * m_Scrollbar.value + MiddlePoint, m_Transform.localPosition.z);
    }

    private void Update()
    {
        Vector3 _Movement = actionReferenceScroll.action.ReadValue<Vector2>();
        m_Scrollbar.value += -_Movement.y * ScrollOverTime * Time.deltaTime;
        m_Scrollbar.value = Mathf.Min(Mathf.Max(m_Scrollbar.value, 0), 1);
        UpdateTransformPos();
    }
}
