using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ProjectileManager : MonoBehaviour {
    
    // Singleton for spawning projectiles
    private static ProjectileManager instance;

    // Use this for initialization
    public delegate bool OnCollision(Transform TargetHit, Vector3 hitPoint, Vector3 hitDirection);

    // Assign a generic projectile in the editor
    public Projectile ProjectilePrototype;

    public static ProjectileManager getProjectileManager()
    {
        return instance;
    }

    private void Awake()
    {
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
		
	}

    public void SpawnProjectile(
        Color projectileColor,
        float ProjectileSpeed,
        float duration,//in Seconds
        LayerMask CollisionMask,
        Transform Spawner,//can not collide with the thing that spawned it
        OnCollision effect//function to call on the target
        )
    {
        if (ProjectilePrototype == null)
            Debug.LogError("Assign projectile in editor ");

        if (Spawner == null)
            Debug.LogError("Spawner is null");

        Projectile newProjectile = Instantiate(ProjectilePrototype, Spawner.position, Spawner.rotation);

        if (newProjectile == null)
            Debug.LogError("failed Instantiating projectile");

        newProjectile.SetSpeed(ProjectileSpeed);
        newProjectile.SetEffect(effect);
        newProjectile.SetLifeTime(duration);
        newProjectile.SetCollisionLayer(CollisionMask);
        //TODO Set color
    }

}
