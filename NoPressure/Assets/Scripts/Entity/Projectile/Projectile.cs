using UnityEngine;

public class Projectile : MonoBehaviour {

    public delegate void OnDeath(Projectile theProjectile);
    public OnDeath mOnDeath;

    public LayerMask collisionLayer;
    // This allows us to create projectiles that don't move forward.
    float projectileSpeed = 10f;
    float lifeTime = 3f;
    float age = 0f;
    float skinWidth = .1f;
    float explosionSize = 0f;
    float explosionDamage = 2f;

    public Explosion explosion;

    // Delegate to handle the effect of a projectile when it detects a collision
    OnCollision mOnCollisionHandler;

    void Start () {
        //Destroy(gameObject, lifeTime);
        
        // Compute collision detection for the first object the Projectile
        // collides with.

    }
	
    public void Init()
    {
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

        age += Time.deltaTime;
        if (age > lifeTime)
            die();
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
        age = 0f; 
        lifeTime = newTime;
    }

    public void SetEffect(OnCollision lEffect)
    {
        mOnCollisionHandler = lEffect;
    }


    public void SetCollisionLayer(LayerMask lCollisionLayer)
    {
        collisionLayer = lCollisionLayer;
    }

    public void setExplosionRange(float newRange)
    {
        explosionSize = newRange;
    }

    public void setExplosionDamage(float damage)
    {
        explosionDamage = damage;
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
            Debug.Log("collision effect is null " + collider + ":" + collider.transform.position);
            return;
        }

        if (mOnCollisionHandler(collider.transform, hitPoint, transform.forward) == true)
        {
            die();
            //else
            //Destroy(this);
        }
    }

    Material explosionMat;
    Color explosionShine;
    public void ChangeExplosionMat(Material lexplosionMat, Color lShine)
    {
        explosionMat = lexplosionMat;
        explosionShine = lShine;
    }

    void die()
    { 
        if(explosion != null)
        {
            if (explosionSize > 0)
            {
                Explosion explosionInst = Instantiate(explosion, transform.position, transform.rotation);

                ChangeMaterial(explosionInst.gameObject, explosionMat);

                explosionInst.GetComponentInChildren<Light>().color = explosionShine;
                explosionInst.GetComponentInChildren<Light>().range = explosionSize * 2.5f;

                explosionInst.SetSize(explosionSize);
                explosionInst.SetDamage(explosionDamage);
                
                Destroy(explosionInst.gameObject, 0.4f);
            }
        }

        if (mOnDeath != null)
            mOnDeath(this);

    }
    public void ChangeMaterial(GameObject toChange, Material newMaterial)
    {
        Material[] mats;
        mats = toChange.GetComponent<Renderer>().materials;
        mats[0] = newMaterial;
        toChange.GetComponent<Renderer>().materials = mats;
    }
}
