using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapController : MonoBehaviour
{
    private static MapController instance;

    public Transform CameraFocus { get; set; }
    const int LoadRadius = 2;// Max = 4 (64 sectors or 6400 tiles)
    string levelName = "Demo";

    PrefabSpawner spawner;

    Coord CurrentSector;
    Coord CurrentMapNode;

    float updateDelay = 1f;
    float updateCurrent = 0f;

    PathfindingManager pathfindingManager;
    PrefabSpawner prefabSpawner;
    Map map;
    Queue<MapSector> SectorsToSpawnQueue;
    Queue<MapSector> SectorsToDespawnQueue;

    public bool LoadNextSector()
    {
        if (SectorsToSpawnQueue.Count == 0)
            return false;

        PrefabSpawner.GetPrefabSpawner().SpawnSector(SectorsToSpawnQueue.Dequeue());
        return true;
    }

    public bool DespawnNextSector()
    {
        if (SectorsToDespawnQueue.Count == 0)
            return false;

        PrefabSpawner.GetPrefabSpawner().DespawnSector(SectorsToDespawnQueue.Dequeue());
        return true;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        SectorsToSpawnQueue = new Queue<MapSector>();
        SectorsToDespawnQueue = new Queue<MapSector>();
    }

    public static MapController GetMapController()
    {
        if (instance == null)
        {
            Debug.Log("Trying to acces MapController befor it has spawned");
        }
        return instance;
    }

    private void Start()
    {

        spawner = PrefabSpawner.GetPrefabSpawner();
        Debug.Log("LoadRadius: " + LoadRadius);
        if (CameraFocus == null)
        {
            CameraFocus = GameObject.Find("Player").transform;
            CurrentSector = new Coord((int)CameraFocus.position.x, (int)CameraFocus.position.z);
        }

        pathfindingManager = PathfindingManager.getPathfindingManager();
        map = Map.getMap();
        prefabSpawner = PrefabSpawner.GetPrefabSpawner();


    }

    private void Update()
    {
        if (CameraFocus == null)
        {
            CameraFocus = GameObject.Find("Player").transform;
            CurrentSector = new Coord((int)CameraFocus.position.x, (int) CameraFocus.position.z);
        }
        if (CameraFocus == null)
            Debug.LogError("Can not find player");

        updateCurrent += Time.deltaTime;
        if(updateCurrent >= updateDelay )
        {
            updateCurrent = 0;
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

                DynamicLoader.getDynamicLoader().Rest = true;

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
            //Debug.Log(new Coord(i, Col));

            MapSector despawnSector = map.GetSectorAt(WorldSpaceUnit.Sector, new Coord(i, Col),"Demo");
            if (despawnSector != null)
            {
                //Debug.Log(new Coord(i, Col));
                if (despawnSector.pIsLoaded == true)
                {
                    SectorsToDespawnQueue.Enqueue(despawnSector);
                    Debug.Log(new Coord(i, Col));
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
            //PrefabSpawner.GetPrefabSpawner().DespawnSector(despawnSector,new Coord(Row, i));
            if (despawnSector != null)
                if (despawnSector.pIsLoaded == true)
                    SectorsToDespawnQueue.Enqueue(despawnSector);
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
                DespawnRow(CurrentSector.x - i - LoadRadius);
            }
            Debug.Log("Despawn left of :" + (focusSector.x - LoadRadius));
            RemoveEntities = prefabSpawner.FindEntityLeftOf(focusSector.x - LoadRadius );
        }

        else if (xChange < 0)
        {
            for (int i = 0; i < -xChange; i++)
            {
                DespawnRow(CurrentSector.x + i + LoadRadius);
            }
            Debug.Log("Despawn right of :" + (focusSector.x + LoadRadius));
            RemoveEntities = prefabSpawner.FindEntityRightOf(focusSector.x + LoadRadius );
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
                DespawnCol(CurrentSector.y - i - LoadRadius);
            }
            Debug.Log("Despawn Below of :" + (focusSector.y - LoadRadius));
            RemoveEntities = prefabSpawner.FindEntityBelow(focusSector.y - LoadRadius );
        }

        else if (yChange < 0)
        {
            for (int i = 0; i < -yChange; i++)
            {
                DespawnCol(CurrentSector.y + i + LoadRadius);
            }
            Debug.Log("Despawn Above of :" + (focusSector.y + LoadRadius));
            RemoveEntities = prefabSpawner.FindEntityAbove(focusSector.y + LoadRadius );
        }

        if (RemoveEntities.Count > 0)
        {
            prefabSpawner.DespawnList(RemoveEntities);
        }

    }


    public void Init(WorldSpaceUnit unit, Coord location)
    {
        CurrentMapNode = map.ConvertToMapNode(unit, location);
        CurrentSector = map.ConvertToSectorSpace(unit,location);

        pathfindingManager.Init(CurrentMapNode);

        LoadSector(unit, location);
    }

    public void LoadSector(WorldSpaceUnit unit, Coord location)
    {
        // Convert everything into Sector units
        Coord StartSectorLocation = map.ConvertToSectorSpace(unit, location);

        for (int i = -LoadRadius; i <= LoadRadius; i++)
            for (int j = -LoadRadius; j <= LoadRadius; j++)
            {
                Coord SectorLocation = new Coord(StartSectorLocation.x + i, StartSectorLocation.y + j);
                MapSector mapSector = map.GetSectorAt(WorldSpaceUnit.Sector, SectorLocation,"Demo");
                if (mapSector != null)
                {
                    //SectorsToSpawnQueue.Enqueue(mapSector);
                    PrefabSpawner.GetPrefabSpawner().SpawnSector(mapSector);
                }
            }
    }

    public void LoadSectorPerformance(WorldSpaceUnit unit, Coord location)
    {
        // Convert everything into Sector units
        Coord StartSectorLocation = map.ConvertToSectorSpace(unit, location);

        for (int i = -LoadRadius; i<= LoadRadius; i++)
            for(int j = -LoadRadius; j<= LoadRadius; j++)
            {
                Coord SectorLocation = new Coord(StartSectorLocation.x + i, StartSectorLocation.y + j);
                MapSector mapSector = map.GetSectorAt(WorldSpaceUnit.Sector, SectorLocation,"Demo");
                if (mapSector != null)
                {
                    SectorsToSpawnQueue.Enqueue(mapSector);
                    //PrefabSpawner.GetPrefabSpawner().SpawnSector(mapSector, SectorLocation);
                }
            }
    }

    private void LoadLine(int[] csvLine, int height, int room)
    {
        for (int i = 0; i < csvLine.Length; i++)
        {
            spawner.SpawnPrefab((SpawnType)csvLine[i], new Coord(i, height));
        }
    }

}
