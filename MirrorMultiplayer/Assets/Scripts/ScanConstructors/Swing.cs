using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

namespace CastBuilds
{
    [System.Serializable]
    public class OneToOneRayPoint
    {
        public Color color;
        public GameObject InitialPoint;
        public GameObject EndPoint;
    }
    [System.Serializable]
    public class AreaRayPoint
    {
        public Color color;

        [Range(0, 360)] public float Degrees;
        [Min(1)] public int Chips;

        [Min(0)] public float InnerRadius;
        [Min(0)] public float OuterRadius;

        public List<Vector3> ReturnListOfNormalPoints(GameObject Source)
        {
            List<Vector3> result = new List<Vector3>();
            float HalfDegree = Degrees * 0.5f;
            float DegreeDifference = Degrees / Chips;
            float HalfDifference = DegreeDifference * 0.5f;
            float InitialDegree = -HalfDegree + HalfDifference;

            for (int i = 0; i < Chips; i++)
            {
                float Simple = InitialDegree / 360 * (2 * Mathf.PI);
                float _ForwardBack = Mathf.Cos(Simple);
                float _LeftRight = Mathf.Sin(Simple);
                Vector3 _Rotated = new Vector3(_LeftRight, 0, _ForwardBack);
                InitialDegree += DegreeDifference;
                _Rotated = Source.transform.TransformDirection(_Rotated);
                result.Add(_Rotated);
            }
            return result;
        }
    }
    [System.Serializable]
    public class SpherePoint
    {
        public Color color;
        [Min(0)] public float SphereOverlap; //Signify the sphereoverlap
    }
    [System.Serializable]
    public class BoxPoint
    {
        public Color color;
        [Min(0)] public float Width; //Signify the width of boxoverlap
        [Min(0)] public float Height; //Signify the height of boxoverlap
        [Min(0)] public float Length; //Signify the length of boxoverlap

    }
}

namespace CastData
{
    [System.Serializable]
    public class OneToOneRayCastInfo
    {
        public Vector3 SourceVector;
        public Vector3 TargetVector;
        public float CastDistance;

        public OneToOneRayCastInfo(Vector3 sourceVector, Vector3 targetVector, float castDistance)
        {
            SourceVector = sourceVector;
            TargetVector = targetVector;
            CastDistance = castDistance;
        }
    }
    [System.Serializable]
    public class OneToManyRayCastInfo
    {
        public Vector3 SourceVector;
        public List<Vector3> TargetVector;
        public List<float> CastDistance;

        public OneToManyRayCastInfo(Vector3 sourceVector, List<Vector3> targetVector, List<float> castDistance)
        {
            SourceVector = sourceVector;
            TargetVector = targetVector;
            CastDistance = castDistance;
        }
    }
    [System.Serializable]
    public class SphereOverlapInfo
    {
        public Vector3 SourceVector;
        public float CastDistance;

        public SphereOverlapInfo(Vector3 sourceVector, float castDistance)
        {
            SourceVector = sourceVector;
            CastDistance = castDistance;
        }
    }
    [System.Serializable]
    public class BoxOverlapInfo
    {
        public Vector3 SourceVector;
        public Vector3 FullSize; //Signify the size of boxoverlap
        public Quaternion Rotation;

        public BoxOverlapInfo(Vector3 sourceVector, Vector3 fullSize, Quaternion rotation)
        {
            SourceVector = sourceVector;
            FullSize = fullSize;
            Rotation = rotation;
        }
    }
}


public class Swing : MonoBehaviour
{

    [SerializeField]
    private List<CastBuilds.AreaRayPoint> AreaRayPoints = new();

    [SerializeField]
    private List<CastBuilds.OneToOneRayPoint> SimpleRayCast = new(); //Signify the ray from this object to it.

    [SerializeField]
    private CastBuilds.SpherePoint SpherePoints = new();

    [SerializeField]
    private CastBuilds.BoxPoint BoxPoints = new();

    //[SerializeField]
    //private List<CastData.OneToManyRayCastInfo> TestData = new();

    public List<CastData.OneToOneRayCastInfo> SetUpOneToOneRayCastInfo(Vector3 ClipWorldPosition, Vector3 ClipLocalPosition)
    {
        Vector3 TrueSource = transform.position - ClipWorldPosition + ClipLocalPosition;
        List<CastData.OneToOneRayCastInfo> oneToOneRayCastInfos = new List<CastData.OneToOneRayCastInfo>();
        foreach(var areaRayPoint in AreaRayPoints)
        {
            if (areaRayPoint.InnerRadius > 0)
            {
                List<Vector3> vector3s = areaRayPoint.ReturnListOfNormalPoints(gameObject);
                foreach(Vector3 vector3 in vector3s)
                {
                    Vector3 InnerVector = vector3 * areaRayPoint.InnerRadius + TrueSource;
                    Vector3 OuterVector = vector3 * areaRayPoint.OuterRadius + TrueSource;
                    oneToOneRayCastInfos.Add(new CastData.OneToOneRayCastInfo(InnerVector, OuterVector, Mathf.Abs(areaRayPoint.OuterRadius - areaRayPoint.InnerRadius)));
                }
            }
        }
        return oneToOneRayCastInfos;
    }

    public CastData.OneToManyRayCastInfo SetUpRayCast(Vector3 ClipWorldPosition, Vector3 ClipLocalPosition)
    {
        Vector3 TrueSource = transform.position - ClipWorldPosition + ClipLocalPosition;
        CastData.OneToManyRayCastInfo _RayCastData = new(TrueSource, new List<Vector3>(), new List<float>());
        foreach (var areaRayPoint in AreaRayPoints)
        {
            if (areaRayPoint.InnerRadius == 0)
            {
                List<Vector3> vector3s = areaRayPoint.ReturnListOfNormalPoints(gameObject);
                foreach (Vector3 vector3 in vector3s)
                {
                    Vector3 OuterVector = vector3 * areaRayPoint.OuterRadius + TrueSource;
                    _RayCastData.TargetVector.Add(OuterVector);
                    _RayCastData.CastDistance.Add(areaRayPoint.OuterRadius);
                }
            }
        }
        foreach (CastBuilds.OneToOneRayPoint oneToOneRayPoint in SimpleRayCast) //Simple To Point
        {
            _RayCastData.TargetVector.Add(gameObject.transform.localPosition + TrueSource);
            _RayCastData.CastDistance.Add(Vector3.Distance(transform.position, gameObject.transform.position));
        }
        return _RayCastData;
    }

    public CastData.SphereOverlapInfo SetUpSphereOverlapCast(Vector3 ClipWorldPosition, Vector3 ClipLocalPosition)
    {
        return new CastData.SphereOverlapInfo(transform.position - ClipWorldPosition + ClipLocalPosition, SpherePoints.SphereOverlap);
    }

    public bool ReturnSphereCheck()
    {
        return SpherePoints.SphereOverlap > 0.0f;
    }

    public CastData.BoxOverlapInfo SetUpBoxOverlapCast(Vector3 ClipWorldPosition, Vector3 ClipLocalPosition)
    {
        return new CastData.BoxOverlapInfo(transform.position - ClipWorldPosition + ClipLocalPosition, new Vector3(BoxPoints.Width, BoxPoints.Height, BoxPoints.Length), transform.localRotation);
    }

    public void OnDrawGizmosSelected()
    {
        Vector3 GlobalScale = transform.lossyScale;
        foreach (CastBuilds.OneToOneRayPoint oneToOneRayPoint in SimpleRayCast)
        {
            Gizmos.color = oneToOneRayPoint.color;
            Gizmos.DrawLine(oneToOneRayPoint.InitialPoint.transform.position, oneToOneRayPoint.EndPoint.transform.position);
        }
        foreach (var AreaRayPoint in AreaRayPoints)
        {
            Gizmos.color = AreaRayPoint.color;
            foreach (var obj in AreaRayPoint.ReturnListOfNormalPoints(gameObject))
            {
                Vector3 NewObj = new Vector3(obj.x * GlobalScale.x, obj.y * GlobalScale.y, obj.z * GlobalScale.z);
                Gizmos.DrawLine(transform.position + (NewObj * AreaRayPoint.InnerRadius), transform.position + (NewObj * AreaRayPoint.OuterRadius));
                //Debug.LogError(obj * 10);
            }
        }
        Gizmos.color = SpherePoints.color;
        Gizmos.DrawSphere(transform.position, SpherePoints.SphereOverlap);

        Gizmos.color = BoxPoints.color;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.localRotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, new Vector3(BoxPoints.Width, BoxPoints.Height, BoxPoints.Length));

    }

    //[MyBox.ButtonMethod]
    //public void Test()
    //{
    //    SetUpRayCast(transform.position);
    //    TestData.Add(SetUpRayCast(transform.position));
    //}
}

