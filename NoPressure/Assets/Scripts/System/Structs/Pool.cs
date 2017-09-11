using System;

//Immutable
[Serializable]
public struct Pool
{
    public readonly float mCapacity;
    public readonly float mValue;

    public Pool(float lCapacity)
    {
        // Pools must have a positive size
        if (lCapacity < 0)
        {
            throw new ArgumentOutOfRangeException(
                "lCapacity",
                lCapacity,
                "Pool size must be a positive number");
        }

        mCapacity = lCapacity;
        mValue = lCapacity;
    }

    public Pool(float lCapacity, float lValue)
    {
        // Pools must have a positive size
        if (lCapacity < 0)
        {
            throw new ArgumentOutOfRangeException(
                "lCapacity",
                lCapacity,
                "Pool size must be a positive number");
        }
        mCapacity = lCapacity;
        mValue = lValue;
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

    public float GetCapacity()
    {
        return mCapacity;
    }

    public Pool IncreaseCapacity(float lAmount)
    {
        try
        {
            return new Pool(mCapacity + lAmount, mValue);
        }
        catch (ArgumentOutOfRangeException e)
        {
            return new Pool(0);
        }
    }

    public Pool DecreaseCapacity(float lAmount)
    {
        return IncreaseCapacity(lAmount * -1);
    }

    public Pool Add(float lAmount)
    {
        return new Pool(mCapacity, mValue + lAmount);
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

    public Pool SetValue(float lValue)
    {
        return new Pool(mCapacity, lValue);
    }


    public static void Transfer(ref Pool rTransferFrom, ref Pool rTransferTo)
    {
        float transferAmount = 0;

        if (rTransferTo.VacantCapacity() > rTransferFrom.mValue)
            transferAmount = rTransferFrom.mValue;
        else
            transferAmount = rTransferTo.VacantCapacity();

        rTransferFrom = new Pool(rTransferFrom.mCapacity, rTransferFrom - transferAmount);
        rTransferTo = new Pool(rTransferTo.mCapacity, rTransferTo + transferAmount);
    }

    public static void Transfer(ref Pool rTransferFrom, ref Pool rTransferTo, float lAmount)
    {
        float transferAmount = 0;

        if (rTransferTo.VacantCapacity() > rTransferFrom.mValue)
            transferAmount = rTransferFrom.mValue;
        else
            transferAmount = rTransferTo.VacantCapacity();

        if (transferAmount > lAmount)
            transferAmount = lAmount;

        rTransferFrom = new Pool(rTransferFrom.mCapacity, rTransferFrom - transferAmount);
        rTransferTo = new Pool(rTransferTo.mCapacity, rTransferTo + transferAmount);
    }

    public static Pool operator ++(Pool lPool)
    {
        return new Pool(lPool.mCapacity, lPool.mValue + 1);
    }

    public static Pool operator --(Pool lPool)
    {
        return new Pool(lPool.mCapacity, lPool.mValue - 1);
    }

    public static Pool operator +(Pool lPool, float lAmount)
    {
        return new Pool(lPool.mCapacity, lPool.mValue + lAmount);
    }

    public static Pool operator -(Pool lPool, float lAmount)
    {
        return new Pool(lPool.mCapacity, lPool.mValue - lAmount);
    }

    public static Pool operator *(Pool lPool, float lAmount)
    {
        return new Pool(lPool.mCapacity, lPool.mValue * lAmount);
    }

    public static Pool operator /(Pool lPool, float lAmount)
    {
        return new Pool(lPool.mCapacity, lPool.mValue / lAmount);
    }

    public static float operator +(float lAmount, Pool lPool)
    {
        return lAmount + lPool.mValue;
    }

    public static float operator -(float lAmount, Pool lPool)
    {
        return lAmount - lPool.mValue;
    }

    public static float operator *(float lAmount, Pool lPool)
    {
        return lAmount * lPool.mValue;
    }

    public static float operator /(float lAmount, Pool lPool)
    {
        return lAmount / lPool.mValue;
    }

    public static Pool operator +(Pool lPool1, Pool lPool2)
    {
        return new Pool(lPool1.mCapacity, lPool1.mValue + lPool2.mValue);
    }

    public static Pool operator -(Pool lPool1, Pool lPool2)
    {
        return new Pool(lPool1.mCapacity, lPool1.mValue - lPool2.mValue);
    }

    public static Pool operator *(Pool lPool1, Pool lPool2)
    {
        return new Pool(lPool1.mCapacity, lPool1.mValue * lPool2.mValue);
    }

    public static Pool operator /(Pool lPool1, Pool lPool2)
    {
        return new Pool(lPool1.mCapacity, lPool1.mValue / lPool2.mValue);
    }

    public static bool operator >(Pool lPool, float lAmount)
    {
        return lPool.mValue > lAmount;
    }

    public static bool operator <(Pool lPool, float lAmount)
    {
        return lPool.mValue < lAmount;
    }

    public static bool operator >(float lAmount, Pool lPool)
    {
        return lAmount > lPool.mValue;
    }

    public static bool operator <(float lAmount, Pool lPool)
    {
        return lAmount < lPool.mValue;
    }

    public static bool operator >(Pool lPool1, Pool lPool2)
    {
        return lPool1.mValue > lPool2.mValue;
    }

    public static bool operator <(Pool lPool1, Pool lPool2)
    {
        return lPool1.mValue < lPool2.mValue;
    }

    public static bool operator >=(Pool lPool, float lAmount)
    {
        return lPool.mValue >= lAmount;
    }

    public static bool operator <=(Pool lPool, float lAmount)
    {
        return lPool.mValue <= lAmount;
    }

    public static bool operator >=(float lAmount, Pool lPool)
    {
        return lAmount >= lPool.mValue;
    }

    public static bool operator <=(float lAmount, Pool lPool)
    {
        return lAmount <= lPool.mValue;
    }

    public static bool operator >=(Pool lPool1, Pool lPool2)
    {
        return lPool1.mValue >= lPool2.mValue;
    }

    public static bool operator <=(Pool lPool1, Pool lPool2)
    {
        return lPool1.mValue <= lPool2.mValue;
    }

    public static bool operator ==(Pool lPool, float lAmount)
    {
        return lPool.mValue == lAmount;
    }

    public static bool operator !=(Pool lPool, float lAmount)
    {
        return lPool.mValue != lAmount;
    }

    public static bool operator ==(float lAmount, Pool lPool2)
    {
        return lAmount == lPool2.mValue;
    }

    public static bool operator !=(float lAmount, Pool lPool2)
    {
        return lAmount == lPool2.mValue;
    }

    public static bool operator ==(Pool lPool, Pool lPool2)
    {
        return lPool.mValue == lPool2.mValue;
    }

    public static bool operator !=(Pool lPool, Pool lPool2)
    {
        return lPool.mValue != lPool2.mValue;
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public bool Equals(Pool lPool)
    {
        return mValue == lPool.mValue;
    }

    public bool Equals(float lValue)
    {
        return mValue == lValue;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            // Choose large primes to avoid hashing collisions
            const int HashingBase = (int)2166136261;
            const int HashingMultiplier = 16777619;

            int hash = HashingBase;
            hash = (hash * HashingMultiplier) ^ (int)(mCapacity * 10000);
            hash = (hash * HashingMultiplier) ^ (int)(mValue * 10000);
            return hash;
        }
    }

    public override string ToString()
    {
        return Math.Round(mValue, 2) + " / " + Math.Round(mCapacity, 2);
    }

    public static implicit operator float(Pool lPool)
    {
        return lPool.mValue;
    }

    public static explicit operator int(Pool lPool)
    {
        return (int)lPool.mValue;
    }

}
