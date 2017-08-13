using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolsItem : IInventoryItem {

    public enum ToolActions
    {
        OpenDoor,//Mec
        OpenVent,//Mec
        OpenLock,//Mec
        DisableTrap,//Elc
        RepairConsole,//Elc
    }

    public List<ISkill> ItemSkills;

    // The display icon of the item
    public Sprite mDisplaySprite { get; protected set; }

    // The MonoBehaviour object that is currenly using this item
    public Transform mEquipedBy { get; protected set; }

    // The amount of uses the item has
    public ResourcePool mCharges { get; protected set; }

    List<ToolActions> toolActionList;

    public void UpdateTime()
    {

    }

    public IInventoryItem Clone()
    {
        return new ToolsItem(mDisplaySprite, toolActionList);
    }

    public ToolsItem (Sprite lDisplaySprite, List<ToolActions> lToolActions)
    {
        toolActionList = lToolActions;
        mDisplaySprite = lDisplaySprite;
        ItemSkills = new List<ISkill>();

        if (toolActionList.Contains(ToolActions.OpenVent))
        {
            ItemSkills.Add(new ActionSkill("OpenVent", 1));
        }
        if (toolActionList.Contains(ToolActions.DisableTrap))
        {
            ItemSkills.Add(new ActionSkill("DisableTrap", 2));
        }


    }

    public void Select(Transform lEquipedBy)
    {
        mEquipedBy = lEquipedBy;

        Player player = mEquipedBy.GetComponent<Player>();

        if (player == null)
        {
            Debug.Log("Help the Ai is using tools again!");
            return;
        }

        //Functionality to display gun on character when selected
    }

    public void Use()
    {
        //Functionality to spawn item
    }

    public void UnSelect()
    {
        if (mEquipedBy == null)
        {
            return;
        }

    }
    public void AbortUse()
    {

    }

    public bool Reload(ResourcePool lResourceStorage)
    {
        return false;
    }

    public void AbortReload()
    {

    }

    public List<ISkill> GetSkillsFromItem()
    {
        return ItemSkills;
    }

}
