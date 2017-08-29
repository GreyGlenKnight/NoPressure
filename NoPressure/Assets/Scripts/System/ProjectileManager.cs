using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate bool OnCollision(Transform TargetHit, Vector3 hitPoint, Vector3 hitDirection);

public class ProjectileManager : MonoBehaviour {

    private PrefabPooler prefabPooler;

    // Singleton for spawning projectiles
    private static ProjectileManager instance;

    public static ProjectileManager getProjectileManager()
    {
        return instance;
    }

    void Start () {
        prefabPooler = PrefabPooler.instance;
        instance = this;
    }
	
    public void SpawnProjectile(
    Material projectileMateral,
    Material explosionMateral,
    Color Shine,
        float ProjectileSpeed,
        float duration,//in Seconds
        LayerMask CollisionMask,
        Transform Spawner,//can not collide with the thing that spawned it
        OnCollision effect,//function to call on the target
        float explosionSize = 0f,
        float explosionDamage =2f
        )
    {
        prefabPooler.SpawnProjectile(
                projectileMateral,
                explosionMateral,
                Shine,
                ProjectileSpeed,
                duration,//in Seconds
                CollisionMask,
                Spawner,//can not collide with the thing that spawned it
                effect,
                explosionSize,
                explosionDamage);//function to call on the target
        //TODO Set color
    }

    public void SpawnProjectile(
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
        prefabPooler.SpawnProjectile(
                projectileMateral,
                explosionMateral,
                Shine,
                ProjectileSpeed,
                duration,//in Seconds
                CollisionMask,
                SpawnLocation,//can not collide with the thing that spawned it
                SpawnRotation,//can not collide with the thing that spawned it
                effect,
                explosionSize,
                explosionDamage);//function to call on the target
    }





}
