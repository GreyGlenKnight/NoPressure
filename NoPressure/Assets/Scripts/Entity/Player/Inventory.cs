﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory {

    IInventoryItem[] inventory;
    Sprite[] spriteList;

    public ResourcePool mMassDriverAmmo { get; set; }
    public ResourcePool mEnergyCells { get; set; }
    public ResourcePool mExplosives { get; set; }
    public ResourcePool mParts { get; set; }

    int inventoryCap;

    int InventoryCount = 0;
    int CurentlySelected = 0;

    public bool mHasFixPressureSkill
    {
        get
        {
            if (GetSelectionItem() == null)
                return false;

            List<ISkill> skills = GetSelectionItem().GetSkillsFromItem();
            if (skills != null)
            {
                if (skills[0].mName.Equals("OpenVent"))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public bool mHasDisableTrapSkill
    {
        get
        {
            if (GetSelectionItem() == null)
                return false;

            List<ISkill> skills = GetSelectionItem().GetSkillsFromItem();
            if (skills != null)
            {
                if (skills[0].mName.Equals("DisableTrap"))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public void updateTime()
    {
        if (inventory[0] != null)
            inventory[0].UpdateTime();
        if (inventory[1] != null)
            inventory[1].UpdateTime();
        if (inventory[2] != null)
            inventory[2].UpdateTime();
        if (inventory[3] != null)
            inventory[3].UpdateTime();
    }

    public void UpdateResource(ResourcePool lResource)
    {
        switch (lResource.mResourceType)
        {
            case ResourceType.MassDriver:
                mMassDriverAmmo = lResource;
                break;

            case ResourceType.EnergyCell:
                mEnergyCells = lResource;
                break;

            case ResourceType.Explosive:
                mExplosives = lResource;
                break;

            case ResourceType.Parts:
                mParts = lResource;
                break;

            default:
                Debug.Log("Warning, Inventory does not hold " + lResource.mResourceType);
                break;
        }
    }

    public string DisplayChargesForItem(int index)
    {
        if (inventory[index] == null)
        {
            return "";
        }

        if (inventory[index].mCharges == null)
        {
            return "";
        }

        ResourcePool pool = getResource(inventory[index].mCharges.mResourceType);

        return inventory[index].mCharges.mValue + " / " + pool.mValue;
    }

    public void ReloadEquipedWeapon()
    {
        IInventoryItem currentItem = GetSelectionItem();
        if (currentItem == null)
            return;

        ResourceType reloadWithResource = currentItem.mCharges.mResourceType;
        ResourcePool resource = getResource(reloadWithResource);

        currentItem.Reload(resource);
    }

    public ResourcePool getResource(ResourceType resourceType)
    {
        switch (resourceType)
        {
            case ResourceType.EnergyCell:
                return mEnergyCells;
            case ResourceType.Explosive:
                return mExplosives;
            case ResourceType.MassDriver:
                return mMassDriverAmmo;
            case ResourceType.Parts:
                return mParts;
            default:
                Debug.Log("Player does not know Resource: " + resourceType);
                return null;
        }
    }

    public int getInventoryCap()
    {
        return inventoryCap;
    }

    public void RemoveSelectedItem()
    {
        inventory[CurentlySelected] = null;
        spriteList[CurentlySelected] = null;
    }

    public IInventoryItem getItemAt(int index)
    {
        if (index < 0)
            return null;
        if (index > inventoryCap)
            return null;

        return inventory[index];

    }

    public IInventoryItem GetSelectionItem()
    {
        return inventory[CurentlySelected];
    }

    public int getSelectionIndex()
    {
        return CurentlySelected;
    }

    public void setSelection(int lSelection)
    {
        CurentlySelected = lSelection;
    }

    public Inventory(int lInventoryCap)
    {
        inventoryCap = lInventoryCap;
        inventory = new IInventoryItem[inventoryCap];
        spriteList = new Sprite[inventoryCap];
    }

    public Sprite[] getSpriteList()
    {
        return spriteList;
    }

    //return index of the location added ot -1 if not added at all
    public int AddItemToInventory(IInventoryItem itemToAdd)
    {
        if (InventoryCount == inventoryCap)
        {
            return -1;
        }

        if (inventory[CurentlySelected] == null)
        {
            inventory[CurentlySelected] = itemToAdd;
            spriteList[CurentlySelected] = itemToAdd.mDisplaySprite;
            return CurentlySelected;
        }

        else
        {
            for(int i = 0; i< inventoryCap; i++)
            {
                if (inventory[i]==null)
                {
                    inventory[i] = itemToAdd;
                    return i;
                }
            }
            Debug.Log("InventoryCount<Max but cant find non null inventory space");
            return -1;
        }

    }

}
