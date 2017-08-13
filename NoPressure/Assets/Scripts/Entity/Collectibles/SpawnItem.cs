using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItem : IInventoryItem {

    // The display icon of the item
    public Sprite mDisplaySprite { get; protected set; }

    // The MonoBehaviour object that is currenly using this item
    public Transform mEquipedBy { get; protected set; }

    // The amount of uses the item has
    public ResourcePool mCharges { get; protected set; }

    SpawnType itemToSpawn;

    public IInventoryItem Clone()
    {
        return new SpawnItem(mDisplaySprite, itemToSpawn);
    }

    public void UpdateTime()
    {
        
    }

    public SpawnItem(Sprite lDisplaySprite, SpawnType lItemToSpawn )
    {
        mDisplaySprite = lDisplaySprite;
        itemToSpawn = lItemToSpawn;
    }

    public void Select(Transform lEquipedBy)
    {
        //Functionality to display gun on character when selected
    }

    public void Use()
    {
        //Functionality to spawn item
    }

    public void UnSelect()
    {

    }
    public void AbortUse()
    {

    }
    public bool Reload(ResourcePool ammoStorage)
    {
        return false;
    }

    public void AbortReload()
    {

    }

    public List<ISkill> GetSkillsFromItem()
    {
        return null;
    }

}
