using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapController : MonoBehaviour
{
    // How many sectors [10x10 Tiles] are to be kept in memory in each direction
    // 0 = 1 Sector loaded [1x1 sectors in memory]
    // 1 = 9 Sectors loaded [3x3 sectors in memory]
    // 2 = 25 Sectors loaded [5x5 sectors in memory] * recommended
    // 3 = 49 Sectors loaded [7x7 sectors in memory] 
    // 4 = 81 Sectors loaded [9x9 sectors in memory]
    public int pLoadRadius = 2;

    // TODO: Get from camera controller
    public Transform CameraFocus { get; set; }
    
    Coord CurrentSector;
    Coord CurrentMapNode;

    PrefabSpawner prefabSpawner;
    Map map;
    PathfindingManager pathfindingManager;

    // Dynamic loading
    TheDynamicLoader gDynamicLoader;
    Queue<MapSector> SectorsToSpawnQueue;
    Queue<MapSector> SectorsToDespawnQueue;
    List<MapSector> DoNotDespawn;
    List<MapSector> CurrentlySpawnedSectors;
    List<MapSector> ScheduledToBeDespawned;

    int upkeepTilesIndex = 0;

    // Load the map starting at location
    public void Init(WorldSpaceUnit unit, Coord location, PrefabSpawner lPrefabSpawner)
    {
        gDynamicLoader = TheDynamicLoader.getDynamicLoader();
        SectorsToSpawnQueue = new Queue<MapSector>();
        SectorsToDespawnQueue = new Queue<MapSector>();
        CurrentlySpawnedSectors = new List<MapSector>();
        DoNotDespawn = new List<MapSector>();
        ScheduledToBeDespawned = new List<MapSector>();

        map = new Map();

        prefabSpawner = lPrefabSpawner;
        prefabSpawner.Init(map.SaveLocationOnMap);

        Player player = GameObject.Find("Player").GetComponent<Player>();

        // Give player functionality to create Crates
        player.mAttemptDropItem += lPrefabSpawner.CreateCrate;

        // Move the player to the starting location
        player.transform.position = new Vector3(location.x, 1, location.y);

        // Store the current MapNode and Sector to detect when to concern ourselves
        // with loading new sectors and despawn thoes that are out of bounds.
        CurrentMapNode = map.ConvertToMapNode(unit, location);
        CurrentSector = map.ConvertToSectorSpace(unit, location);

        // Init pathfinding system
        pathfindingManager = new PathfindingManager(CurrentMapNode);

        // Load the current sector and sector within pLoadRadius into memory
        LoadSector(unit, location);

        // Schedule upkeep to spawn sectors within bounds
        gDynamicLoader.AddActionToQueue(FindSectorsToSpawnUpkeep, Priority.Low);

        // Schedule upkeep to move pathfinding current location
        gDynamicLoader.AddActionToQueue(PathfinderManagerUpkeep, Priority.Low);

        // Schedule upkeep to despawn units out of bounds
        gDynamicLoader.AddActionToQueue(FindUnitsToDespawnUpkeep, Priority.Low);

        // Schedule upkeep to despawn sectors out of bounds
        gDynamicLoader.AddActionToQueue(FindSectorsToDespawnUpkeep, Priority.Low);

        // Schedule upkeep to spread pressure
        gDynamicLoader.AddActionToQueue(SpreadPressureUpkeep, Priority.Low);

        gDynamicLoader.AddActionToQueue(FindPowerNetworksUpkeep, Priority.Low);

        gDynamicLoader.AddActionToQueue(CalcPowerUpkeep, Priority.Low);

        prefabSpawner.FindPowerNetworks();
        PowerNetwork.CalcPowerOnAllNetworks();

    }

    public MapTile getTileAt(Coord location)
    {
        return map.GetMapTileAt(location);
    }

    public void FindPowerNetworksUpkeep()
    {
        prefabSpawner.FindPowerNetworks();
        gDynamicLoader.AddActionToQueue(FindPowerNetworksUpkeep, Priority.Low);
    }

    public void CalcPowerUpkeep()
    {
        PowerNetwork.CalcPowerOnAllNetworks();
        gDynamicLoader.AddActionToQueue(CalcPowerUpkeep, Priority.Low);
    }

    public void SpreadPressureUpkeep()
    {
        if (upkeepTilesIndex < prefabSpawner.mSectors.Count)
            prefabSpawner.GeneratePressureUpkeep(upkeepTilesIndex);
        upkeepTilesIndex++;

        //if (upkeepTilesIndex < prefabSpawner.mSectors.Count)
        //    prefabSpawner.GeneratePressureUpkeep(upkeepTilesIndex);
        //upkeepTilesIndex++;

        //if (upkeepTilesIndex < prefabSpawner.mSectors.Count)
        //    prefabSpawner.GeneratePressureUpkeep(upkeepTilesIndex);
        //upkeepTilesIndex++;

        //if (upkeepTilesIndex < prefabSpawner.mSectors.Count)
        //    prefabSpawner.GeneratePressureUpkeep(upkeepTilesIndex);
        //upkeepTilesIndex++;

        if (upkeepTilesIndex < prefabSpawner.mSectors.Count)
            gDynamicLoader.AddActionToQueue(SpreadPressureUpkeep, Priority.Medium);
        else
        {
            upkeepTilesIndex = 0;
            gDynamicLoader.AddActionToQueue(SpreadPressureUpkeep, Priority.Low);
        }
    }

    private void Update()
    {
        if (CameraFocus == null)
        {
            GameObject temp = GameObject.Find("Player");

            if (temp == null)
            {
                Debug.Log("Player Is null");
                return;
            }
            CameraFocus = temp.transform;

            CurrentSector = new Coord((int)CameraFocus.position.x, (int) CameraFocus.position.z);
        }
    }

    public void PathfinderManagerUpkeep()
    {
        if (CameraFocus == null)
        {
            Debug.Log("Can not find focus(player)");
            gDynamicLoader.AddActionToQueue(PathfinderManagerUpkeep, Priority.Low);
            return;
        }
        Coord NewMapNode = map.ConvertToMapNode(WorldSpaceUnit.Sector, CurrentSector);

        if (NewMapNode.x > CurrentMapNode.x)
            pathfindingManager.MovePlayerMapNodeRight();
        else if (NewMapNode.x < CurrentMapNode.x)
            pathfindingManager.MovePlayerMapNodeLeft();

        if (NewMapNode.y > CurrentMapNode.y)
            pathfindingManager.MovePlayerMapNodeUp();
        else if (NewMapNode.y < CurrentMapNode.y)
            pathfindingManager.MovePlayerMapNodeDown();

        CurrentMapNode = NewMapNode;

        gDynamicLoader.AddActionToQueue(PathfinderManagerUpkeep, Priority.Low);
    }

    public void FindSectorsToSpawnUpkeep()
    {
        if (CameraFocus == null)
        {
            Debug.Log("Can not find focus(player)");
            gDynamicLoader.AddActionToQueue(FindSectorsToSpawnUpkeep, Priority.Low);
            return;
        }
        Coord newSector = map.ConvertToSectorSpace(WorldSpaceUnit.Tile,
            new Coord((int)CameraFocus.position.x, (int)CameraFocus.position.z));

        if (newSector != CurrentSector)
        {
            //DespawnSectors(focusSector);
            CurrentSector = newSector;

            ScheduleLoadSectors(WorldSpaceUnit.Sector, CurrentSector);
        }

        gDynamicLoader.AddActionToQueue(FindSectorsToSpawnUpkeep, Priority.Low);
    }

    public void FindUnitsToDespawnUpkeep()
    {
        prefabSpawner.DespawnAbove(CurrentSector.y + pLoadRadius);
        prefabSpawner.DespawnBelow(CurrentSector.y - pLoadRadius);
        prefabSpawner.DespawnRightOf(CurrentSector.x + pLoadRadius);
        prefabSpawner.DespawnLeftOf(CurrentSector.x - pLoadRadius);

        // Schedule this upkeep action back into DynamicLoader
        gDynamicLoader.AddActionToQueue(FindUnitsToDespawnUpkeep, Priority.Low);
    }

    // Upkeep action to despawn sectors when outside loading radius
    public void FindSectorsToDespawnUpkeep()
    {
        for (int i = 0; i < CurrentlySpawnedSectors.Count; i++)
        {
            int distance;
            distance = map.GetDistance(CurrentlySpawnedSectors[i], CurrentSector);
            
            // Sectors outside the bounds are queued to be despawned
            if (distance > pLoadRadius)
            {
                if (ScheduledToBeDespawned.Contains(CurrentlySpawnedSectors[i]))
                {
                    continue;
                }

                // Do not add to list if already queued to despawn
                if (SectorsToDespawnQueue.Contains(CurrentlySpawnedSectors[i]))
                {
                    Debug.Log("Attempting to despawn a sector twice");
                    continue;
                }
                ScheduleDespawnSector(CurrentlySpawnedSectors[i]);
            }
        }
        // Schedule this upkeep action back into DynamicLoader
        gDynamicLoader.AddActionToQueue(FindSectorsToDespawnUpkeep, Priority.Low);
    }

    // Resets Map Data, b/c Non-Monobehaviour objects keep their state between Scenes
    public void ResetLevel()
    {
        gDynamicLoader.ClearQueue();
        map.SetAllSectorsToNotLoaded();
        
    }

    // Load everything in one frame, only call while loading
    public void LoadSector(WorldSpaceUnit unit, Coord location)
    {
        List<MapSector> SpawnSectors = map.FindSectorsAroundLocation(unit, location, pLoadRadius);

        for (int i =0; i<SpawnSectors.Count; i++)
            prefabSpawner.SpawnSector(SpawnSectors[i]);
    }

    // Load sectors over several frames.
    public void ScheduleLoadSectors(WorldSpaceUnit unit, Coord location)
    {
        List<MapSector> SpawnSectors = map.FindSectorsAroundLocation(unit, location,pLoadRadius);

        for (int i = 0; i < SpawnSectors.Count; i++)
            ScheduleSpawnSector(SpawnSectors[i]);
    }

    // Tell the DynamicLoader that there is a sector to spawn
    public void ScheduleSpawnSector(MapSector lSectorToSpawn)
    {
        // Check if the sector is in the despawn queue list
        if (SectorsToDespawnQueue.Contains(lSectorToSpawn))
        {
            // This should be a rare event, it may be useful to log
            Debug.Log("Trying to spawn item on the despawn list");
            // If we find a match, make sure that we dont despawn it
            DoNotDespawn.Add(lSectorToSpawn);
            return;
        }
        // We keep the sector informtaion locally
        SectorsToSpawnQueue.Enqueue(lSectorToSpawn);

        // Enqueue a SpawnNextSector() function call that will happen in a future frame
        gDynamicLoader.AddActionToQueue(SpawnNextSector, Priority.High);
    }

    // Tell DynamicLoader that there is a sector to despawn
    public void ScheduleDespawnSector(MapSector lSectorToDespawn)
    {
        // Add to list scheduled to be despawned
        ScheduledToBeDespawned.Add(lSectorToDespawn);

        SectorsToDespawnQueue.Enqueue(lSectorToDespawn);
        gDynamicLoader.AddActionToQueue(DespawnNextSector, Priority.Medium);
    }

    // Expensive action, call only one expensive action per frame with the help of DynamicLoader
    public void SpawnNextSector()
    {
        if (SectorsToSpawnQueue.Count == 0)
        {
            Debug.Log("Warning, Dynamic loader and SectorsToSpawnQueue out of sync");
            return;
        }

        MapSector sectorToSpawn = SectorsToSpawnQueue.Dequeue();
        // Load Sector into memory
        prefabSpawner.SpawnSector(sectorToSpawn);

        // Keep track of currently spawned sectors
        CurrentlySpawnedSectors.Add(sectorToSpawn);
    }

    // Expensive action, call only one expensive action per frame with the help of DynamicLoader
    public void DespawnNextSector()
    { 
        if(DoNotDespawn.Contains(SectorsToDespawnQueue.Peek()))
        {
            // This should be a rare event, it may be useful to log
            Debug.Log("Decided to not despawn a sector");
            DoNotDespawn.Remove(SectorsToDespawnQueue.Peek());
            return;
        }

        if (SectorsToDespawnQueue.Count == 0)
        {
            Debug.Log("Warning, Dynamic loader and SectorsToDespawnQueue out of sync");
            return;
        }
        MapSector sectorToDespawn = SectorsToDespawnQueue.Dequeue();

        ScheduledToBeDespawned.Remove(sectorToDespawn);
        // Remove from loaded sectors list
        CurrentlySpawnedSectors.Remove(sectorToDespawn);
        // Unload sector from memory
        prefabSpawner.DespawnSector(sectorToDespawn);
    }
}
