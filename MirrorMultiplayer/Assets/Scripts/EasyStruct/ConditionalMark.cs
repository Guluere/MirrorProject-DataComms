using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ConditionalMark : MonoBehaviour
{
    private int ConditionCount;
    public int ConditionLimit;
    public UnityEvent EqualsTo;
    public UnityEvent MoreThan;
    public UnityEvent MoreThanEquals;
    public UnityEvent LessThan;
    public UnityEvent LessThanEquals;

    public void ChangeCountBy(int Change) { ConditionCount += Change; }
    public void ChangeCountTo(int Change) { ConditionCount = Change; }

    public void InvokeEqualsTo()
    {
        if (ConditionCount == ConditionLimit) { EqualsTo.Invoke(); }
    }
    public void InvokeMoreThan()
    {
        if(ConditionCount > ConditionLimit) { MoreThan.Invoke(); }
    }
    public void InvokeMoreThanEquals()
    {
        if (ConditionCount >= ConditionLimit) { MoreThanEquals.Invoke(); }
    }
    public void InvokeLessThan()
    {
        if (ConditionCount < ConditionLimit) { LessThan.Invoke(); }
    }
    public void InvokeLessThanEquals()
    {
        if (ConditionCount <= ConditionLimit) { LessThanEquals.Invoke(); }
    }
}
