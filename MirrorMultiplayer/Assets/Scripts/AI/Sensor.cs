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
 * 1.0V 28/02/2023: - Basic introduction with the ability to 'sense' targets and pass objects using Unity's Collider System toward Sentient
 * 1.1V     -     : - 
 * 1.2V     -     : - 
 */

public class Sensor : MonoBehaviour
{
    //public enum SensorTypes
    //{
    //    AddTarget,
    //    RemoveTarget
    //}
    public enum SensorPoints
    {
        OnCollisionEnter,
        OnCollisionStay,
        OnCollisionExit,
        OnTriggerEnter,
        OnTriggerStay,
        OnTriggerExit
    }

    [SerializeField]
    [Min(0)]
    int sensorID;

    [SerializeField]
    SensorPoints sensorPoints;

    [SerializeField]
    SensorBaseTarget SensorBase;

    [SerializeField]
    LayerMask AcceptedLayer;

    private void Filter(GameObject gameObject)
    {
        SensorBase.ObjectSense(sensorID, gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((AcceptedLayer & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer && sensorPoints == SensorPoints.OnCollisionEnter)
        {
            Filter(collision.gameObject);
        }
    }
    public void OnCollisionStay(Collision collision)
    {
        if ((AcceptedLayer & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer && sensorPoints == SensorPoints.OnCollisionStay)
        {
            Filter(collision.gameObject);
        }
    }
    public void OnCollisionExit(Collision collision)
    {
        if ((AcceptedLayer & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer && sensorPoints == SensorPoints.OnCollisionExit)
        {
            Filter(collision.gameObject);
        }
    }

    public void OnTriggerEnter(Collider collision)
    {
        if ((AcceptedLayer & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer && sensorPoints == SensorPoints.OnTriggerEnter)
        {
            Filter(collision.gameObject);
        }
    }
    public void OnTriggerStay(Collider collision)
    {
        if ((AcceptedLayer & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer && sensorPoints == SensorPoints.OnTriggerStay)
        {
            Filter(collision.gameObject);
        }
    }
    public void OnTriggerExit(Collider collision)
    {
        if ((AcceptedLayer & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer && sensorPoints == SensorPoints.OnTriggerExit)
        {
            Filter(collision.gameObject);
        }
    }

    // ------------------------------------

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((AcceptedLayer & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer && sensorPoints == SensorPoints.OnCollisionEnter)
        {
            Filter(collision.gameObject);
        }
    }
    public void OnCollisionStay2D(Collision2D collision)
    {
        if ((AcceptedLayer & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer && sensorPoints == SensorPoints.OnCollisionStay)
        {
            Filter(collision.gameObject);
        }
    }
    public void OnCollisionExit2D(Collision2D collision)
    {
        if ((AcceptedLayer & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer && sensorPoints == SensorPoints.OnCollisionExit)
        {
            Filter(collision.gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if ((AcceptedLayer & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer && sensorPoints == SensorPoints.OnTriggerEnter)
        {
            Filter(collision.gameObject);
        }
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        if ((AcceptedLayer & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer && sensorPoints == SensorPoints.OnTriggerStay)
        {
            Filter(collision.gameObject);
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if ((AcceptedLayer & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer && sensorPoints == SensorPoints.OnTriggerExit)
        {
            Filter(collision.gameObject);
        }
    }
}


public class SensorBaseTarget : MonoBehaviour //For other components that wants to be able to be sensed.
{
    public virtual void ObjectSense(int i, GameObject gameObject)
    {

    }
}