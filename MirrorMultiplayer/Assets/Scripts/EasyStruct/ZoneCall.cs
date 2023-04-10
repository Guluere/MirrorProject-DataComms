using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MyBox;

/*!!!Notes: 
 * Script Version (1.2V)
 * By Chan Kwok Chun (Gul)
 * Will be reused in future projects because its perfect for copy and pasting
 * 
 * Update Log:
 * 1.0V     -     : - Introduced with a basic on collision involved event calling, as well as conditional value increasing and decrease before potential calls.
 * 1.1V     -     : - Removed conditional value calling and made it a seperate component.
 * 1.2V     -     : - Included trigger only event calls, as well as the ability to specify what collision layer should be targeted seperately.
 */

public class ZoneCall : MonoBehaviour
{
    [SerializeField]
    LayerMask AcceptedLayer;

    [Space]
    [Space]
    [Space]

    [Space]
    [Header("OnCollisionEnter")]
    [SerializeField]
    private UnityEvent CollisionEnter;
    [Space]
    [Header("OnCollisionStay")]
    [SerializeField]
    private UnityEvent CollisionStay;
    [Space]
    [Header("OnCollisionExit")]
    [SerializeField]
    private UnityEvent CollisionExit;

    [Space]
    [Space]
    [Space]

    [Space]
    [Header("OnTriggerEnter")]
    [SerializeField]
    private UnityEvent TriggerEnter;
    [Space]
    [Header("OnTriggerStay")]
    [SerializeField]
    private UnityEvent TriggerStay;
    [Space]
    [Header("OnTriggerExit")]
    [SerializeField]
    private UnityEvent TriggerExit;

    public void OnCollisionEnter(Collision collision)
    {
        if((AcceptedLayer & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer)
        {
            CollisionEnter.Invoke();
        }    
    }
    public void OnCollisionStay(Collision collision)
    {
        if ((AcceptedLayer & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer)
        {
            CollisionStay.Invoke();
        }
    }
    public void OnCollisionExit(Collision collision)
    {
        if ((AcceptedLayer & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer)
        {
            CollisionExit.Invoke();
        }
    }

    public void OnTriggerEnter(Collider collision)
    {
        if ((AcceptedLayer & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer)
        {
            TriggerEnter.Invoke();
        }
    }
    public void OnTriggerStay(Collider collision)
    {
        if ((AcceptedLayer & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer)
        {
            TriggerStay.Invoke();
        }
    }
    public void OnTriggerExit(Collider collision)
    {
        if ((AcceptedLayer & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer)
        {
            TriggerExit.Invoke();
        }
    }
}
