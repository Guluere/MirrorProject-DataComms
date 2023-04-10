using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;


public class Clip : MonoBehaviour
{
    private string ClipNames;
     
    [SerializeField]
    private bool GameObjectLimit;
    [ConditionalField("GameObjectLimit")] [SerializeField]
    [Min(1)] private int LimitNum;

    [SerializeField]
    private LayerMask Mask;

    [SerializeField]
    private List<Swing> SwingList = new List<Swing>();

    public MoveData.FullClipData ReturnFullClipData(Vector3 MoveBehaviorWorldPosition)
    {
        MoveData.FullClipData _SwingData = new MoveData.FullClipData();
        _SwingData.Mask = Mask;
        _SwingData.LimitCount = LimitNum;
        foreach (Swing swing in SwingList)
        {
            _SwingData.OneToManyRayCasts.Add(swing.SetUpRayCast(transform.position, transform.position - MoveBehaviorWorldPosition));
            if(swing.ReturnSphereCheck()) _SwingData.SphereOverlapCasts.Add(swing.SetUpSphereOverlapCast(transform.position, transform.position - MoveBehaviorWorldPosition));
            _SwingData.BoxOverlapCasts.Add(swing.SetUpBoxOverlapCast(transform.position, transform.position - MoveBehaviorWorldPosition));
            foreach (CastData.OneToOneRayCastInfo oneToOneRayCastInfo in swing.SetUpOneToOneRayCastInfo(transform.position, transform.position - MoveBehaviorWorldPosition))
            {
                _SwingData.OneToOneRayCasts.Add(oneToOneRayCastInfo);
            }
        }    
        return _SwingData;
    }

    [MyBox.ButtonMethod]
    public void Test()
    {
        ReturnFullClipData(transform.position);
    }
}
