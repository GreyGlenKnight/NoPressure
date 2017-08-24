using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Consumeable resource types that can refill the charges of items
public enum ResourceType
{
    MassDriver,
    EnergyCell,
    Explosive,
    Parts,
    Charges,
}

public class Pool : MonoBehaviour
{
    public float mCapacity;
    public float mValue;
    public string mName;

    // Default to pools being full
    public Pool(float lCapacity)
    {
        AssignValues(lCapacity, lCapacity);
    }

    public Pool(float lCapacity, float lValue)
    {
        AssignValues(lCapacity, lValue);
    }

    public virtual void Add(float lAmount)
    {
        mValue += lAmount;
        CheckValue();
    }

    protected virtual void CheckValue()
    {
        // Pools must have a non negative value
        if (mValue < 0)
        {
            mValue = 0;
        }

        // Pools can not be "overflowing"
        if (mValue > mCapacity)
        {
            mValue = mCapacity;
        }
    }

    protected void AssignValues(float lSize, float lValue)
    {
        // Pools must have a positive size
        if (lSize <= 0)
        {
            Debug.LogError("Resource Pool size must be a positive number!");
        }
        mCapacity = lSize;
        mValue = lValue;
        CheckValue();
    }

    public bool IsFull()
    {
        if (VacantCapacity() == 0)
            return true;
        return false;
    }

    public bool IsEmpty()
    {
        if (mValue == 0)
            return true;
        return false;
    }

    public float VacantCapacity()
    {
        return mCapacity - mValue;
    }

    public static Pool operator +(Pool lPool, float lAmount)
    {
        if (lPool == null)
            return null;

        lPool.Add(lAmount);
        return lPool;
    }

    public static Pool operator -(Pool lPool, float lAmount)
    {
        if (lPool == null)
            return null;

        lPool.Add(-lAmount);
        return lPool;
    }

    public static float operator +(float lAmount, Pool lPool)
    {
        return lAmount + lPool.mValue;
    }

    public static float operator -(float lAmount, Pool lPool)
    {
        return lAmount - lPool.mValue;
    }

    public static bool operator >(Pool lPool, float lAmount)
    {
        return lPool.mValue > lAmount;
    }

    public static bool operator <(Pool lPool, float lAmount)
    {
        return lPool.mValue < lAmount;
    }

    public static bool operator ==(Pool lPool, float lAmount)
    {
        if (object.ReferenceEquals(lPool, null))
        {

            return false;

        }

        return lPool.mValue == lAmount;
    }
    public static bool operator !=(Pool lPool, float lAmount)
    {
        if (object.ReferenceEquals(lPool, null))
        {
            return true;
        }

        return lPool.mValue != lAmount;
    }

    public static bool operator ==(Pool lPool, Pool lPool2)
    {
        if (object.ReferenceEquals(lPool, null))
        {
            if (object.ReferenceEquals(lPool2, null))
                return true;
            else
                return false;
        }

        if (object.ReferenceEquals(lPool2, null))
            return false;

        return lPool.mValue == lPool2.mValue;
    }
    public static bool operator !=(Pool lPool, Pool lPool2)
    {

        if (object.ReferenceEquals(lPool, null))
        {
            if (object.ReferenceEquals(lPool2, null))
                return false;
            else
                return true;
        }

        if (object.ReferenceEquals(lPool2, null))
            return true;

        return lPool.mValue != lPool2.mValue;
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public virtual bool Equals(Pool lPool)
    {
        return  mValue == lPool.mValue;
    }

    public virtual bool Equals(float lValue)
    {
        return mValue == lValue ;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return Math.Round(mValue, 2) + " / " + Math.Round(mCapacity, 2);
    }

    public static implicit operator float(Pool lPool)  
    {
        return lPool.mValue;  
    }

    public static explicit operator int(Pool d)
    {
        return (int)d.mValue;
    }

    public void SetValue(float lValue)
    {
        mValue = lValue;
        CheckValue();
    }

}
