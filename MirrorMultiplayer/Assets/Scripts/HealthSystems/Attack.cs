using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;


[System.Serializable]
public class Damage
{
    public AdaptiveValue Value = new AdaptiveValue();
    public AdaptiveValue PenetrationPower = new AdaptiveValue();
    public int ClipID;
    public EventCull OnHit;
    public EventCull OnHitByNullEffect;
    public EventCull OnMiss;
    public LayerMask LayerMask;

    public void Initialize()
    {
        Value.Initialize();
        PenetrationPower.Initialize();
    }
}

[System.Serializable]
public class EventCull //To limit the amount of times the event is called because of multiple targets is hit in 1 attack
{
    public int RepeatTimes; //PuttingNegative will cause it to always repeat
    public UnityEvent Event;

    private int CurrentRepeat;

    public void Invoke()
    {
        if (CurrentRepeat == RepeatTimes) return;

        Event.Invoke();
        ++CurrentRepeat;
    }

    public void ResetCount()
    {
        CurrentRepeat = 0;
    }
}

public class Attack : SensorBaseTarget
{
    [SerializeField]
    private List<Damage> Damage;

    [SerializeField]
    private Move Move;

    private void Start()
    {
        foreach (Damage damages in Damage) damages.Initialize();
    }

    public override void ObjectSense(int i, GameObject gameObject)
    {
        if (i >= Damage.Count)
        {
            Debug.LogError("Attack Damage list for " + this.gameObject.name + " of collision is out of bounds.");
            return;
        }

        if (gameObject.TryGetComponent(out Defender Def))
        {
            Damage[i].OnHit.Invoke();
            Def.TakeDamage(Damage[i]);
            return;
        }
        else
        {
            Damage[i].OnHitByNullEffect.Invoke();
            Debug.LogError("GameObject " + gameObject.name + " Does not contain Defender Component");
        }


        Damage[i].OnHit.ResetCount();
        Damage[i].OnHitByNullEffect.ResetCount();

    }

    public void DealDamage(int DamageID) //Targets only the Defender, uses the Hitscan builder
    {
        if (DamageID >= Damage.Count)
        {
            Debug.LogError("Attack Damage list for " + gameObject.name + " of hitscan is out of bounds.");
            return;
        }

        List<GameObject> gameObjects = Move.ReturnGameObjects(Damage[DamageID].ClipID, Damage[DamageID].LayerMask);
        foreach (GameObject gameObject in gameObjects)
        {
            if (gameObject.TryGetComponent(out Defender Def))
            {
                Damage[DamageID].OnHit.Invoke();
                Def.TakeDamage(Damage[DamageID]);
            }
            else
            {
                Damage[DamageID].OnHitByNullEffect.Invoke();
                Debug.LogError("GameObject " + gameObject.name + " Does not contain Defender Component");
            }
        }
        if(gameObjects.Count == 0)
        {
            Damage[DamageID].OnMiss.Invoke();
        }
        Damage[DamageID].OnHit.ResetCount();
        Damage[DamageID].OnHitByNullEffect.ResetCount();
        Damage[DamageID].OnMiss.ResetCount();
    }
    public void DestroySelf() //Used in some cases, do not remove yet.
    {
        Destroy(gameObject);
    }
}
