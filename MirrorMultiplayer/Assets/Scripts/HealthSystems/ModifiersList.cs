using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ModifiersList : MonoBehaviour
{
    List<Modifier> Modifiers;

    public void AddModifier(Modifier mod)
    {
        mod.Owner = this;
        Modifiers.Add(mod);
    }

    public void RemoveModifier(Modifier mod)
    {
        foreach (Modifier mod2 in Modifiers)
        {
            if (mod2 == mod)
            {
                Modifiers.Remove(mod2);
                return;
            }
        }
    }

    public class Modifier
    {
        public ModifiersList Owner;

        List<WaitForSeconds> Intervals;
        WaitForSeconds Duration;

        List<ModifierCore> Effects;

        Modifier(float duration)
        {

        }

        public void AddModCore()
        {

        }
    }
}

public class ModifierCore
{
    public virtual void OnAdd() { }
    public virtual void Action() { }
    public virtual void OnRemove() { }
}
namespace Mods
{
    public class DamageOverTime : ModifierCore
    {
        HealthSystem Heart;
        int Damage;

        DamageOverTime(HealthSystem Target, int damage)
        {
            Heart = Target;
            Damage = damage;
        }

        public override void Action()
        {
            Heart.Hurt(Damage);
        }
    }
}