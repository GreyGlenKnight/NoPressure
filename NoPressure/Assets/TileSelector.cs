using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSelector : MonoBehaviour {

    //public IClickable currentObject;
    public PowerSource currentObject;
    public DestructibleObstacle currentlObstacle;
    public Crate currentCrate;

    public Transform player;
    public float reach = 3f;

    public Material SelectMaterial;
    public Material DetectedObjectMaterial;
    public Material OutOfRangeMaterial;
    public Material PlaceItemMaterial;

    public SelectMode selectMode = SelectMode.PlaceItem;

    public enum SelectMode
    {
        PlaceItem,
        Search,
    }

    public void ChangeMateral(Material newMateral)
    {
        Material[] mats;
        mats = GetComponent<Renderer>().materials;
        mats[0] = newMateral;
        GetComponent<Renderer>().materials = mats;
    }

    public void SetSearchMode(SelectMode newSearchMode)
    {
        selectMode = newSearchMode;
    }

    public bool IsOutOfRange()
    {
        if (Vector3.Distance(transform.position, player.position) > reach)
            return true;
        else
            return false;
    }

    public void Update()
    {

        if (player == null)
            return;

        if (IsOutOfRange())
            ChangeMateral(OutOfRangeMaterial);
        else
        {
            if(selectMode == SelectMode.PlaceItem)
            {
                if (currentObject != null)
                    ChangeMateral(DetectedObjectMaterial);
                else if (currentlObstacle != null)
                    ChangeMateral(DetectedObjectMaterial);
                else if (currentCrate != null)
                    ChangeMateral(DetectedObjectMaterial);
                else
                    ChangeMateral(PlaceItemMaterial);
            }
            else
            {
                ChangeMateral(SelectMaterial);
            }
        }

    }


    public void OnTriggerExit(Collider other)
    {
        PowerSource powerSource = null;
        powerSource = other.GetComponent<PowerSource>();

        if (powerSource != null)
        {
            currentObject = null;
        }

        DestructibleObstacle lObstacle = null;
        lObstacle = other.GetComponent<DestructibleObstacle>();

        if (lObstacle != null)
        {
            currentlObstacle = null;
        }

        Crate crate = null;
        crate = other.GetComponent<Crate>();

        if (crate != null)
        {
            currentCrate.SetButtonInactive();
            currentCrate = null;
            //currentCrate.SetButtonActive();
        }


    }

    public void OnTriggerEnter(Collider other)
    {

        PowerSource powerSource = null;
        powerSource = other.GetComponent<PowerSource>();

        if (powerSource != null)
        {
            currentObject = powerSource;
        }

        DestructibleObstacle lObstacle = null;
        lObstacle = other.GetComponent<DestructibleObstacle>();

        if (lObstacle != null)
        {
            currentlObstacle = lObstacle;
        }

        Crate crate = null;
        crate = other.GetComponent<Crate>();

        if (crate != null)
        {
            currentCrate = crate;
            currentCrate.SetButtonActive();
        }

    }

    public PowerSource getPowerObject()
    {
        return currentObject;
    }

    public Crate getItemCrate()
    {
        return currentCrate;
    }

    public DestructibleObstacle getObstacle()
    {
        return currentlObstacle;
    }

}
