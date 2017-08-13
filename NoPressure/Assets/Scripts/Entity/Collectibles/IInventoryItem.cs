using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TODO: Set a mesh to an inventory item so we can alter how the user of the item looks

public interface IInventoryItem {

    // The display icon of the item
    Sprite mDisplaySprite { get; }

    // The MonoBehaviour object that is currenly using this item
    Transform mEquipedBy { get; }

    // The amount of uses the item has
    ResourcePool mCharges { get; }

    // Some items have cooldown timers, because items are not are not MonoBehaviour items
    // they need the MonoBehaviour user of the item to call it.
    void UpdateTime();

    // An item can create another of itself, used in the ItemManager to create 
    // new items as needed from prototype items. 
    IInventoryItem Clone();

    // Called when this item becomes the active item (equiped)
    void Select(Transform lEquipedBy);

    // Called when this item becomes an inactive item (unequiped)
    void UnSelect();

    // Called when the user presses the use item button
    void Use();

    // Called when the user releases the use item button
    void AbortUse();

    // Called when trying to refill the charges with resources from storage
    bool Reload(ResourcePool lResourceStorage);

    // Fail the curent reload attempt, Resets the reload timers
    void AbortReload();

    List<ISkill> GetSkillsFromItem();

}
