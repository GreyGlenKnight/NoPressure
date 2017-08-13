using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// A boost type item will apply an effect on the user.
public class BoostItem: IInventoryItem {

    // The display icon of the item
    public Sprite mDisplaySprite { get; protected set; }

    // The MonoBehaviour object that is currenly using this item
    public Transform mEquipedBy { get; protected set; }

    // The amount of uses the item has
    public ResourcePool mCharges { get; protected set; }

    public enum StatBoost
    {
        None,
        Shield,
        Oxygen,
    }

    int boostAmount;
    StatBoost boostType;

    public void UpdateTime()
    {

    }

    public IInventoryItem Clone()
    {
        return new BoostItem(mDisplaySprite, boostType, boostAmount);

    }

    public BoostItem(Sprite lDisplaySprite, StatBoost lBoostType, int lAmount)
    {
        mDisplaySprite = lDisplaySprite;
        boostType = lBoostType;
        boostAmount = lAmount;
    }


    public void Select(Transform lEquipedBy)
    {
        mEquipedBy = lEquipedBy;
    }

    public void Use()
    {
        if (mEquipedBy == null)
        {
            Debug.Log("can not use an item that is not equiped");
        }

        Player player = mEquipedBy.GetComponent<Player>();

        if (player == null)
        {
            Debug.Log("only players can use items");
        }

        switch (boostType)
        {
            case StatBoost.Shield:
                player.mShield += boostAmount;
                player.DestroyCurrentItem();
                break;

            case StatBoost.Oxygen:
                player.mPressure += boostAmount;
                player.DestroyCurrentItem();
                break;

            default:
                Debug.Log("boostType not set! no action taken");
                break;
        }
    }

    public void UnSelect() {}
    public void AbortUse() {}

    public bool Reload(ResourcePool lStorage)
    {
        return false;
    }

    public void AbortReload() {}

    public List<ISkill> GetSkillsFromItem()
    {
        return null;
    }
}
