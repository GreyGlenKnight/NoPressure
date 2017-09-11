using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunItem : IInventoryItem {

    // TODO: Make sure you cant reload and fire at the same time
    // TODO: Set and make use of the bullet speed attribute

    public enum FireMode { Auto, Single, Charge };
    
    // Features of a gun
    public float mBulletSpeed;
    public FireMode mFireMode;
    public int mDamage;

    // Cooldown time lengths In Seconds
    public float mReloadSpeed = 0.5f;
    public float mFireRate = 0.2f;
    public float mMaxChargeTime = 5f;
    public float mMinChargeTime = 0.2f;

    public float explosionRange = 0f;
    public float explosionDamage = 2f;

    // Cooldown time state
    float mTimeSinceLastShot = 0f;
    float mTotalChargeTime = 0f;
    float mTimeSpentReloading = 0f;
    bool mIsReloading = false;
    bool mIsTriggerDown = false;
    bool mIsTriggerReleased = true;

    public Material ShotMateral;
    public Material explosionMateral;
    public Color ShotShine;

    public GunItem(
        Sprite lDisplaySprite, 
        int lDamage, 
        ResourcePool lCharges, 
        FireMode lFireMode)
    {
        mDisplaySprite = lDisplaySprite;
        mDamage = lDamage;
        mCharges = lCharges;
        mFireMode = lFireMode;
    }

    public override IInventoryItem Clone()
    {
        return new GunItem(mDisplaySprite, mDamage, mCharges, mFireMode);
    }

    // Must be called by some MonoBehavior Object every frame to keep track of cooldowns
    // like reload speed and weapon charge time.
    public override void UpdateTime()
    {
        // Store deltatime in a local var to reduce number of calls
        float timeSinceLastFrame = Time.deltaTime;

        // Keep track of the last time the user shot.
        mTimeSinceLastShot += timeSinceLastFrame;

        // If user is currently holding the reload button increase the reload timer
        if (mIsReloading == true)
        {
            mTimeSpentReloading += timeSinceLastFrame;
        } else
        {
            //Sanity Check
            if (mTimeSpentReloading != 0f)
            {
                Debug.Log("Warning, do not set isReloading to false without reseting reload time");
                mTimeSpentReloading = 0f;
            }
        }

        // If this is a charge type weapon and the user is holding down the Use button
        // increase the total time charged
        if (mFireMode == FireMode.Charge)
        {
            if (mIsTriggerDown == true)
            {
                mTotalChargeTime += timeSinceLastFrame;
            } else
            {
                // Sanity Check
                if (mTotalChargeTime != 0f)
                {
                    Debug.Log("Warning, do not set isTriggerDown to false without reseting charge time");
                    mTotalChargeTime = 0f;
                }
            }
        }
    }

    // Return True if we successful reloaded. 
    // because this requires time to complete it must be called several times
    // over multiple frames until the reload time has elapsed.
    public override bool Reload(ResourcePool ammoStorage)
    {
        base.Reload(ammoStorage);

        // If we are unable to reload for any reason abort the reload
        if (CanReload(ammoStorage) == false)
        {
            AbortReload();
            return false;
        }

        // It takes some time to reload, we can not reload until we have been
        // reloading for longer than the reload speed of the gun
        if (mTimeSpentReloading < mReloadSpeed)
            return false;

        // Reload the weapon and then stop and reset the reload timer
        ResourcePool.Transfer(ref ammoStorage,ref mCharges);
        AbortReload();
        return true;
    }

    // Reset the reload timer
    public override void AbortReload()
    {
        base.AbortReload();
        mTimeSpentReloading = 0f;
        mIsReloading = false;
    }

    // Reload helper function, will return false if we are unable to reload 
    private bool CanReload(ResourcePool ammoStorage)
    {
        if (ammoStorage == null)
            return false;

        // there is not enough ammo to reload 
        if (ammoStorage.IsEmpty() == true)
        {
            Debug.Log("ammo storage is : " + ammoStorage);
            return false;
        }

        // There is no need to reload because we have a full clip
        if (mCharges.IsFull() == true)
        {
            return false;
        }

        // Set the flag that we are currenly reloading.
        mIsReloading = true;
        return true;
    }

    public override void UnSelect()
    {
        //mEquipedBy = null;
    }

    // Somthing has euiped this item, get a refrence to it so we can spawn
    // bullets at its location, modify the user, or get other information.
    public override void Select(Transform lEquipedBy)
    {
        base.Select(lEquipedBy);
        mEquipedBy = lEquipedBy;
    }

    // User released trigger button
    public override void AbortUse()
    {
        base.AbortUse();
        // Charge weapons fire when button is released
        if (mFireMode == FireMode.Charge)
        {
            // Trigger must be held down for at least Fire Rate Seconds
            if (mTotalChargeTime> mMinChargeTime)
            {
                FireWeapon();
            }
        }
        mIsTriggerReleased = true;
        mIsTriggerDown = false;
        mTotalChargeTime = 0f;
    }

    // User presses the trigger button
    public override void Use()
    {
        // Single fire weapons fire once per trigger press
        if (mFireMode == FireMode.Single)
        {
            if (mIsTriggerReleased == true)
            {
                mIsTriggerReleased = false;
                mIsTriggerDown = true;

                if (mTimeSinceLastShot > mFireRate)
                {
                    FireWeapon();
                    mTimeSinceLastShot = 0f;
                }
            }
        }

        // Auto weapons fire every mFireRate seconds
        if (mFireMode == FireMode.Auto)
        {
            if (mIsTriggerReleased == true)
            {
                mIsTriggerReleased = false;
                mIsTriggerDown = true;

                if (mTimeSinceLastShot > mFireRate)
                {
                    FireWeapon();
                    mTimeSinceLastShot = 0f;
                }
            }
            else
            {
               // Give auto a smoother(less dependant on frame Rate) rate of fire if trigger is held down
                if (mTimeSinceLastShot > mFireRate)
                {
                    FireWeapon();
                    mTimeSinceLastShot = 0f;
                }
            }
        }
        if (mFireMode == FireMode.Charge)
        {
            // Increase the charge of Charge weapons during UpdateTime(),
            // if there is enough charge when the user releases the trigger,
            // the gun will fire
            mIsTriggerReleased = false;
            mIsTriggerDown = true;
        }
    }

    // Create a projectile and reduce ammo by 1
    private void FireWeapon()
    {
        if (mCharges == 0)
        {
            return;
        }
        LayerMask CollisionMask = LayerMask.GetMask(new string[] {"Entity", "Obstacle" });



    // Create bullet
    ProjectileManager.getProjectileManager().SpawnProjectile
            (ShotMateral,explosionMateral,ShotShine, 10, 3, CollisionMask, mEquipedBy, onHitEffect, explosionRange, explosionDamage);

        mCharges -= 1;
        if (mUseSFX != null)
            SoundManager.instance.RandomizeSfx(mUseSFX); 
    }

    // Function for delegate call, Action to take on the victim of the bullet 
    // collision
    private bool onHitEffect(Transform TargetHit,Vector3 hitPoint, Vector3 hitDirection)
    {
        IDamageable damageableObject = TargetHit.GetComponent<IDamageable>();

        if (damageableObject == null)
        {
            //Debug.Log("Colision with a non damagable target, refine the collision mask");
            return false; 
        }

        // Cause damage to the target
        damageableObject.TakeHit(mDamage, hitPoint, mEquipedBy.forward);
        return true;
    }

    public override List<ISkill> GetSkillsFromItem()
    {
        return null;
    }
}
