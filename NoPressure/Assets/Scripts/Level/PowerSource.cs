using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerSource : MonoBehaviour, IClickable,IDamageable {
    public MapTile linkedTile;
    public float power;
    public float consumption;
    
    public Material OnMateral;
    public Material OffMateral;
    public Material BrokenMateral;

    public bool isOn;
    public bool isDamaged;

    public float totalDamage = 0;

    protected virtual void Update()
    {
        if (linkedTile != null)
        {
            if (isOn == true)
                if (isDamaged == false)
                    linkedTile.powerGeneration = power;
                else
                    linkedTile.powerGeneration = 0;
            else
                linkedTile.powerGeneration = 0;
        }
        if (isDamaged == true)
        {
            ChangeMateral(BrokenMateral);
            return;
        }

        if(isOn == true)
        {
            ChangeMateral(OnMateral);
            return;
        }

        else
            ChangeMateral(OffMateral);

    }

    public void onDeathDelegate(Transform dieingUnit)
    {
        Debug.Log("Stop the Power");
        linkedTile.powerConsumption = 0;
        linkedTile.powerGeneration = 0;
    }

    public void ChangeMateral(Material newMateral)
    {
        Material[] mats;
        mats = GetComponent<Renderer>().materials;
        mats[0] = newMateral;
        GetComponent<Renderer>().materials = mats;
    }

    public void Repair()
    {
        totalDamage = 0;
        isDamaged = false;
    }

    public void OnClick()
    {

        if (isOn == true)
            isOn = false;
        else
        {
            isOn = true;
        }
    }

    public void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        totalDamage += damage;
        if (totalDamage > 10)
        {
            isDamaged = true;
        }
    }
}
