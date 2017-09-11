using System.Collections;
using System.Collections.Generic;
using System;

// Consumeable resource types that can refill the charges of items
public enum ResourceType
{
    None,
    MassDriver,
    EnergyCell,
    Explosive,
    Parts,
    Charges,
}

// Immutable
[Serializable]
public struct ResourcePool
{
    private const string INCOMPATABLE_RESOURCE_ERROR_STRING = 
        "Unable to transfer resources of different types";

    private ResourceType mResourceType;
    private Pool mPool;

    public ResourcePool(
        ResourceType lResourceType, 
        float lCapacity) 
    {
        mResourceType = lResourceType;
        mPool = new Pool(lCapacity);
    }

    public ResourcePool(
        ResourceType lResourceType,
        float lCapacity,
        float lValue)
    {
        mResourceType = lResourceType;
        mPool = new Pool(lCapacity,lValue);
    }

    public ResourcePool(
        ResourceType lResourceType,
        Pool lPool)
    {
        mResourceType = lResourceType;
        mPool = lPool;
    }

    public ResourceType GetResourceType()
    {
        return mResourceType;
    }

    public bool IsEmpty()
    {
        return mPool.IsEmpty();
    }

    public bool IsFull()
    {
        return mPool.IsFull();
    }

    public static bool IsSameResourceType(ResourcePool lPool1, ResourcePool lPool2)
    {
        if (lPool1.mResourceType == lPool2.mResourceType)
            return true;
        else
            return false;
    }

    public static bool Transfer(ref ResourcePool rTransferFrom, ref ResourcePool rTransferTo)
    {
        // Check to only allow exchange of resource of compatable types
        if (!IsSameResourceType(rTransferFrom,rTransferTo))
        {
            return false;
        }

        Pool.Transfer(ref rTransferFrom.mPool, ref rTransferTo.mPool);
        return true;
    }

    public static bool Transfer(ref ResourcePool rTransferFrom, ref ResourcePool rTransferTo, float lAmount)
    {
        // Check to only allow exchange of resource of compatable types
        if (!IsSameResourceType(rTransferFrom, rTransferTo))
        {
            return false;
        }

        Pool.Transfer(ref rTransferFrom.mPool, ref rTransferTo.mPool,lAmount);
        return true;
    }

    public static ResourcePool Add(ResourcePool lResourcePool1, ResourcePool lResourcePool2)
    {
        // Resources of different types cannot be added together
        if (!IsSameResourceType(lResourcePool1, lResourcePool2))
        {
            throw new InvalidOperationException(
                INCOMPATABLE_RESOURCE_ERROR_STRING);
        }

        return new ResourcePool(lResourcePool1.mResourceType, new Pool(lResourcePool1.mPool.GetCapacity(), lResourcePool1.mPool + lResourcePool2.mPool));
    }


    public static ResourcePool Subtract(ResourcePool lResourcePool1, ResourcePool lResourcePool2)
    {
        // Resources of different types cannot be added together
        if (!IsSameResourceType(lResourcePool1, lResourcePool2))
        {
            throw new InvalidOperationException(
                INCOMPATABLE_RESOURCE_ERROR_STRING);
        }

        return new ResourcePool(lResourcePool1.mResourceType, new Pool(lResourcePool1.mPool.GetCapacity(), lResourcePool1.mPool - lResourcePool2.mPool));
    }

    public static ResourcePool Add (ResourcePool lResourcePool1, float lAmount)
    {
        return new ResourcePool(lResourcePool1.mResourceType, new Pool(lResourcePool1.Capacity(), lResourcePool1 + lAmount));
    }

    public static ResourcePool Subtract(ResourcePool lResourcePool1, float lAmount)
    {
        return new ResourcePool(lResourcePool1.mResourceType, new Pool(lResourcePool1.Capacity(), lResourcePool1 - lAmount));
    }

    public ResourcePool SetValue(float lAmount)
    {
        return new ResourcePool(mResourceType, new Pool(Capacity(), lAmount));
    }

    public static ResourcePool operator + (ResourcePool lResourcePool1, ResourcePool lResourcePool2)
    {
        return Add(lResourcePool1,lResourcePool2);
    }

    public static ResourcePool operator - (ResourcePool lResourcePool1, ResourcePool lResourcePool2)
    {
        return Subtract(lResourcePool1, lResourcePool2);
    }

    public static ResourcePool operator + (ResourcePool lResourcePool1, float lAmount)
    {
        return new ResourcePool(lResourcePool1.mResourceType, new Pool(lResourcePool1.Capacity(), lResourcePool1 + lAmount));
    }

    public static ResourcePool operator - (ResourcePool lResourcePool1, float lAmount)
    {
        return new ResourcePool(lResourcePool1.mResourceType, new Pool(lResourcePool1.Capacity(), lResourcePool1 - lAmount));
    }

    public float Capacity()
    {
        return mPool.mCapacity;
    }

    public static bool operator > (ResourcePool lResourcePool1, ResourcePool lResourcePool2)
    {
        //if (!lResourcePool1.IsSameResource(lResourcePool2))
        //{
        //    throw new InvalidOperationException(
        //        INCOMPATABLE_RESOURCE_ERROR_STRING);
        //}
        return lResourcePool1.mPool > lResourcePool2.mPool;
    }

    public static bool operator < (ResourcePool lResourcePool1, ResourcePool lResourcePool2)
    {
        return lResourcePool2 > lResourcePool1;
    }

    public static bool operator > (ResourcePool lResourcePool, float lAmount)
    {
        return lResourcePool.mPool > lAmount;
    }

    public static bool operator < (ResourcePool lResourcePool, float lAmount)
    {
        return lResourcePool.mPool < lAmount;
    }

    public static bool operator >= (ResourcePool lResourcePool, float lAmount)
    {
        return lResourcePool.mPool >= lAmount;
    }

    public static bool operator <= (ResourcePool lResourcePool, float lAmount)
    {
        return lResourcePool.mPool <= lAmount;
    }

    public static bool operator == (ResourcePool lResourcePool1, ResourcePool lResourcePool2)
    {
        if (!IsSameResourceType(lResourcePool1, lResourcePool2))
        {
            return false;
        }

        return lResourcePool1.mPool == lResourcePool2.mPool;
    }

    public static bool operator != (ResourcePool lResourcePool1, ResourcePool lResourcePool2)
    {
        return !(lResourcePool1 == lResourcePool2);
    }

    public static bool operator == (ResourcePool lResourcePool, float lAmount)
    {
        return lResourcePool.mPool == lAmount;
    }

    public static bool operator != (ResourcePool lResourcePool, float lAmount)
    {
        return lResourcePool.mPool != lAmount;
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public bool Equals(ResourcePool lResourcePool2)
    {
        return this == lResourcePool2;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            // Choose large primes to avoid hashing collisions
            const int HashingBase = (int)2166136261;
            const int HashingMultiplier = 16777619;

            int hash = HashingBase;
            hash = (hash * HashingMultiplier) ^ mPool.GetHashCode();
            hash = (hash * HashingMultiplier) ^ (int)(mResourceType) * 10000;
            return hash;
        }
    }

    public override string ToString()
    {
        return mResourceType + " : " + mPool.ToString();
    }

    public static implicit operator float(ResourcePool d)
    {
        return d.mPool;
    }

    public static explicit operator int(ResourcePool d)
    {
        return (int)d.mPool;
    }

}