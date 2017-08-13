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

public class Pool
{
    public float mCapacity { protected set; get; }
    public float mValue { protected set; get; }

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
        lPool.Add(lAmount);
        return lPool;
    }

    public static Pool operator -(Pool lPool, float lAmount)
    {
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

// Representation for the amount of a resource left and its capacity
public class ResourcePool : Pool 
{
    public ResourceType mResourceType { private set; get; }

    public ResourcePool(ResourceType lResourceType, float lCapacity) : base (lCapacity)
    {
        mResourceType = lResourceType;
    }

    public ResourcePool(
        ResourceType lResourceType, 
        float lCapacity, 
        float lValue) : base(lCapacity, lValue)
    {
        mResourceType = lResourceType;
    }

    // Transfer as many resources as possible from lTransferFrom to lTransferTo
    public void Transfer(ResourcePool lTransferFrom)
    {
        if (mResourceType != lTransferFrom.mResourceType)
        {
            Debug.Log("Warning, Trying to transfer incompatable resources types");
            return;
        }

        float transferAmount = 0;

        if (VacantCapacity() > lTransferFrom.mValue)
            transferAmount = lTransferFrom.mValue;
        else
            transferAmount = VacantCapacity();

        mValue += transferAmount;
        lTransferFrom -= transferAmount;
    }

    public static ResourcePool operator +(ResourcePool lResourcePool, ResourcePool lResourcePool2)
    {
        // Resources of different types cannot be added together
        if (lResourcePool.mResourceType != lResourcePool2.mResourceType)
        {
            Debug.Log("Warning, Trying to add resource of different types");
            return lResourcePool;
        }
        lResourcePool.Add(lResourcePool2.mValue);
        return lResourcePool;
    }

    public static ResourcePool operator -(ResourcePool lResourcePool, ResourcePool lResourcePool2)
    {
        // Resources of different types cannot be added together
        if (lResourcePool.mResourceType != lResourcePool2.mResourceType)
        {
            Debug.Log("Warning, Trying to add resource of different types");
            return lResourcePool;
        }
        lResourcePool.Add(-lResourcePool2.mValue);
        return lResourcePool;
    }

    public static ResourcePool operator +(ResourcePool lResourcePool, float lAmount)
    {
        lResourcePool.Add(lAmount);
        return lResourcePool;
    }

    public static ResourcePool operator -(ResourcePool lResourcePool, float lAmount)
    {
        //Debug.Log("" + lResourcePool.mValue + " - " + lAmount);  
        lResourcePool.Add(-lAmount);
        return lResourcePool;
    }

    public static bool operator >(ResourcePool lResourcePool, ResourcePool lResourcePool2)
    {
        if (lResourcePool.mResourceType != lResourcePool2.mResourceType)
        {
            Debug.Log("Warning, Trying to compare two different resource types");
            return false;
        }
        return lResourcePool.mValue > lResourcePool2.mValue;
    }

    public static bool operator <(ResourcePool lResourcePool, ResourcePool lResourcePool2)
    {
        if (lResourcePool.mResourceType != lResourcePool2.mResourceType)
        {
            Debug.Log("Warning, Trying to compare two different resource types");
            return false;
        }
        return lResourcePool.mValue < lResourcePool2.mValue;
    }

    public static bool operator >(ResourcePool lResourcePool, float lAmount)
    {
        return lResourcePool.mValue > lAmount;
    }

    public static bool operator <(ResourcePool lResourcePool, float lAmount)
    {
        return lResourcePool.mValue < lAmount;
    }

    public static bool operator >=(ResourcePool lResourcePool, float lAmount)
    {
        return lResourcePool.mValue >= lAmount;
    }

    public static bool operator <=(ResourcePool lResourcePool, float lAmount)
    {
        return lResourcePool.mValue <= lAmount;
    }

    public static bool operator ==(ResourcePool lResourcePool, ResourcePool lResourcePool2)
    {
        if (object.ReferenceEquals(lResourcePool, null))
        {
            if (object.ReferenceEquals(lResourcePool2, null))
                return true;
            else
                return false;
        }

        if (lResourcePool2 == null)
            return false;

        if (lResourcePool.mResourceType != lResourcePool2.mResourceType)
        {
            Debug.Log("Warning, Trying to compare two different resource types");
            return false;
        }
        return lResourcePool.mValue == lResourcePool2.mValue;
    }

    public static bool operator !=(ResourcePool lResourcePool, ResourcePool lResourcePool2)
    {
        if (object.ReferenceEquals(lResourcePool,null))
        {
            if (object.ReferenceEquals(lResourcePool2, null))
                return false;
            else
                return true;
        }

        if (lResourcePool2 == null)
            return true;

        if (lResourcePool.mResourceType != lResourcePool2.mResourceType)
        {
            Debug.Log("Warning, Trying to compare two different resource types");
            return true;
        }
        return lResourcePool.mValue != lResourcePool2.mValue;
    }

    public static bool operator ==(ResourcePool lResourcePool, float lAmount)
    {
        return lResourcePool.mValue == lAmount;
    }

    public static bool operator !=(ResourcePool lResourcePool, float lAmount)
    {
        return lResourcePool.mValue != lAmount;
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public bool Equals(ResourcePool lResourcePool2) 
    {
        return (this.Equals(lResourcePool2));
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return Math.Round(mValue, 2) + " / " + Math.Round(mCapacity,2);
    }

    public static implicit operator float(ResourcePool d)
    {
        return d.mValue;
    }

    public static explicit operator int(ResourcePool d)
    {
        return (int)d.mValue;
    }


}