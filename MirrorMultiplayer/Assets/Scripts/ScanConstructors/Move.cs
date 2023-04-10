using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*!!!Notes: 
 * Script Version (1.5V)
 * By Chan Kwok Chun (Gul)
 * Will be reused in future projects because its perfect for copy and pasting
 * 
 * Update Log:
 * 1.0V     -     : - Basic introduction with simple One to One raycast, Sphereoverlap casts
 */

namespace MoveData
{
    [System.Serializable]
    public class FullClipData
    {
        public int LimitCount;

        public LayerMask Mask;

        public List<CastData.OneToOneRayCastInfo> OneToOneRayCasts = new();
        public List<CastData.OneToManyRayCastInfo> OneToManyRayCasts = new();
        public List<CastData.SphereOverlapInfo> SphereOverlapCasts = new();
        public List<CastData.BoxOverlapInfo> BoxOverlapCasts = new();
    }
}
public class Move : MonoBehaviour
{
    [SerializeField]
    private List<Clip> ClipList = new List<Clip>();

    [SerializeField]
    private List<MoveData.FullClipData> ClipDatas = new List<MoveData.FullClipData>(); //Contains the full data required for building all casts for this move

    //[SerializeField]
    //private List<GameObject> TestList = new(); //For debugging

    [SerializeField]
    private Color GizmosColors;

    [SerializeField]
    [Min(0)] private int ClipViewID;

    [SerializeField]
    private bool TestKey;

    public Dictionary<GameObject, int> ReturnGameObjectsDictionary(int ClipID)
    {
        return ReturnGameObjectsDictionary(ClipID, ClipDatas[ClipID].Mask);
    }
    public List<GameObject> ReturnGameObjects(int ClipID)
    {
        return ReturnGameObjects(ClipID, ClipDatas[ClipID].Mask);
    }

    public Dictionary<GameObject, int> ReturnGameObjectsDictionary(int ClipID, LayerMask Mask)
    {
        Dictionary<GameObject, int> keyValueClipObjects = new();
        foreach (CastData.OneToOneRayCastInfo oneToOneRayCastInfo in ClipDatas[ClipID].OneToOneRayCasts)
        {
            Vector3 RelativeStartPoint = transform.TransformPoint(oneToOneRayCastInfo.SourceVector);
            Vector3 RelativeEndPoint = transform.TransformPoint(oneToOneRayCastInfo.TargetVector);
            RaycastHit[] raycastHit = Physics.RaycastAll(RelativeStartPoint, RelativeEndPoint - RelativeStartPoint, oneToOneRayCastInfo.CastDistance, Mask);
            foreach (RaycastHit hit in raycastHit)
            {
                GameObject gameObject = hit.transform.gameObject;
                if (keyValueClipObjects.ContainsKey(gameObject))
                {
                    keyValueClipObjects[gameObject] += 1;
                }
                else keyValueClipObjects.Add(gameObject, 1);
            }
        }

        foreach (CastData.OneToManyRayCastInfo simpleRayCastInfo in ClipDatas[ClipID].OneToManyRayCasts)
        {
            Vector3 RelativeStartPoint = transform.TransformPoint(simpleRayCastInfo.SourceVector);
            for (int i = 0; i < simpleRayCastInfo.TargetVector.Count; i++)
            {
                Vector3 RelativeEndPoint = transform.TransformPoint(simpleRayCastInfo.TargetVector[i]);
                RaycastHit[] raycastHit = Physics.RaycastAll(RelativeStartPoint, RelativeEndPoint - RelativeStartPoint, simpleRayCastInfo.CastDistance[i], Mask);
                foreach (RaycastHit hit in raycastHit)
                {
                    GameObject gameObject = hit.transform.gameObject;
                    if (keyValueClipObjects.ContainsKey(gameObject))
                    {
                        keyValueClipObjects[gameObject] += 1;
                    }
                    else keyValueClipObjects.Add(gameObject, 1);
                }
            }
        }
        foreach (CastData.SphereOverlapInfo sphereCastInfo in ClipDatas[ClipID].SphereOverlapCasts)
        {
            Vector3 RelativePoint = transform.TransformPoint(sphereCastInfo.SourceVector);
            Collider[] colliderHit = Physics.OverlapSphere(RelativePoint, sphereCastInfo.CastDistance, Mask);
            foreach (Collider collider in colliderHit)
            {
                GameObject gameObject = collider.gameObject;
                if (keyValueClipObjects.ContainsKey(gameObject))
                {
                    keyValueClipObjects[gameObject] += 1;
                }
                else keyValueClipObjects.Add(gameObject, 1);
            }
        }
        foreach (CastData.BoxOverlapInfo boxCastInfo in ClipDatas[ClipID].BoxOverlapCasts)
        {
            Vector3 RelativePoint = transform.TransformPoint(boxCastInfo.SourceVector);
            Collider[] colliderHit = Physics.OverlapBox(RelativePoint, boxCastInfo.FullSize * 0.5f, boxCastInfo.Rotation, Mask);
            foreach (Collider collider in colliderHit)
            {
                GameObject gameObject = collider.gameObject;
                if (keyValueClipObjects.ContainsKey(gameObject))
                {
                    keyValueClipObjects[gameObject] += 1;
                }
                else keyValueClipObjects.Add(gameObject, 1);
            }
        }
        return keyValueClipObjects;
    }
    public List<GameObject> ReturnGameObjects(int ClipID, LayerMask Mask)
    {
        Dictionary<GameObject, int> keyValueClipObjects = ReturnGameObjectsDictionary(ClipID, Mask);
        List<GameObject> returnClips = new List<GameObject>();
        foreach (GameObject objectKey in keyValueClipObjects.Keys)
        {
            int HighestRepeat = Mathf.Min(ClipDatas[ClipID].LimitCount, keyValueClipObjects[objectKey]);
            for (int i = 0; i < HighestRepeat; i++)
            {
                returnClips.Add(objectKey);
            }
        }
        return returnClips;
    }

    void Awake()
    {
        //Grab and hold move's object and all it's parent's rotation, and set all those object's rotation to 0.
        Transform _gameObject = gameObject.transform;
        List<Quaternion> _quaternions = new List<Quaternion>();
        _quaternions.Add(_gameObject.transform.localRotation);
        _gameObject.rotation = new Quaternion(0, 0, 0, 0);
        while (_gameObject.parent != null)
        {
            _gameObject = _gameObject.parent;
            _quaternions.Add(_gameObject.transform.localRotation);
            _gameObject.rotation = new Quaternion(0, 0, 0, 0);
        }
        foreach (Clip clip in ClipList)
        {
            ClipDatas.Add(clip.ReturnFullClipData(transform.position));
            Destroy(clip.gameObject);
        }
        ////return the held rotations to their objects
        int _i = 0;
        _gameObject = gameObject.transform;
        _gameObject.rotation = _quaternions[_i];
        while (_gameObject.parent != null)
        {
            _gameObject = _gameObject.parent;
            _gameObject.rotation = _quaternions[++_i];
        }
        ClipList.Clear();
    }

    public void OnDrawGizmos()
    {
        if (TestKey)
        {
            Gizmos.color = GizmosColors;
            if (ClipDatas.Count > ClipViewID)
            {
                MoveData.FullClipData fullClipData = ClipDatas[ClipViewID];
                foreach (CastData.OneToOneRayCastInfo oneToOneRayCastInfo in fullClipData.OneToOneRayCasts)
                {
                    Gizmos.DrawLine(transform.TransformPoint(oneToOneRayCastInfo.SourceVector), transform.TransformPoint(oneToOneRayCastInfo.TargetVector));
                }
                foreach (CastData.OneToManyRayCastInfo oneToManyRayCastInfo in fullClipData.OneToManyRayCasts)
                {
                    foreach (Vector3 vector3 in oneToManyRayCastInfo.TargetVector)
                    {
                        Gizmos.DrawLine(transform.TransformPoint(oneToManyRayCastInfo.SourceVector), transform.TransformPoint(vector3));
                    }
                }
                foreach (CastData.SphereOverlapInfo sphereCastInfo in fullClipData.SphereOverlapCasts)
                {
                    Gizmos.DrawSphere(transform.TransformPoint(sphereCastInfo.SourceVector), sphereCastInfo.CastDistance);
                }
                foreach (CastData.BoxOverlapInfo boxOverlapInfo in fullClipData.BoxOverlapCasts)
                {
                    Gizmos.matrix = Matrix4x4.TRS(transform.TransformPoint(boxOverlapInfo.SourceVector), transform.localRotation, Vector3.one);
                    Gizmos.DrawCube(Vector3.zero, new Vector3(boxOverlapInfo.FullSize.x, boxOverlapInfo.FullSize.y, boxOverlapInfo.FullSize.z));
                }
            }
        }
    }

    //[MyBox.ButtonMethod]
    //public void Restart() //Debug use
    //{
    //    ClipDatas.Clear();
    //    Start();
    //}

    //[MyBox.ButtonMethod]
    //public void PressTest() //For debugging
    //{
    //    foreach(GameObject obj in ReturnClipIDCollisions(0))
    //    {
    //        TestList.Add(obj);
    //    }
    //}
}
