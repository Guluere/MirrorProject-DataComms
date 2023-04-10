using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class EventRandomCall : MonoBehaviour
{
    public List<UnityEvent> RandomOrderList = new List<UnityEvent>();
    public void InvokeRandom()
    {
        int _i = Random.Range(0, RandomOrderList.Count);
        RandomOrderList[_i].Invoke();
    }
    public void InvokeByMaxID(int max)
    {
        int _i = Random.Range(0, Mathf.Min(max + 1, RandomOrderList.Count));
        RandomOrderList[_i].Invoke();
    }
    public void InvokeByRangeID(int min, int max)
    {
        int MinLimit = Mathf.Max(0, min);
        int MaxLimit = Mathf.Min(max + 1, RandomOrderList.Count);
        int _i = Random.Range(Mathf.Min(MaxLimit, MinLimit), Mathf.Max(MaxLimit, MinLimit));
        RandomOrderList[_i].Invoke();
    }
}
