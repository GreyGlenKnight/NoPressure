using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Pathfinding;

public class Enemy : MovingEntity, IItemCase {

    public float rangeOfSight; // how far the Enemy can see
    public float rangeOfAttack; // how far the Enemy can attack
    public LayerMask viewMask; // layer on which obstacles to sight exist
    public Transform attackSpawn;
    public Projectile attackType;

    public ParticleSystem deathEffect;
    // List of items that may be spawned when the Enemy dies
    public List<Pack> deathItems;
    public Pack xpPack;
    Animator m_Animator;

    public float msBetweenAttacks;
    float nextAttackTime;
    float attackSpeed = 0f;
    float attackLifeTime = .2f;

    enum State { Idle, Chasing, Attacking };
    State currentState;

    //private NavMeshAgent navMeshAgent;
    //private FloorBoss bossBehaviour;
    Transform target;
    PersistentEntity targetEntity;
    bool hasTarget;

    float collisionRadius;
    float targetCollisionRadius;

    Color flashColor = Color.white;
    float flashDuration = .15f;
    Material skinMaterial;
    Color originalColor;

    int itemCount = 1;
    int exp = 2;

    System.Random prng = new System.Random();

    // The point to move to
    public Transform targetPosition;
    private Seeker seeker;
    //private CharacterController controller;
    private Rigidbody controller;
    // The calculated path
    public Path path;
    // The AI's speed in meters per second
    public float speed = 2;
    // The max distance from the AI to a waypoint for it to continue to the next waypoint
    public float nextWaypointDistance = 0.001f;
    // The waypoint we are currently moving towards
    private int currentWaypoint = 0;
    // How often to recalculate the path (in seconds)
    public float repathRate = 2f;
    private float lastRepath = -9999;

    Vector3 mVelocity;

    public float determination = 5f;
    private float currentChaseTime = 0f;

    public Material ShotMateral;
    public Material explosionMateral;
    public Color ShotShine;


    private void Awake()
    {
        seeker = GetComponent<Seeker>();
        //controller = GetComponent<CharacterController>();
        controller = GetComponent<Rigidbody>();

        //navMeshAgent = GetComponent<NavMeshAgent>();
        skinMaterial = GetComponent<Renderer>().material;
        //bossBehaviour = GetComponent<FloorBoss>();

        //if (GameObject.FindGameObjectWithTag("Player") != null)
        //{
        //    hasTarget = true;

        //    // Get a reference to the Player
            target = GameObject.FindGameObjectWithTag("Player").transform;
        //    targetEntity = target.GetComponent<PersistentEntity>();

        //    // The Player and Enemy should collide on their surfaces and not their centres.
        //    collisionRadius = GetComponent<CapsuleCollider>().radius;
        //    targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
        //}
    }

    protected override void Start()
    {
        base.Start();

        m_Animator = GetComponent<Animator>();
        m_Animator.SetBool("isRunning", false);

        //if (hasTarget)
        //{
        currentState = State.Idle;


            // Listen for the Player's death event
            //targetEntity.mOnDeathHandler += OnTargetDeath;

            // Cast a ray from the Enemy to the Player
            //StartCoroutine(UpdatePath());
        //}
    }

    public void SetUpEnemy(float addHealth, float damage, Color enemyColor, int numItems, int baseXP)
    {
        
        if (mHealth == null)
            if (addHealth > 0)
                mHealth = new Pool(addHealth);
            else
                mHealth = new Pool(1);
        else
            mHealth += addHealth;
        //TODO Set ability to spawn projectiles

        //attackType.SetDamage(damage);
        itemCount = numItems;
        exp = baseXP;

        skinMaterial.color = enemyColor;
        originalColor = enemyColor;
    }

    protected override void Update()
    {
        base.Update();

        if (CanAttackPlayer())
        {
            Attack();
        }

        if (CanSeePlayer())
        {
            currentChaseTime = 0;

            transform.LookAt(target.transform.position);

            if (currentState != State.Chasing)
            {
                targetPosition = target;

                currentState = State.Chasing;
                m_Animator.SetBool("isRunning", true);
            }
        }
        else
        {
            if (currentState == State.Chasing)
            {
                currentChaseTime += Time.deltaTime;
                if(currentChaseTime > determination)
                {
                    currentState = State.Idle;
                    m_Animator.SetBool("isRunning", false);
                }
            }
        }

        if(currentState == State.Chasing)
            Goto();
    }

    private void FixedUpdate()
    {
        if (controller != null)
        {
            controller.MovePosition(controller.position + mVelocity * Time.deltaTime);
            if(controller.position.y > 0.9)
                controller.MovePosition(controller.position + new Vector3(0,-1*(controller.position.y - 0.9f) ,0) * Time.deltaTime);

            if(controller.position.y < 0.3)
            {
                controller.MovePosition(controller.position + new Vector3(0, -1*(0.3f - controller.position.y) , 0) * Time.deltaTime);

            }
        }
    }

    public void Move(Vector3 _velocity)
    {
        //mVelocity.y = 0;
        mVelocity = _velocity * speed;
    }

    private void Goto()
    {
        if (seeker == null)
        {
            return;
        }
        if(targetPosition == null)
        {
            return;
        }

        if (Time.time - lastRepath > repathRate && seeker.IsDone())
        {
            lastRepath = Time.time + Random.value * repathRate * 0.5f;
            // Start a new path to the targetPosition, call the the OnPathComplete function
            // when the path has been calculated (which may take a few frames depending on the complexity)
            seeker.StartPath(transform.position, targetPosition.position, OnPathComplete);
        }
        if (path == null)
        {
            // We have no path to follow yet, so don't do anything
            return;
        }
        //Debug.Log("Following path");

        if (currentWaypoint > path.vectorPath.Count) return;
        if (currentWaypoint == path.vectorPath.Count)
        {
            //Debug.Log("End Of Path Reached");
            currentWaypoint++;
            return;
        }
        // Direction to the next waypoint
        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        //dir *= speed;

        //controller.detectCollisions = false;
        // Note that SimpleMove takes a velocity in meters/second, so we should not multiply by Time.deltaTime
        Move(dir);
        //controller.MovePosition(dir);

        // The commented line is equivalent to the one below, but the one that is used
        // is slightly faster since it does not have to calculate a square root
        //if (Vector3.Distance (transform.position,path.vectorPath[currentWaypoint]) < nextWaypointDistance) {
        if ((transform.position - path.vectorPath[currentWaypoint]).sqrMagnitude < nextWaypointDistance * nextWaypointDistance)
        {
            //Debug.Log("Next Waypoint");
            currentWaypoint++; 
            return;
        }
    }

    // check if the Enemy can see the Player
    bool CanSeePlayer()
    {
        // 1. Check the distance between the Enemy and the Player
        if (Vector3.Distance(transform.position, target.position) < rangeOfSight)
        {
            // 2. Cast a ray from the Enemy to the Player. If it doesn't hit anything, the Enemy can see the Player
            if (!Physics.Linecast(transform.position, target.position, viewMask))
            {
                return true;
            }
        }

        return false;
    }

    // check if the Enemy can attack the Player
    bool CanAttackPlayer()
    {
        // 1. Check the distance between the Enemy and the Player
        if (Vector3.Distance(transform.position, target.position) < rangeOfAttack)
        {
            // 2. Cast a ray from the Enemy to the Player. If it doesn't hit anything, the Enemy can see the Player
            if (!Physics.Linecast(transform.position, target.position, viewMask))
            {
                return true;
            }
        }
        return false;
    }

    public void OnPathComplete(Path p)
    {
        //Debug.Log("A path was calculated. Did it fail with an error? " + p.error);
        if (!p.error)
        {
            path = p;
            // Reset the waypoint counter so that we start to move towards the first point in the path
            currentWaypoint = 0;
        }
        else
        {
            if (currentChaseTime > determination)
            {
                currentState = State.Idle;
                m_Animator.SetBool("isRunning", false);
            }
        }
    }

    // Calculate a new path to the target four times a second.
    // This is more performant than calculating the path every frame
    // (if this was in the update method).
    //IEnumerator UpdatePath()
    //{
    //    float refreshRate = .25f;

    //    while(hasTarget)
    //    {
    //        if(CanSeePlayer())
    //        {
    //            // When a LivingEntity dies, its transform remains but its
    //            // navMeshAgent does not.
    //            if(!mDead)
    //            {
    //                Debug.Log("Found Player!");
    //                //navMeshAgent.enabled = true;


    //                //Vector3 dirToTarget = (target.position - transform.position).normalized;
    //                //Vector3 targetPos = target.position - dirToTarget * (collisionRadius + targetCollisionRadius);

    //                //Path path = seeker.StartPath(transform.position, target.position);

    //                if (currentState != State.Chasing)
    //                {
    //                    targetPosition = target;

    //                    currentState = State.Chasing;
    //                }
    //                //TODO A* -> FindPath
    //                //navMeshAgent.SetDestination(targetPos);
    //            }  
    //        } else
    //        {
    //            if (!mDead)
    //            {
    //                //navMeshAgent.enabled = false; // this stops the Enemy from chasing the Player
    //            }
    //        }
    //        yield return new WaitForSeconds(refreshRate);
    //    }
    //}

    void Attack()
    {
        if (Time.time > nextAttackTime)
        {
            // The Enemy might not have been chasing the Player before attacking
            // so we store the previous state in order to return to it when
            // the Enemy is done attacking.
            State prevState = currentState;
            currentState = State.Attacking;
            // The Enemy should not be chasing the Player while attacking.
            //navMeshAgent.enabled = false;

            nextAttackTime = Time.time + msBetweenAttacks / 1000;

            LayerMask layerMask = LayerMask.GetMask("Player");

            Vector3 spawnLocation = new Vector3(transform.position.x, 0.6f, transform.position.z);

            Vector3 baseRot = transform.rotation.eulerAngles;
            Quaternion rotation = Quaternion.Euler(baseRot.x, baseRot.y -10 + Random.Range(0f, 20f), baseRot.z);

            ProjectileManager.getProjectileManager().SpawnProjectile(ShotMateral, explosionMateral, ShotShine, 6, 2, layerMask,spawnLocation ,rotation, DamagePlayer);
            //Projectile newProjectile = Instantiate(attackType, attackSpawn.position, attackSpawn.rotation);
            //newProjectile.SetLifeTime(attackLifeTime);
            //newProjectile.SetSpeed(attackSpeed);

            currentState = prevState;
            //navMeshAgent.enabled = true;
        }
    }

    public bool DamagePlayer(Transform TargetHit, Vector3 hitPoint, Vector3 hitDirection)
    {

        IDamageable damageableObject = (IDamageable) TargetHit.GetComponent<Player>();
        //IDamageable damageableObject = TargetHit.GetComponent<IDamageable>();

        if (damageableObject == null)
        {
           // Debug.Log("Colision with a non damagable target, refine the collision mask");
            return false;
        }
        // Cause damage to the target
        damageableObject.TakeHit(4, hitPoint, transform.forward);
        return true;
    }

    IEnumerator DamageFlash()
    {
        float duration = 0f;
        skinMaterial.color = flashColor;

        while(duration < flashDuration)
        {
            duration += Time.deltaTime;
            yield return null;
        }

        skinMaterial.color = originalColor;
    }

    public override void TakeDamage(float damage)
    {
        // Enemy should flash when it takes damage
        StartCoroutine(DamageFlash());

        base.TakeDamage(damage);
    }

    void OnTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }

    public void Explode(ParticleSystem deathEffect, Vector3 hitPoint)
    { 
        //Destroy(Instantiate(deathEffect.gameObject, hitPoint, transform.rotation));
    }

    public void SpawnItems(List<Pack> items)
    {
        // always spawn an XP pack
        //Instantiate(xpPack, transform.position + new Vector3(.2f, 0, 0), transform.rotation);
        //xpPack.itemValue = exp;

        if (deathItems.Count != 0)
            for (int i = 0; i < itemCount; i++)
            {

                // pick a random item from the list of items and spawn it
                Pack itemToSpawn = deathItems[prng.Next(items.Count)];
                Instantiate(itemToSpawn, transform.position, transform.rotation);
            }
    
    }

    public override void Die()
    {
        Explode(deathEffect, transform.position);
        SpawnItems(deathItems);
        //bossBehaviour.SpawnExit(transform.position);
        base.Die();
    }
    private void OnDisable()
    {
        //seeker.pathCallback -= OnPathComplete; 
    }

}
