using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Traits
{
    Organic,
    Electronic,
    Indestructable,
}

[SerializeField]
public class Vitals {

    // 24 bytes
    private Pool mArmor;
    private Pool mShield;
    private Pool mHealth;

    public Vitals(float lHealth)
    {
        mHealth = new Pool(lHealth);
        mShield = new Pool(10, 0);
        mArmor = new Pool(10, 0);
    }

    public bool IsDead()
    {
        if (mHealth == 0)
            return true;
        else
            return false;
    }

    public Vitals TakeDamage(float lDamage)
    {
        return null;
    }
        //    if (mShield == null)
        //    {
        //        //Debug.Log("No Shield value set");
        //    }
        //    else
        //    {
        //        if (mShield > damage)
        //        {
        //            mShield -= damage;
        //            return;
        //        }
        //        else
        //        {
        //            if (mShield > 0)
        //            {
        //                mShield -= damage;
        //                damage = 0;
        //            }
        //        }
        //    }

        //    if (mHealth == null)
        //    {
        //        Debug.Log("No Health Value is set");
        //        Die();
        //        return;
        //    }

        //    mHealth -= damage;

        //    if (mHealth <= 0 && !mLiving)
        //    {
        //        Die();
        //    }
        //}


    }
