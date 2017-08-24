using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ProjectileManager : MonoBehaviour {
    
    // Singleton for spawning projectiles
    private static ProjectileManager instance;

    // Use this for initialization
    public delegate bool OnCollision(Transform TargetHit, Vector3 hitPoint, Vector3 hitDirection);

    public Queue<Projectile> projectilesToSpawn = new Queue<Projectile>();  

    // Assign a generic projectile in the editor
    public Projectile ProjectilePrototype;

    public float warningDelay = 0f;

    public static ProjectileManager getProjectileManager()
    {
        return instance;
    }

    private void Awake()
    {
        for (int i = 0; i < 200; i++)
        {
            Projectile newProjectile = Instantiate(ProjectilePrototype);
            newProjectile.gameObject.SetActive(false);
            projectilesToSpawn.Enqueue(newProjectile);
        }
        //if (instance == null)
            instance = this;
        //else if (instance != this)
        //{
        //    Debug.Log("SelfDestroy");
        //    Destroy(gameObject);
        //}
        //Persist the GameManager instance across scenes
        //DontDestroyOnLoad(gameObject);
    }

    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        warningDelay -= Time.deltaTime;

    }

    public void SpawnProjectile(
        Color projectileColor,
        float ProjectileSpeed,
        float duration,//in Seconds
        LayerMask CollisionMask,
        Transform Spawner,//can not collide with the thing that spawned it
        OnCollision effect,//function to call on the target
        float explosionSize = 0f,
        float explosionDamage =2f
        )
    {
        if (ProjectilePrototype == null)
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
             return;
        }
        Projectile newProjectile = projectilesToSpawn.Dequeue(); //stantiate(ProjectilePrototype, Spawner.position, Spawner.rotation);

        if (newProjectile == null)
            Debug.LogError("failed Instantiating projectile");

        newProjectile.Init();
        newProjectile.transform.position = Spawner.position;
        newProjectile.transform.rotation = Spawner.rotation;

        newProjectile.setExplosionRange(explosionSize);
        newProjectile.setExplosionDamage(explosionDamage);

        newProjectile.SetSpeed(ProjectileSpeed);
        newProjectile.SetEffect(effect);
        newProjectile.SetLifeTime(duration);
        newProjectile.SetCollisionLayer(CollisionMask);
        newProjectile.mOnDeath += ProjectileOnDeath;
        newProjectile.gameObject.SetActive(true);
        //TODO Set color
    }

    public void SpawnProjectile(
    Color projectileColor,
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

        if (ProjectilePrototype == null)
            Debug.LogError("Assign projectile in editor ");

        if (projectilesToSpawn.Count == 0)
        {
            if (warningDelay < 0)
            {
                Debug.Log("max projectiles");
                warningDelay = 1f;
            }
            return;
        }

        Projectile newProjectile = projectilesToSpawn.Dequeue();

        if (newProjectile == null)
            Debug.LogError("failed Instantiating projectile");

        newProjectile.Init();
        newProjectile.transform.position = SpawnLocation;
        newProjectile.transform.rotation = SpawnRotation;

        newProjectile.setExplosionRange(explosionSize);
        newProjectile.setExplosionDamage(explosionDamage);

        newProjectile.SetSpeed(ProjectileSpeed);
        newProjectile.SetEffect(effect);
        newProjectile.SetLifeTime(duration);
        newProjectile.SetCollisionLayer(CollisionMask);
        newProjectile.mOnDeath += ProjectileOnDeath;
        newProjectile.gameObject.SetActive(true);
        //TODO Set color
    }



    public void ProjectileOnDeath(Projectile Recycle)
    {
        Recycle.mOnDeath -= ProjectileOnDeath;
        Recycle.gameObject.SetActive(false);
        projectilesToSpawn.Enqueue(Recycle);

    }

}
