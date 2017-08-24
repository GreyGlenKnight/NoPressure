using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolsItem : IInventoryItem {

    public enum ToolActions
    {
        FixBroken,
        AddRemoveWire,
    }

    public List<ISkill> ItemSkills;

    public ToolActions toolAction;

    List<ToolActions> toolActionList;

    public override void UpdateTime()
    {

    }

    public override IInventoryItem Clone()
    {
        return new ToolsItem(mDisplaySprite, toolActionList);
    }

    public ToolsItem(Sprite lDisplaySprite, List<ToolActions> lToolActions)
    {
        
        toolActionList = lToolActions;
        mDisplaySprite = lDisplaySprite;
        ItemSkills = new List<ISkill>();

    }

    public override void Select(Transform lEquipedBy)
    {
        base.Select(lEquipedBy);
        mEquipedBy = lEquipedBy;

        Player player = mEquipedBy.GetComponent<Player>();

        if (player == null)
        {
            Debug.Log("Help the Ai is using tools again!");
            return;
        }

        //Functionality to display gun on character when selected
    }

    public override void Use()
    {
        //Functionality to spawn item
        GameObject.Find("SoundManager");
    }

    public override void UnSelect()
    {
        base.UnSelect();
        if (mEquipedBy == null)
        {
            return;
        }

    }
    public override void AbortUse()
    {
        base.AbortUse();
    }

    public override bool Reload(ResourcePool lResourceStorage)
    {
        base.Reload(lResourceStorage);
        return false;
    }

    public override void AbortReload()
    {
        base.AbortReload();
    }

    public override List<ISkill> GetSkillsFromItem()
    {
        return ItemSkills;
    }

    public override void ClickObject(Transform clickedObject)
    {
        if (toolAction == ToolActions.FixBroken)
        {
            PowerSource ToRepair = clickedObject.GetComponent<PowerSource>();
            if(ToRepair != null)
            {
                ToRepair.Repair();
            }
        }

        //clickedTile.isBroken = false;
        //clickedTile.isWired = false;
    }

    // Fail the curent reload attempt, Resets the reload timers
    public override void ClickTile(MapTile clickedTile)
    {

        if(toolAction == ToolActions.FixBroken)
        {
            clickedTile.isBroken = false;
        }

        if (toolAction == ToolActions.AddRemoveWire)
        {
            if(clickedTile.isWired == true)
                clickedTile.isWired = false;
            else
                clickedTile.isWired = true;
        }

        //clickedTile.isBroken = false;
        //clickedTile.isWired = false;
    }

}
