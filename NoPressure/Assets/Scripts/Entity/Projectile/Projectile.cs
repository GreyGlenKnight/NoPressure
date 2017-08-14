using UnityEngine;

public class Projectile : MonoBehaviour {

    public LayerMask collisionLayer;
    // This allows us to create projectiles that don't move forward.
    float projectileSpeed = 10f;
    float lifeTime = 3f;

    float skinWidth = .1f;

    // Delegate to handle the effect of a projectile when it detects a collision
    ProjectileManager.OnCollision mOnCollisionHandler;

    void Start () {
        Destroy(gameObject, lifeTime);

        // Compute collision detection for the first object the Projectile
        // collides with.
        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, .1f, collisionLayer);
        if (initialCollisions.Length > 0)
        {
            OnHitObject(initialCollisions[0], transform.position);
        }
    }
	
	// Update is called once per frame
	void Update () {
        float moveDistance = projectileSpeed * Time.deltaTime;
        CheckCollisions(moveDistance);

        // Propel the projectile
        transform.Translate(Vector3.forward * moveDistance);
	}

    public void SetSpeed(float newSpeed)
    {
        projectileSpeed = newSpeed;
    }

    // This allows us to define projectiles that stay on the field for shorter
    // or longer periods.
    public void SetLifeTime(float newTime)
    {
        lifeTime = newTime;
    }

    public void SetEffect(ProjectileManager.OnCollision lEffect)
    {
        mOnCollisionHandler = lEffect;
    }

    public void SetCollisionLayer(LayerMask lCollisionLayer)
    {
        collisionLayer = lCollisionLayer;
    }

    // Collision detection using colliders
    void CheckCollisions(float dist)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Cast a ray from the projectile at a specified distance + the thickness of the
        // potential targets skin (this creates a hitBox for the target).
        // If the ray hits something on the specified layer, handle the collision.
        if (Physics.Raycast(ray, out hit, dist + skinWidth, collisionLayer, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit.collider, hit.point);
        }
    }

    // Handle a successful collision
    void OnHitObject(Collider collider, Vector3 hitPoint)
    {
        Debug.Log("Collision with: " + collider);
        if (collider == null)
        {
            Debug.Log("Target of collision is null");
            return;
        }
        if (transform == null)
        {
            Debug.Log("object causing collision is null");
            return;
        }

        if (mOnCollisionHandler == null)
        {
            Debug.Log("collision effect is null");
            return;
        }

        Debug.Log("Collision with: " + collider);

        if (mOnCollisionHandler(collider.transform, hitPoint, transform.forward) == true)
        {
            Destroy(this);
        }
    }
}
