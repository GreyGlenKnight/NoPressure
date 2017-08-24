using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEntity : PersistentEntity
{
    //TODO add Logic to use pathfinding, currently in enemy

    // Event to be fired off when PersistentEntity is despawned
    public event System.Action<Transform> mOnUpdateGetTileInfo;

    // Use this for initialization
    protected override void Start () {
        base.Start();
	}

    // Update is called once per frame
    protected virtual void Update()
    { 
        if (pIsElectronic == false)
        {
            mPressure -= Time.deltaTime * mPressureLeakRate;
        }

        // Because Inventoryitems are not not mono objects and have no update, we need
        // to call a custom update on them to keep track of cooldown timers like
        // fire rate and reload speed.
        if (mInventory != null)
            mInventory.updateTime();

        if (mPressure != null)
            if (mPressure.IsEmpty())
                Die();
        if (mHealth != null)
            if (mHealth.IsEmpty())
                Die();

        if (mOnUpdateGetTileInfo != null)
            mOnUpdateGetTileInfo(transform); 
    }
}
