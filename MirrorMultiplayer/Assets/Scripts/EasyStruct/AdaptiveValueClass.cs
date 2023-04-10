using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AdaptiveValue //Adaptive Value acts as a manager of the layers
{
    [System.Serializable]
    public class AdaptivePoint
    {
        private AdaptiveLayer Factor = null;

        public AdaptivePoint Prev = null;
        public AdaptivePoint Next = null;

        public float CurrentFullTotal { get; private set; } = 0;
        public float CurrentFullAdditive { get; private set; } = 0;
        public float CurrentFullAdditiveMultiplier { get; private set; } = 0;
        public float CurrentFullMultiplicativeMultiplier { get; private set; } = 0;

        public AdaptivePoint(AdaptiveLayer Factor)
        {
            this.Factor = Factor;
            Factor.ReferedAdaptivePoints.Add(this);
        }

        public void ChangeFactor(AdaptiveLayer NewFactor)
        {
            if (Factor != null) Factor.ReferedAdaptivePoints.Remove(this);

            Factor = NewFactor;
            NewFactor.ReferedAdaptivePoints.Add(this);
            RegenerateSelfAndAllNext();
        }

        //public void UpdateAllNext(AdaptivePoint Prev)
        //{
        //    UpdateAll(Prev);
        //    if (Next == null) return;
        //    Next.UpdateAllNext(this);
        //}

        public void RegenerateSelfAndAllNext()
        {
            if (Prev == null) UpdateAll();
            else UpdateAll(Prev);
            if (Next == null) return;
            Next.RegenerateSelfAndAllNext();
        }
        public void UpdateAll() //Assumes that there is no Prev and so it self is the first
        {
            UpdateFullTotal();
            UpdateFullAdditive();
            UpdateFullAdditiveMultiplier();
            UpdateFullMultiplicativeMultiplier();
        }
        public void UpdateAll(AdaptivePoint Prev)
        {
            UpdateFullTotal(Prev.CurrentFullTotal);
            UpdateFullAdditive(Prev.CurrentFullAdditive);
            UpdateFullAdditiveMultiplier(Prev.CurrentFullAdditiveMultiplier);
            UpdateFullMultiplicativeMultiplier(Prev.CurrentFullMultiplicativeMultiplier);
        }

        public void UpdateAll(float PrevFullTotal, float PrevFullAdditive, float PrevFullAdditiveMultiplier, float PrevFullMultiplicativeMultiplier)
        {
            UpdateFullTotal(PrevFullTotal);
            UpdateFullAdditive(PrevFullAdditive);
            UpdateFullAdditiveMultiplier(PrevFullAdditiveMultiplier);
            UpdateFullMultiplicativeMultiplier(PrevFullMultiplicativeMultiplier);
        }

        public void UpdateFullTotal(float PrevValue)
        {
            CurrentFullTotal = (PrevValue + Factor.GetAdditive()) * Factor.GetMultiplicative();
        }
        public void UpdateFullAdditive(float PrevValue)
        {
            CurrentFullAdditive = PrevValue + Factor.GetAdditive();
        }
        public void UpdateFullAdditiveMultiplier(float PrevValue)
        {
            CurrentFullAdditiveMultiplier = PrevValue + (Factor.GetMultiplicative() - 1);
        }
        public void UpdateFullMultiplicativeMultiplier(float PrevValue)
        {
            CurrentFullMultiplicativeMultiplier = PrevValue * Factor.GetMultiplicative();
        }
        public void UpdateFullTotal() //Without a Prev, it assumes that this is the first of the list
        {
            CurrentFullTotal = Factor.GetAdditive() * Factor.GetMultiplicative();
        }
        public void UpdateFullAdditive()
        {
            CurrentFullAdditive = Factor.GetAdditive();
        }
        public void UpdateFullAdditiveMultiplier()
        {
            CurrentFullAdditiveMultiplier = Factor.GetMultiplicative();
        }
        public void UpdateFullMultiplicativeMultiplier()
        {
            CurrentFullMultiplicativeMultiplier = Factor.GetMultiplicative();
        }
    }

    [System.Serializable]
    public class AdaptiveLayer
    {
        [HideInInspector]
        public List<AdaptivePoint> ReferedAdaptivePoints = new List<AdaptivePoint>();

        [SerializeField]
        private float Additive;

        [SerializeField]
        private float Multiplicative;

        public AdaptiveLayer()
        {
            Additive = 0;
            Multiplicative = 1;
        }

        public float GetAdditive()
        {
            return Additive;
        }

        public float GetMultiplicative()
        {
            return Multiplicative;
        }

        public void UpdatePoints()
        {
            foreach (AdaptivePoint adaptivePoint in ReferedAdaptivePoints)
            {
                adaptivePoint.RegenerateSelfAndAllNext();
            }
        }

        public void ChangeValues(float AddSet, float MulSet) //Use this to change the value of all points
        {
            Additive = AddSet;
            Multiplicative = MulSet;

            UpdatePoints();
        }
    }

    [SerializeField]
    private AdaptiveLayer FirstLayer;    //Will always act as the first layer

    private AdaptivePoint FirstPoint; //Point stores the resulting value that is created by the layers

    //[SerializeField]
    private List<AdaptivePoint> AdaptivePoints = new List<AdaptivePoint>();

    [SerializeField]
    private AdaptiveLayer LastLayer;    //The final layer will actively change depending on the final value that you actually need.

    private AdaptivePoint LastPoint;

    public float FullValue { get { return LastPoint.CurrentFullTotal; } private set { } }
    public float FullAdditiveValue { get { return LastPoint.CurrentFullAdditive; } private set { } }
    public float FullAdditiveMultiplier { get { return LastPoint.CurrentFullAdditiveMultiplier; } private set { } }
    public float FullMultiplicativeMultiplier { get { return LastPoint.CurrentFullMultiplicativeMultiplier; } private set { } }

    public AdaptiveValue() //Defaults
    {
        FirstLayer = new AdaptiveLayer();
        LastLayer = new AdaptiveLayer();

        FirstPoint = new AdaptivePoint(FirstLayer);
        LastPoint = new AdaptivePoint(LastLayer);

        FirstPoint.Next = LastPoint;
        LastPoint.Prev = FirstPoint;
    }

    public void Initialize() //In order for the Serialized values to apply, you have to call this after the serialization step, which is after the constuction
    {
        FirstPoint.RegenerateSelfAndAllNext();
    }

    public void SetValueBasedOnFull(float Value)
    {
        LastLayer.ChangeValues(Value - LastPoint.CurrentFullTotal, LastLayer.GetMultiplicative());
    }

    /*

    public AdaptivePoint AddLayer(int Addition, float Multiply) //Add a new layer, you can leave Additive as 0, or multiple as 1 if you want to only include the other.
    {
        AdaptiveLayer adaptiveLayer = new AdaptiveLayer();
        AdaptivePoint adaptivePoint = new AdaptivePoint(adaptiveLayer);

        //Set Next and Prev
        if (AdaptivePoints.Count - 1 < 0)
        {
            adaptivePoint.Prev = FirstPoint;
            FirstPoint.Next = adaptivePoint;
        }
        else
        {
            adaptivePoint.Prev = AdaptivePoints[AdaptivePoints.Count - 1];
            AdaptivePoints[AdaptivePoints.Count - 1].Next = adaptivePoint;
        }
        adaptivePoint.Next = LastPoint;
        LastPoint.Prev = adaptivePoint;

        //Update Values of new layer
        adaptiveLayer.ChangeValues(Addition, Multiply);
        Initialize();
        AdaptivePoints.Add(adaptivePoint);

        return adaptivePoint;
    }

    */

    public AdaptivePoint AddLayer(AdaptiveLayer adaptiveLayer) //Uses a Layer to create a new point
    {
        AdaptivePoint adaptivePoint = new AdaptivePoint(adaptiveLayer);

        //Set Next and Prev
        if (AdaptivePoints.Count - 1 < 0)
        {
            adaptivePoint.Prev = FirstPoint;
            FirstPoint.Next = adaptivePoint;
        }
        else
        {
            adaptivePoint.Prev = AdaptivePoints[AdaptivePoints.Count - 1];
            AdaptivePoints[AdaptivePoints.Count - 1].Next = adaptivePoint;
        }
        adaptivePoint.Next = LastPoint;
        LastPoint.Prev = adaptivePoint;

        //Update Values of new layer
        //adaptivePoint.RegenerateSelfAndAllNext();
        Initialize();
        AdaptivePoints.Add(adaptivePoint);

        return adaptivePoint;
    }

    public void RemoveLayer(int LayerID) //Layer of ID
    {
        if (LayerID < 0 || LayerID >= AdaptivePoints.Count) return; //Out of bounds

        if (LayerID == 0)
        {

        }
        else
        {

        }

        if (LayerID >= AdaptivePoints.Count - 1)
        {

        }
        else
        {

        }

        AdaptivePoints.RemoveAt(LayerID);
    }
}