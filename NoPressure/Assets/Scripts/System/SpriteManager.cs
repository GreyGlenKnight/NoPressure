using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager  {

    static Dictionary<SpawnType, Sprite> InventoryItemSprites;

    public static void LoadSprites()
    {
        InventoryItemSprites = new Dictionary<SpawnType, Sprite>();
        Sprite[] itemSprites = Resources.LoadAll<Sprite>("Items\\");

        if (itemSprites.Length != 7)
        {
            Debug.LogError("Expecting 7 item sprites, not " + itemSprites.Length + " please update load sprites");
            itemSprites = Resources.LoadAll<Sprite>("InventoryItems");
            if (itemSprites.Length != 7)
                return;
        }
        InventoryItemSprites.Add(SpawnType.ElectricalTools, itemSprites[0]);
        InventoryItemSprites.Add(SpawnType.MecanicalTools, itemSprites[1]);
        InventoryItemSprites.Add(SpawnType.Pistol, itemSprites[2]);
        InventoryItemSprites.Add(SpawnType.Shield, itemSprites[3]);
        InventoryItemSprites.Add(SpawnType.Mine, itemSprites[4]);
        InventoryItemSprites.Add(SpawnType.Rifle, itemSprites[5]);
        InventoryItemSprites.Add(SpawnType.Carbine, itemSprites[6]);


    }

    public static Sprite GetInventoryItemSprite(SpawnType itemType)
    {
        if (InventoryItemSprites == null)
        {
            LoadSprites();
        }

        Sprite returnSprite = null;

        if (InventoryItemSprites.ContainsKey(itemType))
        {
            InventoryItemSprites.TryGetValue(itemType, out returnSprite);
        }
        else
        {
            Debug.LogError("Can not find a sprite for itemType: " + itemType);
        }

        //Debug.Log("request sprite: " + itemType + " returning: " + returnSprite.name);

        return returnSprite;

    }


}
