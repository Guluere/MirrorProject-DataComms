using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/*!!!Notes: 
 * Script Version (1.5V)
 * By Chan Kwok Chun (Gul)
 * Will be reused in future projects because its perfect for copy and pasting
 * 
 * Update Log:
 * 1.0V     -     : Basic introduction with seperated vector for movement and rotation for each possible type of behavior.
 * 1.1V     -     : Added ability to create new behavior types and switches
 * 1.2V     -     : Changed to Dictionary based control system and object typed behaviors, allowing fo quick access to all vector values within 1 foreach
 * 1.3V     -     : Included TargetList Logging with the help of the Sensor Component, when adding behaviors, you can include the target list
 * 1.4V 05/04/2023: Improved with ability to add prebuilt calculations that is expected, with this, you are able to adjust the AI behavior as needed
 * 1.5V     -     : 
 */

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class Sentient : SensorBaseTarget
{
    public bool canMove = true;

    public List<GameObject> TargetList = new List<GameObject>(); //VectorBehaviors can freely hold the reference to this list to get targets, and other 'codes' can pass objects to be acted on.

    private Dictionary<string, Dictionary<string, List<VectorControlBehavioral>>> AllVectorBehevarial = new Dictionary<string, Dictionary<string, List<VectorControlBehavioral>>>();

    private Dictionary<string, TargetListFilter> TargetListFilters = new Dictionary<string, TargetListFilter>();

    private Dictionary<string, List<VectorControlBehavioral>> AllMovementControl = new Dictionary<string, List<VectorControlBehavioral>>();
    private Dictionary<string, List<VectorControlBehavioral>> AllRotationalControl = new Dictionary<string, List<VectorControlBehavioral>>();

    //public MoveTowardTarget moveTowardTarget;

    public GameObject Holding;

    public UnityEvent OnUpdate;
    public UnityEvent OnFixedUpdate;

    public void SetCanMoveFalse()
    {
        canMove = false;
    }

    public void SetCanMoveTrue()
    {
        canMove = true;
    }

    private void Update()
    {
        OnUpdate.Invoke();
    }

    private void FixedUpdate()
    {
        OnFixedUpdate.Invoke();
    }

    public override void ObjectSense(int i, GameObject gameObject)
    {
        switch (i)
        {
            case 0:
                if (TargetList.Contains(gameObject)) { return; }
                TargetList.Add(gameObject);
                break;
            case 1:
                if (!TargetList.Contains(gameObject)) { return; }
                TargetList.Remove(gameObject);
                break;
            default:
                Debug.Log("Sensor on " + gameObject.name + " has unknown sensorID used.");
                break;
        }
    }

    public void ClearAllTargetControlBehavioral()
    {
        TargetListFilters.Clear();
    }
    public void ClearAllMovementControlBehavioral()
    {
        AllMovementControl.Clear();
    }
    public void ClearAllRotationalControlBehavioral()
    {
        AllRotationalControl.Clear();
    }
    public void ClearAllMovementControlBehavioral(string BehavioralName)
    {
        if(AllMovementControl.Remove(BehavioralName) == false)
        {
            Debug.LogError("Failed to remove MovementControlBehavioral by name :" + BehavioralName + ", either does not exist or incorrect name.");
        }
    }

    public void UpdateAllTargetFilters()
    {
        foreach(TargetListFilter targetListFilter in TargetListFilters.Values)
        {
            targetListFilter.UpdateNaturalTargets();
        }
    }

    public List<VectorControlBehavioral> GetAllMovementControlListByName(string BehavioralName)
    {
        return AllMovementControl[BehavioralName];
    }
    private Vector3 GetAllVectorControlBehavioralVector3(Dictionary<string, List<VectorControlBehavioral>> VectorControlBehavioralDictionary)
    {
        Vector3 result = Vector3.zero;
        foreach (List<VectorControlBehavioral> movementControl in VectorControlBehavioralDictionary.Values)
        {
            foreach (VectorControlBehavioral movementControl2 in movementControl)
            {
                result += movementControl2.UpdatingMovementVector3();
            }
        }
        return result;
    }

    private Vector3 GetAllVectorControlBehavioralVector3(Dictionary<string, List<VectorControlBehavioral>> VectorControlBehavioralDictionary, string BehaverialName)
    {
        Vector3 result = Vector3.zero;
        foreach (VectorControlBehavioral movementControl in VectorControlBehavioralDictionary[BehaverialName])
        {
            result += movementControl.UpdatingMovementVector3();
        }
        return result;
    }

    public Vector3 GetAllMovementControlVector3()
    {
        return GetAllVectorControlBehavioralVector3(AllMovementControl);
    }
    public Vector3 GetAllMovementControlVector3(string BehaverialName)
    {
        return GetAllVectorControlBehavioralVector3(AllMovementControl, BehaverialName);
    }

    public Vector3 GetAllRotationalControlVector3()
    {
        return GetAllVectorControlBehavioralVector3(AllRotationalControl);
    }
    public Vector3 GetAllRotationalControlVector3(string BehaverialName)
    {
        return GetAllVectorControlBehavioralVector3(AllRotationalControl, BehaverialName);
    }

    public void AddTargetFilterList(TargetListFilter targetListFilter)
    {
        targetListFilter.UnnaturalTargets = TargetList;
        targetListFilter.Owner = this;
        TargetListFilters.Add(targetListFilter.FilterName, targetListFilter);
    }

    public void AddToAllMovementControl(VectorControlBehavioral vectorControlBehavioral)
    {
        //vectorControlBehavioral.NaturalTargets = TargetList; //Give it reference for NaturalTargets
        vectorControlBehavioral.Owner = this;
        AllMovementControl.Add(vectorControlBehavioral.BehavioralName, new List<VectorControlBehavioral>());
        AllMovementControl[vectorControlBehavioral.BehavioralName].Add(vectorControlBehavioral);
    }

    public void AddToAllRotationalControl(VectorControlBehavioral vectorControlBehavioral)
    {
        //vectorControlBehavioral.NaturalTargets = TargetList; //Give it reference for NaturalTargets
        vectorControlBehavioral.Owner = this;
        AllRotationalControl.Add(vectorControlBehavioral.BehavioralName, new List<VectorControlBehavioral>());
        AllRotationalControl[vectorControlBehavioral.BehavioralName].Add(vectorControlBehavioral);
    }

    //================================================

    #region TargetListFilter

    public class TargetListFilter
    {
        public string FilterName = "None";
        public Sentient Owner;
        public List<GameObject> UnnaturalTargets;
        public List<Vector3> NaturalTargetPoints = new List<Vector3>();

        public virtual void UpdateNaturalTargets()
        {
            NaturalTargetPoints.Clear();
            foreach (GameObject gameObject in UnnaturalTargets)
            {
                NaturalTargetPoints.Add(gameObject.transform.position);
            }
        }
        public virtual List<Vector3> GetNaturalTargets()
        {
            return NaturalTargetPoints;
        }
    }

    //================================================
    // TargetListFilters
    //================================================

    public class ReturnFirstInList : TargetListFilter
    {
        public override void UpdateNaturalTargets()
        {
            NaturalTargetPoints.Clear();
            foreach (GameObject gameObject in UnnaturalTargets)
            {
                NaturalTargetPoints.Add(gameObject.transform.position);
            }
        }
        public override List<Vector3> GetNaturalTargets()
        {
            return NaturalTargetPoints;
        }
    }

    public class ReturnClosestToFurthest : TargetListFilter
    {
        public override void UpdateNaturalTargets()
        {
            List<Vector3> _CurrentList = new List<Vector3>(); //Temporaryly grab and hold
            foreach(GameObject gameObject in UnnaturalTargets)
            {
                _CurrentList.Add(gameObject.transform.position);
            }

            List<float> floats = new List<float>();
            foreach(Vector3 point in _CurrentList)
            {
                floats.Add((point - Owner.transform.position).sqrMagnitude);
            }

            Vector3 _CurrentCompare;
            float _Temp;

            for ( int i = 0; i <= _CurrentList.Count - 2; i++ )
            {
                for( int j = 0; j <= _CurrentList.Count - 2; j++ )
                {
                    if (floats[i] > floats[i + 1])
                    {
                        _Temp = floats[i + 1];
                        floats[i + 1] = floats[i];
                        floats[i] = _Temp;

                        _CurrentCompare = _CurrentList[i + 1];
                        _CurrentList[i + 1] = _CurrentList[i];
                        _CurrentList[i] = _CurrentCompare;
                    }
                }
            }
            NaturalTargetPoints = _CurrentList;
        }

        public override List<Vector3> GetNaturalTargets()
        {
            return NaturalTargetPoints;
        }
    }

    #endregion TargetListFilter

    //================================================

    #region VectorCalculationFilters

    public class VectorCalculationFilters
    {
        public string BehavioralName = "None";
        public bool Active;
        public float Strength = 0.0f;

        public Vector2 CurrentVector2 = Vector2.zero;
        public Vector3 CurrentVector3 = Vector3.zero;
        public virtual Vector2 Vector2CalculationPass(ref Vector2 Vector2ToFilter)
        {
            Vector2ToFilter = Vector2.zero;
            Vector2 result = Vector2.zero;
            CurrentVector2 = result;
            return result;
        }
        public virtual Vector3 Vector3CalculationPass(ref Vector3 Vector3ToFilter)
        {
            Vector3ToFilter = Vector3.zero;
            Vector3 result = Vector3.zero;
            CurrentVector3 = result;
            return result;
        }
    }

    //================================================
    // VectorCalculationFilters
    //================================================

    public class VectorFilterCalculateRelativeToCamera : VectorCalculationFilters
    {
        public VectorFilterCalculateRelativeToCamera()
        {
            BehavioralName = "VectorFilterCalculateRelativeToCamera";
            Active = true;
            Strength = 1.0f;
        }

        public override Vector3 Vector3CalculationPass(ref Vector3 Vector3ToFilter)
        {
            if (Active == false) return Vector3ToFilter;
            Vector3ToFilter = CurrentVector3 = Camera.main.transform.TransformVector(Vector3ToFilter);
            return Vector3ToFilter;
        }
    }

    public class VectorFilterCalculateClampToXYToXZ : VectorCalculationFilters
    {
        public VectorFilterCalculateClampToXYToXZ()
        {
            BehavioralName = "VectorFilterCalculateClampToXYToXZ";
            Active = true;
            Strength = 1.0f;
        }

        public override Vector3 Vector3CalculationPass(ref Vector3 Vector3ToFilter)
        {
            if (Active == false) return Vector3ToFilter;
            Vector3ToFilter = CurrentVector3 = new Vector3(Vector3ToFilter.x, 0, Vector3ToFilter.y);
            return Vector3ToFilter;
        }
    }

    public class VectorFilterCalculateNormalize : VectorCalculationFilters
    {
        public VectorFilterCalculateNormalize()
        {
            BehavioralName = "VectorFilterCalculateNormalize";
            Active = true;
            Strength = 1.0f;
        }

        public override Vector3 Vector3CalculationPass(ref Vector3 Vector3ToFilter)
        {
            if (Active == false) return Vector3ToFilter;
            Vector3ToFilter = CurrentVector3 = Vector3ToFilter.normalized;
            return Vector3ToFilter;
        }
    }
    public class VectorFilterCalculateMultiply : VectorCalculationFilters
    {
        public VectorFilterCalculateMultiply(float Strength)
        {
            BehavioralName = "VectorFilterCalculateMultiply";
            Active = true;
            this.Strength = Strength;
        }

        public override Vector3 Vector3CalculationPass(ref Vector3 Vector3ToFilter)
        {
            if (Active == false) return Vector3ToFilter;
            Vector3ToFilter = CurrentVector3 = Vector3ToFilter * Strength;
            return Vector3ToFilter;
        }
    }

    #endregion VectorCalculationFilters

    //================================================

    #region VectorControlBehaviorals
    public class VectorControlBehavioral
    {
        public string BehavioralName = "None";
        public bool Active;
        public float Strength = 0.0f;
        public Sentient Owner;
        public TargetListFilter PureTargets;
        private List<VectorCalculationFilters> CalculationFilters;

        public Vector2 CurrentVector2 = Vector2.zero;
        public Vector3 CurrentVector3 = Vector3.zero;

        public void AddCalculator(VectorCalculationFilters Calculator)
        {
            CalculationFilters.Add(Calculator);
        }

        public void RemoveCalculator(VectorCalculationFilters Calculator)
        {
            CalculationFilters.Remove(Calculator);
        }

        public virtual Vector2 UpdatingMovementVector2()
        {
            return CurrentVector2;
        }
        public virtual Vector3 UpdatingMovementVector3()
        {
            return PassThoughCalculationsVector3(CurrentVector3);
        }
        public Vector2 PassThoughCalculationsVector2(Vector2 Pass)
        {
            foreach (VectorCalculationFilters calculationFilters in CalculationFilters)
            {
                calculationFilters.Vector2CalculationPass(ref Pass);
            }
            return Pass;
        }
        public Vector3 PassThoughCalculationsVector3(Vector3 Pass)
        {
            foreach( VectorCalculationFilters calculationFilters in CalculationFilters)
            {
                calculationFilters.Vector3CalculationPass(ref Pass);
            }
            return Pass;
        }
    }

    //================================================
    // VectorControlBehaviorals
    //================================================


    public class ControlledVector : VectorControlBehavioral
    {
        Vector3 Vector3;

        public ControlledVector()
        {
            BehavioralName = "ControlledVector";
            Active = true;
            Strength = 1.0f;
        }

        public void ChangeVector(Vector3 vector3)
        {
            Vector3 = vector3;
        }

        public override Vector3 UpdatingMovementVector3()
        {
            return PassThoughCalculationsVector3(Vector3) * Strength;
        }
    }

    public class WanderingByTargetless : VectorControlBehavioral
    {
        int NextPointTime;
        int CurrentPointTime;

        Vector3 CurrentTargetPoint;

        float MinRange;
        float MaxRange;

        float VerticleRange;
        float HorizontalRange;

        public WanderingByTargetless(TargetListFilter targetListFilter, int nextPointTime, float minRange, float maxRange, float verticleRange, float horizontalRange)
        {
            BehavioralName = "WanderingByTargetless";
            Active = true;
            Strength = 1.0f;
            PureTargets = targetListFilter;
            NextPointTime = nextPointTime;
            MinRange = minRange;
            MaxRange = maxRange;
            VerticleRange = verticleRange;
            HorizontalRange = horizontalRange;
        }

        public void ResetCurrentTargetPoint() //Randomly determine a point to wander towards
        {
            Vector3 PointToPoint = new Vector3(Random.Range(-HorizontalRange, HorizontalRange), 0, Random.Range(-VerticleRange, VerticleRange)).normalized;
            CurrentTargetPoint = new Vector3(Owner.gameObject.transform.position.x, 0, Owner.gameObject.transform.position.z) + (PointToPoint * Random.Range(MinRange, MaxRange));
        }

        public override Vector3 UpdatingMovementVector3()
        {
            if (!Active || PureTargets.GetNaturalTargets().Count > 0) return Vector3.zero;

            if(++CurrentPointTime >= NextPointTime)
            {
                ResetCurrentTargetPoint();
            }

            Vector3 Difference = CurrentTargetPoint - Owner.transform.position;
            Vector3 MovingVector = (Difference).normalized * Mathf.Min((Difference).sqrMagnitude, 1) * Strength;
            return new Vector3(MovingVector.x, 0, MovingVector.z).normalized;
        }
    }
    public class MoveTowardTarget : VectorControlBehavioral
    {
        float MoveSpeed;
        float DividingStrength;
        float MaximumMultiplyingStrength;

        public MoveTowardTarget(TargetListFilter targetListFilter, float moveSpeed, float dividingStrenght, float maximumMultiplyingStrength)
        {
            BehavioralName = "MoveTowardTarget";
            Active = true;
            Strength = 1.0f;
            MoveSpeed = moveSpeed;
            DividingStrength = dividingStrenght;
            MaximumMultiplyingStrength = maximumMultiplyingStrength;
            PureTargets = targetListFilter;
        }
        public override Vector3 UpdatingMovementVector3()
        {
            if (!Active || PureTargets.GetNaturalTargets().Count == 0 ) return Vector3.zero;

            Vector3 MovingVector;
            //Debug.LogError(PureTargets.GetNaturalTargets()[0]);

            Vector3 TargetPointing = PureTargets.GetNaturalTargets()[0] - Owner.transform.position;
            MovingVector = TargetPointing.normalized * MoveSpeed * Mathf.Min(Vector3.Distance(PureTargets.GetNaturalTargets()[0], Owner.transform.position) / DividingStrength, MaximumMultiplyingStrength);
            MovingVector = new Vector3(MovingVector.x, 0f, MovingVector.z);

            //Debug.LogError(MovingVector * Strenth);

            return MovingVector * Strength;
        }
    }

    public class RigidbodyDifferentiation : VectorControlBehavioral //Changes made directly to the rigidbody can be marked down here.
    {
        Vector3 LastVelocity;
        Rigidbody rb;

        public RigidbodyDifferentiation(Rigidbody rigidbody)
        {
            BehavioralName = "RigidbodyDifferentiation";
            rb = rigidbody;
        }

        public override Vector3 UpdatingMovementVector3()
        {
            Vector3 Differences = rb.velocity - LastVelocity;
            LastVelocity += Differences;
            return Differences;
        }
    }

    #endregion VectorControlBehaviorals

    //================================================
    // Rotations (Note! These will break the movement if used outside of rotation)
    //================================================

    public class FlipYRotTowardTargetRelativeToSelf : VectorControlBehavioral
    {
        Vector3 CurrentRot;
        bool CurrentlyPointActivity;

        public FlipYRotTowardTargetRelativeToSelf(TargetListFilter targetListFilter)
        {
            BehavioralName = "FlipYRotTowardTargetRelativeToSelf";
            Active = true;
            PureTargets = targetListFilter;
        }

        public override Vector3 UpdatingMovementVector3()
        {
            if (PureTargets.NaturalTargetPoints.Count == 0) return new Vector3(0, 0, 0);

            Vector3 TargetPos = PureTargets.GetNaturalTargets()[0];
            Vector3 PointToward = (TargetPos - Owner.transform.position).normalized;

            //Vector3 RelativeDirection = Owner.transform.InverseTransformDirection(TargetPos);
            //Vector3 PointTowardUltra = new Vector3(PointToward.x, 0, 0).normalized;

            Vector3 DirectNegativeX = Owner.transform.TransformDirection(new Vector3(1, 0, 0)).normalized;
            //Vector3 DirectNegativeXUltra = new Vector3(DirectNegativeX.x, 0, 0).normalized;

            float Dot = Vector3.Dot(PointToward, DirectNegativeX);

            //Owner.transform.

            //Debug.LogError("PointTowardUltra: " + PointTowardUltra);
            //Debug.LogError("DirectNegativeXUltra: " + DirectNegativeXUltra);
            //Debug.LogError("Dot: " + Dot);

            if (Dot < 0)
            {
                if (CurrentlyPointActivity)
                {
                    CurrentRot = new Vector3(0, 0, 0);
                    CurrentlyPointActivity = false;
                }
                else if (!CurrentlyPointActivity)
                {
                    CurrentRot = new Vector3(0, 180, 0);
                    CurrentlyPointActivity = true;
                }
            }
            return CurrentRot;
        }
    }

    public class FlipTowardXVelocity : VectorControlBehavioral
    {
        Rigidbody rb;
        Vector3 CurrentRot;

        public FlipTowardXVelocity(Rigidbody rigidbody)
        {
            rb = rigidbody;
        }

        public override Vector3 UpdatingMovementVector3()
        {
            if (rb.velocity.x > 0.1) 
            {
                CurrentRot = new Vector3(0, 0, 0);
                return CurrentRot; 
            }
            else if (rb.velocity.x < -0.1) 
            {
                CurrentRot = new Vector3(0, 180, 0);
                return CurrentRot;
            }

            return CurrentRot;
        }
    }

    public class MaintainRotationTowardCameraY : VectorControlBehavioral
    {
        Vector3 OriginalEulerRotation;

        public MaintainRotationTowardCameraY(GameObject Origin)
        {
            BehavioralName = "MaintainRotationTowardCameraY";
            OriginalEulerRotation = Origin.transform.eulerAngles;
        }
        public override Vector3 UpdatingMovementVector3()
        {
            Vector3 CamRot = new Vector3(0, Camera.main.transform.eulerAngles.y, 0);
            return CamRot;
        }
    }

    public class Flipping : VectorControlBehavioral
    {
        Vector3 FlipValue;

        public override Vector3 UpdatingMovementVector3()
        {
            return FlipValue;
        }

        public void FlipX()
        {
            if(FlipValue.x > 0)
            {
                FlipValue.x = 0;
                return;
            }
            FlipValue.x = 180.0f;
        }
        public void FlipY()
        {
            if (FlipValue.y > 0)
            {
                FlipValue.y = 0;
                return;
            }
            FlipValue.y = 180.0f;
        }
        public void FlipZ()
        {
            if (FlipValue.y > 0)
            {
                FlipValue.y = 0;
                return;
            }
            FlipValue.y = 180.0f;
        }
    }
}
