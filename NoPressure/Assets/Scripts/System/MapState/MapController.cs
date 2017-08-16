using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapController : MonoBehaviour
{
    // public Settings

    // How many sectors [10x10 Tiles] are to be kept in memory in each direction
    // 0 = 1 Sector loaded [1x1 sectors in memory]
    // 1 = 9 Sectors loaded [3x3 sectors in memory]
    // 2 = 25 Sectors loaded [5x5 sectors in memory] * recommended
    // 3 = 49 Sectors loaded [7x7 sectors in memory] 
    // 4 = 81 Sectors loaded [9x9 sectors in memory]
    public int pLoadRadius = 2;

    //   How often to check if the focus has moved to another sector and to 
    // schedule loading of sectors within the load radius and unload sectors
    // outside that range
    public float pUpdateDelay = 1f;

    // Current time counter for update delay
    float mUpdateCurrent = 0f;

    // TODO: Get from camera controller
    public Transform CameraFocus { get; set; }
    
    PrefabSpawner spawner;

    Coord CurrentSector;
    Coord CurrentMapNode;

    PathfindingManager pathfindingManager;
    PrefabSpawner prefabSpawner;
    Map map;
    Queue<MapSector> SectorsToSpawnQueue;
    Queue<MapSector> SectorsToDespawnQueue;

    TheDynamicLoader gDynamicLoader;

    private void Awake()
    {
        SectorsToSpawnQueue = new Queue<MapSector>();
        SectorsToDespawnQueue = new Queue<MapSector>();
    }

    private void Start()
    {
        Debug.Log("LoadRadius: " + pLoadRadius);
        if (CameraFocus == null)
        {
            CameraFocus = GameObject.Find("Player").transform;
            CurrentSector = new Coord((int)CameraFocus.position.x, (int)CameraFocus.position.z);
        }

        pathfindingManager = PathfindingManager.getPathfindingManager();
        //prefabSpawner = PrefabSpawner.GetPrefabSpawner();
        gDynamicLoader = TheDynamicLoader.getDynamicLoader();
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
        if (CameraFocus == null)
            Debug.LogError("Can not find player");

        mUpdateCurrent += Time.deltaTime;
        if(mUpdateCurrent >= pUpdateDelay )
        {
            mUpdateCurrent = 0;
            Coord focusSector = map.ConvertToSectorSpace(WorldSpaceUnit.Tile,
                new Coord((int)CameraFocus.position.x, (int)CameraFocus.position.z));

            if (focusSector != CurrentSector)
            {
                DespawnSectors(focusSector);
                CurrentSector = focusSector;

                LoadSectorPerformance(WorldSpaceUnit.Sector, CurrentSector);

                // Pathfinder mesh update
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

                TheDynamicLoader.getDynamicLoader().Rest = true;

            }
        }
    }

    public void DespawnCol(int Col)
    {
        DespawnCol(Col, 0, 40);
    }

    public void DespawnCol(int Col, int LowerBound, int UpperBound)
    {

        for (int i = LowerBound; i < UpperBound; i++)
        {
            MapSector despawnSector = map.GetSectorAt(WorldSpaceUnit.Sector, new Coord(i, Col),"Demo");
            if (despawnSector != null)
            {
                if (despawnSector.pIsLoaded == true)
                {
                    ScheduleDespawnSector(despawnSector);
                }
            }
        }
    }

    public void DespawnRow(int Row)
    {
        DespawnRow(Row, 0, 40);
    }

    public void DespawnRow(int Row, int LowerBound, int UpperBound)
    {
        for (int i = LowerBound; i < UpperBound; i++)
        {
            MapSector despawnSector = map.GetSectorAt(WorldSpaceUnit.Sector, new Coord(Row, i),"Demo");
            if (despawnSector != null)
                if (despawnSector.pIsLoaded == true)
                    ScheduleDespawnSector(despawnSector);
        }
    }

    public void DespawnSectors(Coord focusSector)
    {
        int xChange = focusSector.x - CurrentSector.x;

        int yChange = focusSector.y - CurrentSector.y;

        List<Transform> RemoveEntities = new List<Transform>();

        if (xChange > 0)
        {
            for (int i = 0; i < xChange; i++)
            {
                DespawnRow(CurrentSector.x - i - pLoadRadius);
            }
            Debug.Log("Despawn left of :" + (focusSector.x - pLoadRadius));
            RemoveEntities = prefabSpawner.FindEntityLeftOf(focusSector.x - pLoadRadius );
        }

        else if (xChange < 0)
        {
            for (int i = 0; i < -xChange; i++)
            {
                DespawnRow(CurrentSector.x + i + pLoadRadius);
            }
            Debug.Log("Despawn right of :" + (focusSector.x + pLoadRadius));
            RemoveEntities = prefabSpawner.FindEntityRightOf(focusSector.x + pLoadRadius );
        }

        if (RemoveEntities.Count >0)
        {
            prefabSpawner.DespawnList(RemoveEntities);
            RemoveEntities = new List<Transform>();
        }

        if (yChange > 0)
        {
            for (int i = 0; i < yChange; i++)
            {
                DespawnCol(CurrentSector.y - i - pLoadRadius);
            }
            Debug.Log("Despawn Below of :" + (focusSector.y - pLoadRadius));
            RemoveEntities = prefabSpawner.FindEntityBelow(focusSector.y - pLoadRadius );
        }

        else if (yChange < 0)
        {
            for (int i = 0; i < -yChange; i++)
            {
                DespawnCol(CurrentSector.y + i + pLoadRadius);
            }
            Debug.Log("Despawn Above of :" + (focusSector.y + pLoadRadius));
            RemoveEntities = prefabSpawner.FindEntityAbove(focusSector.y + pLoadRadius );
        }

        if (RemoveEntities.Count > 0)
        {
            prefabSpawner.DespawnList(RemoveEntities);
        }
    }

    public void ResetLevel()
    {
        map.SetAllSectorsToNotLoaded();
    }

    // Load the map starting at location
    public void Init(WorldSpaceUnit unit, Coord location, PrefabSpawner lPrefabSpawner)
    {
        map = new Map();

        prefabSpawner = lPrefabSpawner;
        prefabSpawner.Init(map.SaveLocationOnMap);

        Player player = GameObject.Find("Player").GetComponent<Player>();

        player.mAttemptDropItem += lPrefabSpawner.CreateCrate;

        Debug.Log("Init Map");

        CurrentMapNode = map.ConvertToMapNode(unit, location);
        CurrentSector = map.ConvertToSectorSpace(unit,location);

        if (pathfindingManager == null)
            pathfindingManager = PathfindingManager.getPathfindingManager();

        pathfindingManager.Init(CurrentMapNode);

        LoadSector(unit, location);
    }

    // Load everything in one frame, only call during Init
    public void LoadSector(WorldSpaceUnit unit, Coord location)
    {
        List<MapSector> SpawnSectors = FindSectorsToSpawn(unit,location);

        for (int i =0; i<SpawnSectors.Count; i++)
            prefabSpawner.SpawnSector(SpawnSectors[i]);
    }

    // Load sector over several frames.
    public void LoadSectorPerformance(WorldSpaceUnit unit, Coord location)
    {
        List<MapSector> SpawnSectors = FindSectorsToSpawn(unit, location);

        for (int i = 0; i < SpawnSectors.Count; i++)
            ScheduleSpawnSector(SpawnSectors[i]);
    }

    private List<MapSector> FindSectorsToSpawn(WorldSpaceUnit unit, Coord location)
    {
        Coord StartSectorLocation = map.ConvertToSectorSpace(unit, location);
        List<MapSector> returnVal = new List<MapSector>();

        for (int i = -pLoadRadius; i <= pLoadRadius; i++)
            for (int j = -pLoadRadius; j <= pLoadRadius; j++)
            {
                Coord SectorLocation = new Coord(StartSectorLocation.x + i, StartSectorLocation.y + j);
                MapSector mapSector = map.GetSectorAt(WorldSpaceUnit.Sector, SectorLocation, "Demo");
                if (mapSector != null)
                {
                    returnVal.Add(mapSector);
                }
            }
        return returnVal;
    }

    public void ScheduleSpawnSector(MapSector lSectorToSpawn)
    {
        SectorsToSpawnQueue.Enqueue(lSectorToSpawn);
        gDynamicLoader.AddActionToQueue(SpawnNextSector, Priority.High);
    }

    public void ScheduleDespawnSector(MapSector lSectorToDespawn)
    {
        SectorsToDespawnQueue.Enqueue(lSectorToDespawn);
        gDynamicLoader.AddActionToQueue(DespawnNextSector, Priority.Low);
    }

    // Expensive action, call only one expensive action per frame with the help of DynamicLoader
    public void SpawnNextSector()
    {
        if (SectorsToSpawnQueue.Count == 0)
        {
            Debug.Log("Warning, Dynamic loader and SectorsToSpawnQueue out of sync");
            return;
        }
        prefabSpawner.SpawnSector(SectorsToSpawnQueue.Dequeue());
    }

    // Expensive action, call only one expensive action per frame with the help of DynamicLoader
    public void DespawnNextSector()
    {
        if (SectorsToDespawnQueue.Count == 0)
        {
            Debug.Log("Warning, Dynamic loader and SectorsToDespawnQueue out of sync");
            return;
        }

        prefabSpawner.DespawnSector(SectorsToDespawnQueue.Dequeue());
    }

}
