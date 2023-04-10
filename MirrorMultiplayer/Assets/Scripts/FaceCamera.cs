using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Sentient))]
public class FaceCamera : MonoBehaviour
{
    private Sentient mSentient;

    // Start is called before the first frame update
    void Start()
    {
        mSentient = GetComponent<Sentient>();
        mSentient.AddToAllRotationalControl(new Sentient.MaintainRotationTowardCameraY(gameObject));
    }

    // Update is called once per frame
    void Update()
    {
        //transform.eulerAngles = mSentient.GetAllRotationalControlVector3();
    }

    private void FixedUpdate()
    {
        transform.eulerAngles = mSentient.GetAllRotationalControlVector3();
    }
}
