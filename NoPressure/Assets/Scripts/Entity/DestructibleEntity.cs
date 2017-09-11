using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleEntity : SpawnableEntity {

    public event System.Action<Transform> mOnDeathHandler;

    // Vitals
    public float pStartingHealth = 1;
    public Vitals mVitals;
    private List<Traits> mTraits;

    protected override void Init()
    {
        mVitals = new Vitals(pStartingHealth);
    }

    protected override void CleanUp()
    {
        mOnDeathHandler = null;
    }

    protected virtual void Die()
    {
        Despawn();

        if (mOnDeathHandler != null)
            mOnDeathHandler(transform);
    }

    public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        TakeDamage(damage);
    }

    public virtual void TakeDamage(float damage)
    {
        mVitals = mVitals.TakeDamage(damage);

        if (mVitals.IsDead())
            Die();

    }

}
