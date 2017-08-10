using UnityEngine;

// Everything that can Die() or be destroyed.
public class LivingEntity : MonoBehaviour, IDamageable {

    // Event to be fired off when LivingEntity dies
    public event System.Action mOnDeathHandler;

    // Public Set variables
    public int pMaxHealth = 1;

    // Mechanical units don’t need pressure and can be effected by EMP weapons
    public bool pIsMechanical = false;
    
    // Resources
    protected Inventory mInventory;

    // Vitals
    public Pool mPressure { get; set; }
    public Pool mShield { get; set; }
    public Pool mHealth { get; set; }
    public float mPressureLeakRate { get; set; } 

    // Skills
    public bool mHasFixPressureSkill {
        get
        {
            return true;
        }

    }
    public bool mHasDisableTrapSkill { get; set; }

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

    protected virtual void Update()
    {
        if (GameManager.instance.loading)
            return;

        if (pIsMechanical == false)
        {
            mPressure -= Time.deltaTime * mPressureLeakRate;
        }

        mInventory.updateTime();

        if (mPressure.IsEmpty())
            Die();
        if (mHealth.IsEmpty())
            Die();

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

        if (mHealth < 0 && !mDead)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        mDead = true;
        mOnDeathHandler.Invoke();
        gameObject.SetActive(false);
        //Destroy(gameObject);
    }

    // This method assumes the amount is positive
    public void UpdateHealth(int amount)
    {
        mHealth += amount;
    }

}
