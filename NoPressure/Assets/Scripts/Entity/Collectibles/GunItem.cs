using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunItem : IInventoryItem {

    // TODO: Make sure you cant reload and fire at the same time
    // TODO: Set and make use of the bullet speed attribute
    // TODO: Be able to set ReloadSpeed, FireRate, and ChargeTimes in constructer

    // The display icon of the item
    public Sprite mDisplaySprite { get; protected set; }

    // The MonoBehaviour object that is currenly using this item
    public Transform mEquipedBy { get; protected set; }

    // The amount of uses the item has
    public ResourcePool mCharges { get; protected set; }

    public enum FireMode { Auto, Single, Charge };
    
    // Features of a gun
    float mBulletSpeed;
    FireMode mFireMode;
    int mDamage;

    // Cooldown time lengths In Seconds
    float mReloadSpeed = 0.5f;
    float mFireRate = 0.2f;
    float mMaxChargeTime = 5f;
    float mMinChargeTime = 0.2f;

    // Cooldown time state
    float mTimeSinceLastShot = 0f;
    float mTotalChargeTime = 0f;
    float mTimeSpentReloading = 0f;
    bool mIsReloading = false;
    bool mIsTriggerDown = false;
    bool mIsTriggerReleased = true;
    
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

    public IInventoryItem Clone()
    {
        return new GunItem(mDisplaySprite, mDamage, mCharges, mFireMode);
    }

    // Must be called by some MonoBehavior Object every frame to keep track of cooldowns
    // like reload speed and weapon charge time.
    public void UpdateTime()
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
    public bool Reload(ResourcePool ammoStorage)
    {
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
        mCharges.Transfer(ammoStorage);
        AbortReload();
        return true;
    }

    // Reset the reload timer
    public void AbortReload()
    {
        mTimeSpentReloading = 0f;
        mIsReloading = false;
    }

    // Reload helper function, will return false if we are unable to reload 
    private bool CanReload(ResourcePool ammoStorage)
    {
        // there is not enough ammo to reload 
        if (ammoStorage.IsEmpty() == true)
        {
            Debug.Log("ammo storage is : " + ammoStorage);
            return false;
        }

        // There is no need to reload because we have a full clip
        if (mCharges.IsFull() == false)
        {
            Debug.Log("Full clip");
            return false;
        }

        // Set the flag that we are currenly reloading.
        mIsReloading = true;
        return true;
    }

    public void UnSelect()
    {
        //mEquipedBy = null;
    }

    // Somthing has euiped this item, get a refrence to it so we can spawn
    // bullets at its location, modify the user, or get other information.
    public void Select(Transform lEquipedBy)
    {
        mEquipedBy = lEquipedBy;
    }

    // User released trigger button
    public void AbortUse()
    {
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
    public void Use()
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
                    mTimeSinceLastShot -= mFireRate;
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
            Debug.Log("Out of ammo in clip");
            return;
        }
        LayerMask CollisionMask = LayerMask.GetMask(new string[] {"Entity", "Obstacle" });

        // Create bullet
        ProjectileManager.getProjectileManager().SpawnProjectile
            (Color.red, 10, 3, CollisionMask, mEquipedBy, onHitEffect);

        mCharges -= 1;
    }

    // Function for delegate call, Action to take on the victim of the bullet 
    // collision
    private bool onHitEffect(Transform TargetHit,Vector3 hitPoint, Vector3 hitDirection)
    {
        IDamageable damageableObject = TargetHit.GetComponent<IDamageable>();

        if (damageableObject == null)
        {
            Debug.Log("Colision with a non damagable target, refine the collision mask");
            return false; 
        }

        // Cause damage to the target
        damageableObject.TakeHit(mDamage, hitPoint, mEquipedBy.forward);
        return true;
    }
}
