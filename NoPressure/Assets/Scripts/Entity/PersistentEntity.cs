using UnityEngine;

// Everything that can Die() or be destroyed.
public class PersistentEntity : MonoBehaviour, IDamageable {

    // Event to be fired off when LivingEntity dies
    public event System.Action mOnDeathHandler;

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
    protected Inventory mInventory;

    public SpawnType spawnType { get; set; }

    public MapSector SectorSpawn{ get; private set; }
    public Coord TileLocationInSector { get; private set; }

    public void SetSpawnPoint(MapSector sector, Coord Tile)
    {
        SectorSpawn = sector;
        TileLocationInSector = Tile;
    }

    // Vitals
    public Pool mPressure { get; set; }
    public Pool mArmor { get; set; }
    public Pool mShield { get; set; }
    public Pool mHealth { get; set; }
    public float mPressureLeakRate { get; set; } 

    // Skills
    public bool mHasFixPressureSkill {
        get
        {
            return mInventory.mHasFixPressureSkill;
        }

    }
    public bool mHasDisableTrapSkill {
        get
        {
            return mInventory.mHasDisableTrapSkill;
        }
    }

    // After something dies it may still have actions it can take
    public bool mDead { get; protected set; } 
    
    // virtual allows the Start method to be called in derived classes
    // For example base.Start()
    // This ensures behaviour common to all derived classes are implemented
    protected virtual void Start()
    {
        if (pMaxHealth <= 0)
            pMaxHealth = 1;

        mHealth = new Pool(pMaxHealth);
        mShield = new Pool(10, 0);
        mPressure = new Pool(30);
        mInventory = new Inventory(4);
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
        if (mShield > damage)
        {
            mShield -= damage;
            return;
        }

        else
        {
            mShield -= damage;
            damage -= mShield;
        }

        mHealth -= damage;

        if (mHealth <= 0 && !mDead)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        mDead = true;

        // If there are any on death triggers, call them
        if (mOnDeathHandler != null)
            mOnDeathHandler.Invoke();

        //If it dies remove it from the Sector
        if (SectorSpawn == null)
        {
            //Debug.Log("Set the Sector of the spawn");
        }
        else
        {
            if (pIsFloor == false) {
                SectorSpawn.RemoveObjectAt(TileLocationInSector);
            }
            else if (pIsFloor == true)
            {
                SectorSpawn.RemoveTileAt(TileLocationInSector);
            }
        }

        PrefabSpawner.GetPrefabSpawner().DespawnObject(gameObject);

    }

    // This method assumes the amount is positive
    public void UpdateHealth(int amount)
    {
        mHealth += amount;
    }

}
