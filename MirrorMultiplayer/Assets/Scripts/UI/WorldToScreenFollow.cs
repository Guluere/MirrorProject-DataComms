using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldToScreenFollow : MonoBehaviour
{
    public GameObject target;
    // Update is called once per frame
    void Update()
    {
        Vector3 ScreenPos = Camera.current.WorldToScreenPoint(target.transform.position);
        Debug.Log(ScreenPos);
    }
}
