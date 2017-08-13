using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTile : PersistentEntity {


    public void ChangeMateral(Material newMateral)
    {
        //TODO
    }

    public void RemoveWire()
    {

    }

    public void AddWireToTile()
    {

    }

    public void PowerOn()
    {
        // If this tile already has power, we stop propagating the current


        // If this tile was off and is turning on, propagate power to all neighbors

    }

    // Use this for initialization
    protected override void Start () {
        base.Start();
	}
	
}
