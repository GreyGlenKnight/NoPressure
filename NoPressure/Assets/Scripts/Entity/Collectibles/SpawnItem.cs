using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItem : IInventoryItem {

    public Transform itemToSpawn;
    //public Transform detectCollisionSpawn;
    SpawnType RemoveMe;

    public override IInventoryItem Clone()
    {
        return new SpawnItem(mDisplaySprite, RemoveMe);
    }

    public override void UpdateTime()
    {
        
    }

    public SpawnItem(Sprite lDisplaySprite, SpawnType lItemToSpawn )
    {
        mDisplaySprite = lDisplaySprite;
        RemoveMe = lItemToSpawn;
    }

    public override void Select(Transform lEquipedBy)
    {
        base.Select(lEquipedBy);
        //Functionality to display gun on character when selected
    }

    public override void Use()
    {
        base.Use();
        //Functionality to spawn item
    }

    public override void UnSelect()
    {
        base.UnSelect();
    }
    public override void AbortUse()
    {
        base.AbortUse();
    }
    public override bool Reload(ResourcePool ammoStorage)
    {
        base.Reload(ammoStorage);
        return false;
    }

    public override void AbortReload()
    {
        base.AbortReload();
    }

    public override List<ISkill> GetSkillsFromItem()
    {
        return null;
    }

    // Fail the curent reload attempt, Resets the reload timers
    public override void ClickTile(MapTile clickedTile)
    {
        Player player = mEquipedBy.GetComponent<Player>();

        if (player == null)
        {
            Debug.Log("only players can use items");
        }

        Instantiate(itemToSpawn, new Vector3(clickedTile.m_Location.x, 0f, clickedTile.m_Location.y), Quaternion.Euler(0f,0f,0f));
        player.DestroyCurrentItem();
    }

}
