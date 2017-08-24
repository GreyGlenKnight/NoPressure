using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// A boost type item will apply an effect on the user.
public class BoostItem: IInventoryItem {


    public enum StatBoost
    {
        None,
        Shield,
        Oxygen,
    }

    public int boostAmount;
    public StatBoost boostType;

    public override void UpdateTime()
    {

    }

    public override IInventoryItem Clone()
    {
        return new BoostItem(mDisplaySprite, boostType, boostAmount);

    }

    public BoostItem(Sprite lDisplaySprite, StatBoost lBoostType, int lAmount)
    {
        mDisplaySprite = lDisplaySprite;
        boostType = lBoostType;
        boostAmount = lAmount;
    }


    public override void Select(Transform lEquipedBy)
    {
        base.Select(lEquipedBy);
        mEquipedBy = lEquipedBy;
    }

    public override void Use()
    {
        base.Use();
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

    public override void UnSelect() { base.UnSelect(); }
    public override void AbortUse() { base.AbortUse(); }

    public override bool Reload(ResourcePool lStorage)
    {
        base.Reload(lStorage);
        return false;
    }

    public override void AbortReload() { base.AbortReload(); }

    public override List<ISkill> GetSkillsFromItem()
    {
        return null;
    }
}
