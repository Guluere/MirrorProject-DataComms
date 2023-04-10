using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StartCall : MonoBehaviour
{
    [SerializeField] UnityEvent StartCallEvent;

    // Start is called before the first frame update
    void Start()
    {
        StartCallEvent.Invoke();
    }

}
