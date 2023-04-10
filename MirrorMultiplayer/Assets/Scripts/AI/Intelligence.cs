using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MyBox;

/*!!!Notes: 
 * Script Version (1.0V)
 * By Chan Kwok Chun (Gul)
 * Will be reused in future projects because its perfect for copy and pasting
 * 
 * Update Log:
 * 1.0V 09/03/2023: - Basic introduction with a virtual ChangeStateTo
 */

/// <summary>
/// DO NOT USE THIS COMPONENT SEPERATELY 
/// </summary>
public class Intelligence : MonoBehaviour
{
    [SerializeField]
    public Sentient Brain;

    public virtual void TurnOffIntelligence()
    {
        enabled = false;
    }
    public virtual void TurnOnIntelligence()
    {
        enabled = true;
    }

    public virtual void ClearGarbageControls()
    {
        Brain.ClearAllMovementControlBehavioral();
        Brain.ClearAllTargetControlBehavioral();
        Brain.ClearAllRotationalControlBehavioral();
    }

    public virtual void ChangeStateTo(int StateChoice)
    {
    }
}
