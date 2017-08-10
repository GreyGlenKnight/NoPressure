using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager {

    static Dictionary<SpawnType, IInventoryItem> Items;

    public static void LoadItemTypes()
    {
        Items = new Dictionary<SpawnType, IInventoryItem>();


        ResourcePool chargesPistol = new ResourcePool(ResourceType.MassDriver, 12);
        Items.Add(SpawnType.Pistol, 
            new GunItem(SpriteManager.GetInventoryItemSprite(SpawnType.Pistol),7, chargesPistol, GunItem.FireMode.Single));

        ResourcePool chargesRifle = new ResourcePool(ResourceType.MassDriver, 24);
        Items.Add(SpawnType.Rifle,
            new GunItem(SpriteManager.GetInventoryItemSprite(SpawnType.Rifle), 5, chargesRifle, GunItem.FireMode.Auto));


        ResourcePool chargesCarbine = new ResourcePool(ResourceType.EnergyCell, 10);
        Items.Add(SpawnType.Carbine,
            new GunItem(SpriteManager.GetInventoryItemSprite(SpawnType.Carbine), 60, chargesCarbine, GunItem.FireMode.Charge));

        Items.Add(SpawnType.Shield,
            new BoostItem(SpriteManager.GetInventoryItemSprite(SpawnType.Shield),BoostItem.StatBoost.Shield, 100));

        List<ToolsItem.ToolActions> MechanicalToolsActions = new List<ToolsItem.ToolActions>();
        MechanicalToolsActions.Add(ToolsItem.ToolActions.OpenVent);
        MechanicalToolsActions.Add(ToolsItem.ToolActions.OpenLock);
        MechanicalToolsActions.Add(ToolsItem.ToolActions.OpenDoor);

        Items.Add(SpawnType.MecanicalTools,
            new ToolsItem(SpriteManager.GetInventoryItemSprite(SpawnType.MecanicalTools), MechanicalToolsActions));


        List<ToolsItem.ToolActions> ElectricalToolsActions = new List<ToolsItem.ToolActions>();
        ElectricalToolsActions.Add(ToolsItem.ToolActions.DisableTrap);
        ElectricalToolsActions.Add(ToolsItem.ToolActions.RepairConsole);

        Items.Add(SpawnType.ElectricalTools,
            new ToolsItem(SpriteManager.GetInventoryItemSprite(SpawnType.ElectricalTools), ElectricalToolsActions));

        Items.Add(SpawnType.Mine,
            new SpawnItem(SpriteManager.GetInventoryItemSprite(SpawnType.Mine), SpawnType.Mine));
    }

    public static IInventoryItem SpawnItem(SpawnType typeToSpawn)
    {
        IInventoryItem returnItem = null;

        if (Items == null)
            LoadItemTypes();

        if (Items.ContainsKey(typeToSpawn))
            Items.TryGetValue(typeToSpawn, out returnItem);
        else
        {
            Debug.Log("Missing Value for ItemType: " + typeToSpawn);
            return null;
        }

        return returnItem.Clone();
    }

}
