using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTile : FloorTile
{
    //public EffectType pEffectTypePower;
    //public EffectType pEffectTypePowerAndBroken;

    public Material pBrokenPoweredMateral;
    public Material pBrokenNotPoweredMateral;
    public Material pPoweredMateral;
    public Material pNotPoweredMateral;

    public float pMagnitude = 1f;
    public float TimesinceLastTrigger = 0f;
    public float TriggerDelay = 1f;
    //public bool pIsPowered = false;
    //public bool pIsBroken = false;

    public bool pWillEmitLight = true;


    public int mDamage = 1;

    //public MapTile linkedTile;

    public float power = 0f;
    public float consumption = 20f;

    public float ChargeRequired = 1f;
    public float amountCharged = 0f;



    // Use this for initialization
    protected override void Start()
    {
        amountCharged += Random.Range(0f, 1f) * ChargeRequired;
        base.Start();
    }

    // Update is called once per frame
    //void Update()
    //{
    //    TimesinceLastTrigger += Time.deltaTime;



    //}

    public void OnTriggerExit(Collider other)
    {
        MovingEntity moving = other.GetComponent<MovingEntity>();
        if (moving != null)
        {
            moving.mOnUpdateGetTileInfo -= updatePressure;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        GeneratePressure generatePressure = null;
        generatePressure = other.GetComponent<GeneratePressure>();
        if (generatePressure != null)
        {
            //Debug.Log("Pressure Start " + TileData.m_Location);
            generatePressure.linkedTile = TileData;
            //PersistentEntity entity = other.GetComponent<PersistentEntity>();
            //entity.mOnDeathHandler += generatePressure.onDeathDelegate;
            TileData.powerGeneration += generatePressure.power;
            TileData.powerConsumption += generatePressure.consumption;
            return;
            //Debug.Log("enter");
            //TileData.PressureRate = 100;
            //Debug.Log("Adding Pressure");
        }

        PowerSource powerSource = null;
        powerSource = other.GetComponent<PowerSource>();

        if(powerSource != null)
        {
            TileData.powerGeneration += powerSource.power;
            TileData.powerConsumption += powerSource.consumption;
            powerSource.linkedTile = TileData;
            //PersistentEntity entity = other.GetComponent<PersistentEntity>();
            //entity.mOnDeathHandler += powerSource.onDeathDelegate;
        }


        MovingEntity moving = other.GetComponent<MovingEntity>();
        if (moving != null)
        {
            moving.mOnUpdateGetTileInfo += updatePressure;
        }

    }

    public void updatePressure(Transform entity)
    {
        MovingEntity movingEntity = entity.GetComponent<MovingEntity>();

        if(TileData.getPressure() > 0)
            movingEntity.mPressure += (TileData.getPressure() / 40f) * Time.deltaTime;

    }


    // Update is called once per frame
    void Update()
    {
        if (TileData == null)
            return;

        if (TileData.isBroken == true)
        {
            TileData.powerConsumption = consumption;

            amountCharged += Time.deltaTime * consumption * TileData.getPower();

            if (amountCharged > ChargeRequired)
            {
                FireWeapon();
                amountCharged -= ChargeRequired * Random.Range(0f,1f);
            }
        }

    }

    // Create a projectile and reduce ammo by 1
    private void FireWeapon()
    {
        LayerMask CollisionMask = LayerMask.GetMask(new string[] { "Entity", "Player", "Obstacle" });

        Vector3 spawnLocation = new Vector3(transform.position.x, 0.5f, transform.position.z);

        Quaternion rotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);

        rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        // Create bullet
        ProjectileManager.getProjectileManager().SpawnProjectile
            (Color.red, 5, 1f, CollisionMask, spawnLocation, rotation, onHitEffect);

        Quaternion rotation2 = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        // Create bullet
        ProjectileManager.getProjectileManager().SpawnProjectile
            (Color.red, 5, 1f, CollisionMask, spawnLocation, rotation2, onHitEffect);

    }

    // Function for delegate call, Action to take on the victim of the bullet 
    // collision
    private bool onHitEffect(Transform TargetHit, Vector3 hitPoint, Vector3 hitDirection)
    {
        IDamageable damageableObject = TargetHit.GetComponent<IDamageable>();

        if (damageableObject == null)
        {
            //Debug.Log("Colision with a non damagable target, refine the collision mask");
            return false;
        }

        // Cause damage to the target
        damageableObject.TakeHit(mDamage, hitPoint, transform.forward);
        return true;
    }
}


    //public void OnTriggerStay(Collider other)
    //{
    //    GeneratePressure generatePressure = null;
    //    generatePressure = other.GetComponent<GeneratePressure>();
    //    if (generatePressure != null)
    //    {
    //        TileData.PressureRate = 1000;
    //        //Debug.Log("Adding Pressure");
    //    }


    //    if (pIsPowered == false)
    //        return;

    //    Player player = other.GetComponent<Player>();

    //    if (TimesinceLastTrigger < TriggerDelay)
    //        return;

    //    if (player != null)
    //    {
    //        if (pIsBroken)
    //        {
    //            switch (pEffectTypePowerAndBroken)
    //            {
    //                case EffectType.Damage:

    //                    if (player.mHasDisableTrapSkill == false)
    //                    {
    //                        player.TakeDamage(pMagnitude);
    //                        TimesinceLastTrigger = 0f;
    //                    }
    //                    //else
    //                    //{
    //                    //    GetComponent<Renderer>().material = disabledMateral;
    //                    //    pEffectType = EffectType.None;
    //                    //}
    //                    break;

    //                case EffectType.GivePressure:
    //                    player.mPressure += pMagnitude;
    //                    TimesinceLastTrigger = 0f;
    //                    break;
    //            }
    //        }

    //        else
    //        {
    //            switch (pEffectTypePower)
    //            {
    //                case EffectType.Damage:

    //                    if (player.mHasDisableTrapSkill == false)
    //                    {
    //                        player.TakeDamage(pMagnitude);
    //                        TimesinceLastTrigger = 0f;
    //                    }
    //                    //else
    //                    //{
    //                    //    GetComponent<Renderer>().material = disabledMateral;
    //                    //    pEffectType = EffectType.None;
    //                    //}
    //                    break;

    //                case EffectType.GivePressure:
    //                    player.mPressure += pMagnitude;
    //                    TimesinceLastTrigger = 0f;
    //                    break;
    //            }
    //        }
    //    }

    //}

