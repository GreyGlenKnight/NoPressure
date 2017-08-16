using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEntity : PersistentEntity
{
    //TODO add Logic to use pathfinding, currently in enemy

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
        mInventory.updateTime();

        if (mPressure.IsEmpty())
            Die();
        if (mHealth.IsEmpty())
            Die();
    }
}
