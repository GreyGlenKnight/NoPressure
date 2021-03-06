﻿using UnityEngine;

// Everything that can Die() or be destroyed.
public class PersistentEntity : MonoBehaviour, IDamageable {

    // Event to be fired off when PersistentEntity dies
    public event System.Action<Transform> mOnDeathHandler;

    // Event to be fired off when PersistentEntity is despawned
    public event System.Action<Transform> mOnDespawn;

    // Public Set variables
    public int pMaxHealth = 1;

    // Can not die from damage
    public bool pIsIndestructable = false;

    // Organic Units need oxygen to breath
    public bool pIsOrganic = false;

    // can be effected by EMP weapons
    public bool pIsElectronic = false;

    // Is Tile
    public bool pIsFloor = false;

    // Resources, TODO drop when killed
    public Inventory mInventory;

    public SpawnType spawnType { get; set; }

    public MapSector SectorSpawn{ get; private set; }
    public Coord TileLocationInSector { get; private set; }

    public void SetSpawnPoint(MapSector sector, Coord Tile)
    {
        SectorSpawn = sector;
        TileLocationInSector = Tile;
    }

    // Vitals
    public Pool mPressure;
    public Pool mArmor;
    public Pool mShield;
    public Pool mHealth;
    public float mPressureLeakRate;

    // Skills
    //public bool mHasFixPressureSkill {
    //    get
    //    {
    //        return mInventory.mHasFixPressureSkill;
    //    }

    //}
    //public bool mHasDisableTrapSkill {
    //    get
    //    {
    //        return mInventory.mHasDisableTrapSkill;
    //    }
    //}

    // After something dies it may still have actions it can take
    public bool mDead { get; protected set; } 
    
    // virtual allows the Start method to be called in derived classes
    // For example base.Start()
    // This ensures behaviour common to all derived classes are implemented
    protected virtual void Start()
    {
        if (pMaxHealth <= 0)
            pMaxHealth = 1;

        //mHealth = new Pool(pMaxHealth);
        //mShield = new Pool(10, 0);
        //mPressure = new Pool(30);
        //mInventory = new Inventory(4);
    }

    // TODO add delegate peram to cause a side effect
    //public virtual void TakeHit(float damage,  Vector3 hitPoint, Vector3 hitDirection)
    //{
    //    TakeDamage(damage);
    //}

    public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        TakeDamage(damage);
    }

    public virtual void TakeDamage(float damage)
    {
        if (mShield == null)
        {
            //Debug.Log("No Shield value set");
        }
        else
        {
            if (mShield > damage)
            {
                mShield -= damage;
                return;
            }
            else
            {
                if (mShield > 0)
                {
                    mShield -= damage;
                    damage = 0;
                }
            }
        }


        if(mHealth == null)
        {
            Debug.Log("No Health Value is set");
            Die();
            return;
        }

        mHealth -= damage;

        if (mHealth <= 0 && !mDead)
        {
            Die();
        }
    }

    // What to do when this entiry has despawned from being to far away
    public virtual void Despawn()
    {
        if (mOnDespawn != null)
            mOnDespawn(transform);
    }

    public virtual void Die()
    {
        mDead = true;

        // If there are any on death triggers, call them
        if (mOnDeathHandler != null)
            mOnDeathHandler(transform);

        //If it dies remove it from the Sector
        if (SectorSpawn == null)
        {
            //Debug.Log("Set the Sector of the spawn");
        }
        else
        {

        }
    }

    // This method assumes the amount is positive
    public void UpdateHealth(int amount)
    {
        mHealth += amount;
    }

}
