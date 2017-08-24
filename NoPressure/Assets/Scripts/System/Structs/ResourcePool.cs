using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ResourcePool : Pool
{
    public ResourceType mResourceType;

    public ResourcePool(ResourceType lResourceType, float lCapacity) : base(lCapacity)
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
        if (object.ReferenceEquals(lResourcePool, null))
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
        return Math.Round(mValue, 2) + " / " + Math.Round(mCapacity, 2);
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