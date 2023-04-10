using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

[System.Serializable]
public class UnityEventListing
{
    public List<UnityEvent> Events = new List<UnityEvent>();

    public void CallEvent(int ID)
    {
        Events[ID].Invoke();
    }
}
