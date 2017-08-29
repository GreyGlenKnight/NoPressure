using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpawnableEntity : MonoBehaviour {

    public System.Action mOnSpawn { private get; set; }
    public System.Action mOnDespawn { private get; set; }
    public bool IsSpawned { get; private set; }

    public void Spawn()
    {
        if (IsSpawned == false)
            IsSpawned = true;
        else
            Debug.Log(this + " has already been spawned");
        if (mOnSpawn != null)
            mOnSpawn();

        Init();
    }

    protected abstract void Init();

    public void Despawn()
    {
        if (IsSpawned == true)
            IsSpawned = false;
        else
            Debug.Log(this + " has not been spawned or has already been despawned");

        if (mOnDespawn != null)
            mOnDespawn();

        CleanUp();
    }

    protected abstract void CleanUp();
}
