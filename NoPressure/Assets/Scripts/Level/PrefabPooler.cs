using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabPooler : MonoBehaviour {

    public float warningDelay = 0f;

    public static PrefabPooler instance;
    public static PrefabPooler getProjectileManager()
    {
        return instance;
    }

    private void CreatePools()
    {
        Debug.Log("Creating Pools");
        initProjectiles();
        initWalls();
        initFloor();
        initDrones();
        initInvaders();
    }

    private void Awake()
    {
        instance = this;

    }

    public void Init()
    {
        CreatePools();
    }

    void Update()
    {
        warningDelay -= Time.deltaTime;
    }

    /// <summary>
    /// ResourcePools
    /// </summary>
    public Pool PoolPrefab;
    public ResourcePool ResourcePoolPrefab;
    //Queue<ResourcePool> PoolsToSpawn = new Queue<ResourcePool>();
    //public int NumberOfResourcePools = 4000;

    //public ResourcePool SpawnResourcePool()
    //{
    //    if (PoolsToSpawn.Count == 0)
    //    {
    //        if (warningDelay < 0)
    //        {
    //            Debug.Log("Out of pools");
    //            warningDelay = 1f;
    //        }
    //        return null;
    //    }

    //    return PoolsToSpawn.Dequeue();
    //}

    //public void initPools()
    //{
    //    for(int i = 0; i< NumberOfResourcePools; i++)
    //    {
    //        ResourcePool newPool = Instantiate(ResourcePoolPrefab);
    //        newPool.gameObject.SetActive(false);
    //        PoolsToSpawn.Enqueue(newPool);

    //    }
    //}

    /// <summary>
    /// Walls
    /// </summary>
    public DestructibleObstacle WallPrefab;
    Queue<DestructibleObstacle> WallPool = new Queue<DestructibleObstacle>();
    public int wallNum = 800;

    public Material WallMaterial;
    public int WallHealth = 50;
    public Material AmmoMaterial;
    public int AmmoWallHealth = 5;

    public Pack ammoPack;

    public void WallOnDeath(Transform Recycle)
    {
        Recycle.gameObject.SetActive(false);
        DestructibleObstacle wall = Recycle.GetComponent<DestructibleObstacle>();
        WallPool.Enqueue(wall);
    }

    public DestructibleObstacle SpawnDestructibleObstacle()
    {
        if (WallPool.Count == 0)
        {
            if (warningDelay <= 0)
            {
                Debug.Log("max walls");
                warningDelay = 1f;
            }
            return null;
        }

        DestructibleObstacle returnWall = WallPool.Dequeue();
        returnWall.gameObject.SetActive(true);

        return returnWall;
    }

    public DestructibleObstacle spawnWall(Coord location)
    {
        DestructibleObstacle wall = spawnWall();

        if (wall == null)
        {
            Debug.Log("Failed Creating Wall");
            return null;
        }

        wall.transform.position = new Vector3(location.x, 1f, location.y);

        return wall;
    }

    public DestructibleObstacle spawnWall()
    {
        DestructibleObstacle wall = SpawnDestructibleObstacle();

        if (wall == null)
        {
            Debug.Log("Failed Creating Wall");
            return null;
        }

        ChangeMaterial(wall.gameObject, WallMaterial);
        wall.mHealth = new Pool(WallHealth);

        return wall;
    }

    public DestructibleObstacle SpawnAmmoWall(Coord location)
    {
        DestructibleObstacle wall = SpawnAmmoWall();

        if (wall == null)
        {
            Debug.Log("Failed Creating Wall");
            return null;
        }

        wall.transform.position = new Vector3(location.x, 1f, location.y);

        return wall;
    }
     
    public DestructibleObstacle SpawnAmmoWall()
    {
        DestructibleObstacle wall = SpawnDestructibleObstacle();

        if (wall == null)
        {
            Debug.Log("Failed Creating Wall");
            return null;
        }

        ChangeMaterial(wall.gameObject, AmmoMaterial);

        wall.deathItems.Add(Instantiate(ammoPack));

        wall.mHealth = new Pool(WallHealth);

        return wall;
    }

    private void initWalls()
    {
        if (WallPrefab == null)
            Debug.LogError("Assign Wall prefab");

        // Create Walls
        for (int i = 0; i < wallNum; i++)
        {
            DestructibleObstacle newWall = Instantiate(WallPrefab);
            newWall.gameObject.SetActive(false);
            Pool HealthPool = new Pool(10);

            newWall.mHealth = HealthPool;
            //newWall.mArmor = ArmorPool;
            //newWall.mShield = ShieldPool;
            //newWall.mPressure = PressurePool;
            newWall.mOnDeathHandler += WallOnDeath;
            newWall.mOnDespawn += WallOnDeath;

            WallPool.Enqueue(newWall);
        }
    }

    /// <summary>
    /// Floors
    /// </summary>
    public EffectTile FloorTilePrefab;
    public Queue<EffectTile> FloorPool = new Queue<EffectTile>();
    public Queue<EffectTile> SpawnedFloors;
    public int NumFloors = 2000;

    public void FloorOnDeath(Transform Recycle)
    {
        //Recycle.gameObject.SetActive(false);
        EffectTile tile = Recycle.GetComponent<EffectTile>();
        FloorPool.Enqueue(tile);
    }

    private void initFloor()
    {
        if (FloorTilePrefab == null)
            Debug.LogError("Missing Floor Tile");

        // Create Floor
        for (int i = 0; i < NumFloors; i++)
        {
            EffectTile newTile = Instantiate(FloorTilePrefab);
            //newTile.mOnDeathHandler += FloorOnDeath;
            newTile.mOnDespawn += FloorOnDeath;
            //newTile.gameObject.SetActive(false);

            FloorPool.Enqueue(newTile);
        }
    }

    public EffectTile SpawnTile(MapTile tileData, Coord location)
    {
        EffectTile returnTile = SpawnTile();

        if (returnTile == null)
        {
            Debug.Log("Failed creating Tile");
            return null;
        }

        returnTile.TileData = tileData;
        returnTile.transform.position = new Vector3(location.x, 0f, location.y);

        returnTile.TileData.isWired = false;
        returnTile.TileData.isBroken = false;
        returnTile.TileData.isSpace = false;
        returnTile.updateMat();

        return returnTile;
    }

    private EffectTile SpawnTile()
    {
        if (FloorPool.Count == 0)
        {
            if (warningDelay < 0)
            {
                Debug.Log("max tiles");
                warningDelay = 1f;
            }
            Debug.Log("max tiles");
            return null;
        }

        EffectTile returnTile = FloorPool.Dequeue();
        returnTile.gameObject.SetActive(true);
        returnTile.isSpawned = true;


        return returnTile;
    }

    public EffectTile CreateWiredTile(MapTile tileData, Coord location)
    {
        EffectTile returnTile = SpawnTile(tileData, location);

        if (returnTile == null)
        {
            Debug.Log("Failed creating Tile");
            return null;
        }

        returnTile.TileData.isWired = true;
        returnTile.TileData.isBroken = false;
        returnTile.TileData.isSpace = false;
        returnTile.updateMat();
        return returnTile;
    }

    public EffectTile CreateDeadSpaceTile(MapTile tileData, Coord location)
    {
        EffectTile returnTile = SpawnTile(tileData, location);

        if (returnTile == null)
        {
            Debug.Log("Failed creating Tile");
            return null;
        }
         
        returnTile.TileData.isWired = false;
        returnTile.TileData.isBroken = true;
        returnTile.TileData.isSpace = true;
        returnTile.updateMat();

        return returnTile;
    }

    public EffectTile CreateBrokenAndWired(MapTile tileData, Coord location)
    {
        EffectTile returnTile = SpawnTile(tileData, location);

        if (returnTile == null)
        {
            Debug.Log("Failed creating Tile");
            return null;
        }

        returnTile.TileData.isWired = true;
        returnTile.TileData.isBroken = true;
        returnTile.TileData.isSpace = false;
        returnTile.updateMat();
        return returnTile;
    }

    public EffectTile CreateBrokenTile(MapTile tileData, Coord location)
    {
        EffectTile returnTile = SpawnTile(tileData, location);

        if (returnTile == null)
        {
            Debug.Log("Failed creating Tile");
            return null;
        }

        returnTile.TileData.isWired = false;
        returnTile.TileData.isBroken = true;
        returnTile.TileData.isSpace = false;
        returnTile.updateMat();
        return returnTile;
    }

    /// <summary>
    /// Drones
    /// </summary>
    public Transform DroneEnemyPrefab;
    private Queue<Transform> DroneQueue = new Queue<Transform>();
    public int NumDrones = 100;

    private void initDrones()
    {
        // Create Drones
        for (int i = 0; i < NumDrones; i++) 
        {
            Transform newDrone = Instantiate(DroneEnemyPrefab);
            newDrone.gameObject.SetActive(false);

            newDrone.GetComponent<Enemy>().mHealth = new Pool(10);// Instantiate(PoolPrefab);
            newDrone.GetComponent<Enemy>().mOnDeathHandler += DroneOnDeath;
            newDrone.GetComponent<Enemy>().mOnDespawn += DroneOnDeath;
            DroneQueue.Enqueue(newDrone);
        }
    }

    public void DroneOnDeath(Transform Recycle)
    {
        Recycle.gameObject.SetActive(false);
        DroneQueue.Enqueue(Recycle.transform);
    }

    public Enemy SpawnDrone()
    {
        if (DroneQueue.Count == 0)
        {
            if (warningDelay < 0)
            {
                Debug.Log("max drones");
                warningDelay = 1f;
            }
            return null;
        }

        Transform returnDrone = DroneQueue.Dequeue();
        returnDrone.gameObject.SetActive(true);

        return returnDrone.GetComponent<Enemy>() ;
    }


    public Enemy SpawnDrone(Coord location)
    {
        Enemy returnDrone = SpawnDrone();

        if(returnDrone == null)
        {
            Debug.Log("Can not spawn drone");
            return null; 
        }

        returnDrone.transform.position = new Vector3(location.x, 0.5f, location.y);

        return returnDrone;

    }

    /// <summary>
    /// Invader
    /// </summary>
    public Transform InvaderEnemyPrefab;
    private Queue<Transform> InvaderQueue = new Queue<Transform>();
    public int NumInvaders = 50;

    private void initInvaders()
    {
        // Create Invaders
        for (int i = 0; i < NumInvaders; i++)
        {
            Transform newInvader = Instantiate(InvaderEnemyPrefab);
            newInvader.gameObject.SetActive(false);
            InvaderQueue.Enqueue(newInvader);
        }
    }

    /// <summary>
    /// Projectiles
    /// </summary>
    public Projectile ProjectilePrefab;
    public Queue<Projectile> projectilesToSpawn = new Queue<Projectile>();
    public int NumberOfProjectiles = 500;

    // Create Projectile from unit
    public Projectile SpawnProjectile(
    Material projectileMateral,
    Material explosionMateral,
    Color Shine,
        float ProjectileSpeed,
        float duration,//in Seconds
        LayerMask CollisionMask,
        Transform Spawner,//can not collide with the thing that spawned it
        OnCollision effect,//function to call on the target
        float explosionSize = 0f,
        float explosionDamage = 2f
        )
    {
        if (ProjectilePrefab == null)
            Debug.LogError("Assign projectile in editor ");

        if (Spawner == null)
            Debug.LogError("Spawner is null");

        if (projectilesToSpawn.Count == 0)
        {
            if (warningDelay < 0)
            {
                Debug.Log("max projectiles");
                warningDelay = 1f;
            }
            return null;
        }
        Projectile newProjectile = projectilesToSpawn.Dequeue(); //stantiate(ProjectilePrototype, Spawner.position, Spawner.rotation);

        if (newProjectile == null)
            Debug.LogError("failed Instantiating projectile");

        newProjectile.Init();
        newProjectile.transform.position =
            new Vector3(Spawner.position.x, 0.6f, Spawner.position.z);

        newProjectile.transform.rotation = Spawner.rotation;

        newProjectile.setExplosionRange(explosionSize);
        newProjectile.setExplosionDamage(explosionDamage);

        newProjectile.SetSpeed(ProjectileSpeed);
        this.ChangeMaterial(newProjectile.gameObject, projectileMateral);
        newProjectile.GetComponentInChildren<Light>().color = Shine;
        newProjectile.SetEffect(effect);
        newProjectile.SetLifeTime(duration);
        newProjectile.SetCollisionLayer(CollisionMask);
        newProjectile.mOnDeath += ProjectileOnDeath;
        newProjectile.gameObject.SetActive(true);

        return newProjectile;
        //TODO Set color
    }

    // Create Projectile at location
    public Projectile SpawnProjectile(
    Material projectileMateral,
    Material explosionMateral,
    Color Shine,
    float ProjectileSpeed,
    float duration,//in Seconds
    LayerMask CollisionMask,
    Vector3 SpawnLocation,//can not collide with the thing that spawned it
    Quaternion SpawnRotation,//can not collide with the thing that spawned it
    OnCollision effect,
    float explosionSize = 0f,
    float explosionDamage = 2f//function to call on the target
    )
    {
        if (ProjectilePrefab == null)
            Debug.LogError("Assign projectile in editor ");

        if (projectilesToSpawn.Count == 0)
        {
            if (warningDelay < 0) 
            {
                Debug.Log("max projectiles");
                warningDelay = 1f;
            }
            return null;
        }

        Projectile newProjectile = projectilesToSpawn.Dequeue();

        if (newProjectile == null)
            Debug.LogError("failed Instantiating projectile");

        newProjectile.Init();
        newProjectile.transform.position = SpawnLocation;
        newProjectile.transform.rotation = SpawnRotation;

        newProjectile.setExplosionRange(explosionSize);
        newProjectile.setExplosionDamage(explosionDamage);

        this.ChangeMaterial(newProjectile.gameObject, projectileMateral);
        newProjectile.GetComponentInChildren<Light>().color = Shine;

        newProjectile.SetSpeed(ProjectileSpeed);
        newProjectile.SetEffect(effect);
        newProjectile.SetLifeTime(duration);
        newProjectile.SetCollisionLayer(CollisionMask);
        newProjectile.mOnDeath += ProjectileOnDeath;
        newProjectile.gameObject.SetActive(true);


        return newProjectile;
        //TODO Set color
    }

    public void ProjectileOnDeath(Projectile Recycle)
    {
        Recycle.mOnDeath -= ProjectileOnDeath;
        Recycle.gameObject.SetActive(false);
        projectilesToSpawn.Enqueue(Recycle);
    }

    private void initProjectiles()
    {
        // Create Projectiles
        for (int i = 0; i < NumberOfProjectiles; i++)
        {
            Projectile newProjectile = Instantiate(ProjectilePrefab);
            newProjectile.gameObject.SetActive(false);
            projectilesToSpawn.Enqueue(newProjectile);
        }
    }

    public void ChangeMaterial(GameObject toChange, Material newMaterial)
    {
        Material[] mats;
        mats = toChange.GetComponent<Renderer>().materials;
        mats[0] = newMaterial;
        toChange.GetComponent<Renderer>().materials = mats; 
    }

}
