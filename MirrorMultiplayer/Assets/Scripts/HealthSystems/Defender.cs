using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class Defender : MonoBehaviour
{
    [SerializeField]
    HealthSystem Heart;

    [SerializeField]
    AdaptiveValue Shield = new AdaptiveValue();

    [SerializeField]
    AdaptiveValue Armor = new AdaptiveValue();

    public UnityEvent OnShieldBlocked;
    public UnityEvent OnArmorBlocked;

    public void TakeDamage(Damage damage) //Filters Damage
    {
        if ((int)damage.PenetrationPower.FullValue < (int)Shield.FullValue) { OnShieldBlocked.Invoke(); return; }

        switch (damage.Value.FullValue)
        {
            case > 0:
                float _d = damage.Value.FullValue - Armor.FullValue;
                if (_d <= 0)
                {
                    OnArmorBlocked.Invoke();
                    break;
                }
                Heart.Hurt(_d);
                break;
            case < 0:
                Heart.Heal(Mathf.Abs(damage.Value.FullValue));
                break;
            default:
                break;
        }
    }


    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
