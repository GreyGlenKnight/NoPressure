using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TODO: Set a mesh to an inventory item so we can alter how the user of the item looks

public abstract class IInventoryItem: MonoBehaviour {

    public SoundManager soundManager;

    public AudioClip[] mUseSFX;
    public AudioClip[] mAbortUseSFX;

    public AudioClip[] mEquipSFX;
    public AudioClip[] mDropSFX;
    public AudioClip[] mReloadSFX;

    // The display icon of the item
    public Sprite mDisplaySprite;

    // The MonoBehaviour object that is currenly using this item
    public Transform mEquipedBy;

    // The amount of uses the item has
    public ResourcePool mCharges;

    // Some items have cooldown timers, because items are not are not MonoBehaviour items
    // they need the MonoBehaviour user of the item to call it.
    public abstract void UpdateTime();

    // An item can create another of itself, used in the ItemManager to create 
    // new items as needed from prototype items. 
    public abstract IInventoryItem Clone();

    // Called when this item becomes the active item (equiped)
    public virtual void Select(Transform lEquipedBy)
    {
        mEquipedBy = lEquipedBy;

        if (mEquipSFX != null)
            SoundManager.instance.RandomizeSfx(mEquipSFX);
    }

    // Called when this item becomes an inactive item (unequiped)
    public virtual void UnSelect()
    {
        if (mAbortUseSFX != null)
            SoundManager.instance.RandomizeSfx(mAbortUseSFX);
    }

    // Called when the user presses the use item button
    public virtual void Use()
    {
        if (mUseSFX != null)
            SoundManager.instance.RandomizeSfx(mUseSFX);
    }

    // Called when the user releases the use item button
    public virtual void AbortUse()
    {
        if (mAbortUseSFX.Length == 0)
            SoundManager.instance.RandomizeSfx(mAbortUseSFX);
    }

    // Called when trying to refill the charges with resources from storage
    public virtual bool Reload(ResourcePool lResourceStorage)
    {
        if(mReloadSFX != null)
            SoundManager.instance.RandomizeSfx(mReloadSFX);
        return false;
    }

    // Fail the curent reload attempt, Resets the reload timers
    public virtual void AbortReload()
    {

    }

    // Fail the curent reload attempt, Resets the reload timers
    public virtual void ClickTile(MapTile clickedTile)
    {
        //clickedTile.isBroken = false;
        //clickedTile.isWired = false;
    }

    public virtual void ClickObject(Transform clickedTile)
    {
        //clickedTile.isBroken = false;
        //clickedTile.isWired = false;
    }

    public abstract List<ISkill> GetSkillsFromItem();

}
