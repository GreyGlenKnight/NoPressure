using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using System;

public enum TilePosition { Wall, Room, Corridor };

public enum PrefabType
{
    Wall, None, Obstacle, Enemy, Player, Exit, Boss
};

public class BoardManager : MonoBehaviour {

    public enum Direction
    {
        North, East, South, West
    };

    public enum LevelType
    {
        DebugOpenArea,
        RandomSmallRooms,
        FromFile,
        FromFileMap,
    };

    public struct LevelPerams
    {
        public int columns;
        public int rows;
        public IntRange numRooms;
        public IntRange roomWidth;
        public IntRange roomHeight;
        public IntRange corridorLength;
        public IntRange innerWallsPerRoom;
        public IntRange enemiesPerRoom;
        public IntRange obstaclesPerRoom;
        public int difficulty;//Higher is harder
    }

    public LevelType leveltype = LevelType.DebugOpenArea;

    LevelPerams RandomSmallRoomsParams = new LevelPerams
    {
        columns = 50,
        rows = 50,
        numRooms = new IntRange(4, 5),
        roomWidth = new IntRange(4, 10),
        roomHeight = new IntRange(8, 10),
        corridorLength = new IntRange(2, 4),
        innerWallsPerRoom = new IntRange(2, 3),
        enemiesPerRoom = new IntRange(4, 6),
        obstaclesPerRoom = new IntRange(2, 3),
        difficulty = 1,
    };

    LevelPerams DebugOpenAreaParams = new LevelPerams
    {
        columns = 20,
        rows = 20,
        numRooms = new IntRange(1, 1),
        roomWidth = new IntRange(18, 18),
        roomHeight = new IntRange(18, 18),
        corridorLength = new IntRange(1, 1),
        innerWallsPerRoom = new IntRange(1, 1),
        enemiesPerRoom = new IntRange(0, 0),
        obstaclesPerRoom = new IntRange(10, 10),
        difficulty = 0,
    };

    public Transform[] floorTiles;
    public Transform[] wallTiles;
    public Transform[] obstacleTiles;
    public Transform[] enemies;
    public Transform floorBoss;

    // GameObject that acts as a container for all other tiles.
    public static Transform boardHolder;

    // GameObject to act as walking plane for LivingEntity objects
    private Transform groundPlane;
    private NavMeshSurface groundSurface;
    private GameObject player;

    public Transform CratePrefab;

    public Transform ElectricTrapTile;
    public Transform PTSDTile;
    public Transform BrokenPressureStationTile;

    TileInfo[,] tileMap;
    List<Room> rooms;
    List<Corridor> corridors;
    private Color enemyColor = Color.green;
    
    
    public void SetupLevel(LevelPerams levelParams)
    {
        player = GameObject.FindGameObjectWithTag("Player");

        // Position and scale the ground plane
        groundPlane.position = new Vector3(levelParams.columns / 2 - .5f, 0, levelParams.rows / 2 - .5f);
        groundPlane.localScale = new Vector3(levelParams.columns / 10, 1, levelParams.rows / 10);
        // Rebuild the ground's navmesh
        groundSurface.BuildNavMesh();

        tileMap = new TileInfo[levelParams.columns, levelParams.rows];
        CreateRoomsAndCorridors(levelParams);
        SetTileValuesForRooms();
        SetTileValuesForCorridors();

        //LayoutFloor();
        LayoutRoomsAndCorridors(levelParams);
        SetPlayerPosition();
        LayoutRoomObjects(levelParams);
        SetFloorBoss(levelParams.difficulty);

        // TODO: this should be moved to a method.
        // In that method, the GameManager also removes the loading screen set in
        // OnLevelFinishLoading.
        GameManager.instance.loading = false;
    }

    public void SetUpLevel(LevelType lLeveltype)
    {
        //Debug.Log(lLeveltype);

        leveltype = lLeveltype;

        switch (leveltype)
        {
            case LevelType.DebugOpenArea:
                SetupLevel(DebugOpenAreaParams);
                break;
            case LevelType.RandomSmallRooms:
                SetupLevel(RandomSmallRoomsParams);
                break;
            case LevelType.FromFile:


            case LevelType.FromFileMap:

            default:
                Debug.LogError("Leveltype not defined");
                break;
        }
    }

    public void SetUpLevel(LevelType lLeveltype, Coord levelNo)
    {
        //Debug.Log(lLeveltype);

        leveltype = lLeveltype;

        switch (leveltype)
        {
            case LevelType.DebugOpenArea:
                SetupLevel(DebugOpenAreaParams);
                break;

            case LevelType.RandomSmallRooms:
                SetupLevel(RandomSmallRoomsParams);
                break;

            case LevelType.FromFile:
                loadLevelFromFile(levelNo.x * 10 + levelNo.y);
                break;

            case LevelType.FromFileMap:
                loadLevelFromFileMap(new Coord(levelNo.x, levelNo.y ));
                break;

            default:
                Debug.LogError("Leveltype not defined");
                break;
        }
        GameManager.instance.loading = false;

    }

    // StartLocation is in unity units tile space
    public void loadLevelFromFileMap(Coord startLocation)
    {
        MapController mapContoller = MapController.GetMapController();

        mapContoller.LoadSectorIntoMemory(WorldSpaceUnit.Tile, startLocation);

        PrefabSpawner.GetPrefabSpawner().MovePlayer(startLocation.x, startLocation.y);

    }

    public void loadLevelFromFile(int levelNo)
    {
        int[] DilimLine;
        FileLoader fileLoader = new FileLoader();
        string fileName = "Assets\\Levels\\Demo" + levelNo.ToString() + ".csv";

        if (fileLoader.load(fileName) == false)
        {
            Debug.LogError("Failed to load file: " + fileName);
        }

        if (fileLoader.getLineCommaDelim() == null)//step passed header line. it is not used by program
            Debug.LogError("Failure reading file for level " + levelNo.ToString());

        DilimLine = fileLoader.getIntLineCommaDelim();//Param line

        if (DilimLine == null)
            Debug.LogError("Failure reading file for level " + levelNo.ToString());

        int roomNo = DilimLine[0];
        int Width = DilimLine[1];
        int Height = DilimLine[2];

        int priorWidth = Width;
        int priorHeight = Height;

        if(groundPlane == null)
        {
            boardHolder = transform.Find("BoardHolder").transform;
            groundPlane = transform.Find("Ground").transform;
            groundSurface = groundPlane.GetComponent<NavMeshSurface>();
        }

        //Create Nav mesh
        groundPlane.position = new Vector3(Width / 2 - .5f, 0, Height / 2 - .5f);
        groundPlane.localScale = new Vector3(Width / 10, 1, Height / 10);
        // Rebuild the ground's navmesh
        groundSurface.BuildNavMesh();

        tileMap = new TileInfo[Width + 1, Height + 1];

        int currentLine = 0;

        DilimLine = fileLoader.getIntLineCommaDelim();//1st line of the file

        while (DilimLine != null)
        {
            currentLine++;
            Width = DilimLine.Length;
            if (Width != priorWidth)
                Debug.Log("Width of Line: " + currentLine + " (" + Width + ") " + "!=" + priorWidth);

            if (currentLine > Height)
                Debug.Log("currentLine: "+ currentLine+" exceeds Height: " + Height);
            else
            {
                Debug.Log("TODO reminder"); 
                //TODO LoadLine(DilimLine, currentLine, levelNo);
            }
            DilimLine = fileLoader.getIntLineCommaDelim();//nth line of the file
        }
    }

    //private void LoadLine(int[] csvLine,int height, int room)
    //{
    //    for (int i = 0; i < csvLine.Length; i++)
    //    {
    //        tileMap[i, height] = new TileInfo(new Coord(i, height), TilePosition.Room, PrefabType.None, room.ToString());

    //        switch ((SpawnType)csvLine[i])
    //        {
    //            case SpawnType.None:
    //                CreateFloor(i, height);
    //                break;
    //            case SpawnType.Wall:
    //                CreateWall(i, height);
    //                break;
    //            case SpawnType.Player:
    //                MovePlayer(i, height);
    //                break;
    //            case SpawnType.Enemy:
    //                CreateEnemy(i, height);
    //                break;
    //            case SpawnType.Obstacle:
    //                CreateObstacle(i, height);
    //                break;
    //            case SpawnType.Pistol:
    //                CreateCrate(i, height, SpawnType.Pistol);
    //                break;
    //            case SpawnType.Rifle:
    //                CreateCrate(i, height, SpawnType.Rifle);
    //                break;
    //            case SpawnType.Carbine:
    //                CreateCrate(i, height, SpawnType.Carbine);
    //                break;
    //            case SpawnType.Shield:
    //                CreateCrate(i, height, SpawnType.Shield);
    //                break;
    //            case SpawnType.Mine:
    //                CreateCrate(i, height, SpawnType.Mine);
    //                break;
    //            case SpawnType.MecanicalTools:
    //                CreateCrate(i, height, SpawnType.MecanicalTools);
    //                break;
    //            case SpawnType.ElectricalTools:
    //                CreateCrate(i, height, SpawnType.ElectricalTools);
    //                break;
    //            case SpawnType.PressureStation:
    //                CreatePressureTile(i, height);
    //                break;
    //            case SpawnType.BrokenPressureStation:
    //                CreateBrokenPressureStationTile(i, height);
    //                break;
    //            case SpawnType.ElectricTrap:
    //                CreateElectricTrapTile(i, height);
    //                break;
    //            default:
    //                Debug.Log("Invalid item spawn number: " + csvLine[i]);
    //                break;
    //        }
    //    }
    //}

    //public void RemoveCrateFromList(Transform CrateToRemove)
    //{
    //    NoSpawnTilesList.Remove(CrateToRemove);
    //}

    //public void CreatePressureTile(int x, int z)
    //{
    //    if (PTSDTile == null)
    //    {
    //        PTSDTile = GameObject.Find("PressureStation").transform;
    //        if (PTSDTile == null)
    //        {
    //            Debug.Log("Failed loading PTSDTile 2");
    //            return;
    //        }
    //    }

    //    Transform crateObject = Instantiate(PTSDTile, new Vector3(x, 0.2f, z), Quaternion.identity);
    //    NoSpawnTilesList.Add(crateObject);
        
    //}

    //public void CreateBrokenPressureStationTile(int x, int z)
    //{
    //    if (BrokenPressureStationTile == null)
    //    {
    //        BrokenPressureStationTile = GameObject.Find("BrokenPressureStation").transform;
    //        if (BrokenPressureStationTile == null)
    //        {
    //            Debug.Log("Failed loading broken pressure stations 2");
    //            return;
    //        }
    //    }
    //    Transform crateObject = Instantiate(BrokenPressureStationTile, new Vector3(x, 0.2f, z), Quaternion.identity);
    //    NoSpawnTilesList.Add(crateObject);
    //}

    //public void CreateElectricTrapTile(int x, int z)
    //{
    //    if (ElectricTrapTile == null)
    //    {
    //        ElectricTrapTile = GameObject.Find("ElectricTrap").transform;
    //        if (ElectricTrapTile == null)
    //        {
    //            Debug.Log("Failed loading electric traps");
    //            return;
    //        }
    //    }
    //    Transform crateObject = Instantiate(ElectricTrapTile, new Vector3(x, 0.2f, z), Quaternion.identity);
    //    NoSpawnTilesList.Add(crateObject);
    //}

    //public bool CreateCrate(int x, int z, IInventoryItem contents)
    //{
    //    if (CratePrefab == null)
    //    {
    //        CratePrefab = GameObject.Find("Crate").transform;
    //        if (CratePrefab == null)
    //        {
    //            Debug.Log("Crate Prefab is null, assign in editor");
    //            return false;
    //        }
    //    }
    //    Vector2 SpawnLocation;
    //    if (FindNearestEmptyTile(x, z, out SpawnLocation) == false)
    //        return false;

    //    Transform crateObject = Instantiate(CratePrefab, new Vector3(SpawnLocation.x, 1, SpawnLocation.y), Quaternion.identity);
    //    Crate newCrate = crateObject.GetComponent<Crate>();
    //    if (newCrate == null)
    //        Debug.LogError("new create item does not have crate component");

    //    newCrate.SetCrateItem(contents);

    //    NoSpawnTilesList.Add(crateObject);
    //    return true;
    //}

    //public bool CreateCrate(int x, int z,SpawnType contents)
    //{
    //    IInventoryItem itemToSpawn = ItemManager.SpawnItem(contents);

    //    if (itemToSpawn == null)
    //    {
    //        Debug.Log("can not find item type: " + contents);
    //        return false;
    //    }
    //    return CreateCrate(x, z, ItemManager.SpawnItem(contents));
    //}

    //private bool FindNearestEmptyTile(int x, int z, out Vector2 emptyTile)
    //{

    //    int xBoundHigh = x + 1;
    //    int xBoundLow = x - 1;
    //    int zBoundHigh = z + 1;
    //    int zBoundLow = z - 1;

    //    List<Vector2> closeNonEmptyTiles = new List<Vector2>();

    //    for (int i = 0; i < NoSpawnTilesList.Count; i++)
    //    {
    //        if (NoSpawnTilesList[i] == null)
    //            NoSpawnTilesList.RemoveAt(i);
    //        else
    //        {
    //            int tileX = (int)Math.Round(NoSpawnTilesList[i].position.x, 0);
    //            int tileZ = (int)Math.Round(NoSpawnTilesList[i].position.z, 0);
    //            if (tileX <= xBoundHigh && tileX >= xBoundLow)
    //            {
    //                if (tileZ <= zBoundHigh && tileZ >= zBoundLow)
    //                {
    //                    //Debug.Log(tileX + "," + tileZ);
    //                    closeNonEmptyTiles.Add(new Vector2(tileX, tileZ));
    //                }
    //            }
    //        }
    //    }

    //    if (!closeNonEmptyTiles.Contains(new Vector2(x, z)))
    //    {
    //        //Debug.Log("return "+x + "," + z);
    //        emptyTile = new Vector2(x, z);
    //        return true;
    //    }
    //    if (!closeNonEmptyTiles.Contains(new Vector2(x + 1, z)))
    //    {
    //        emptyTile = new Vector2(x + 1, z);
    //        return true;
    //    }
    //    if (!closeNonEmptyTiles.Contains(new Vector2(x - 1, z)))
    //    { 
    //        emptyTile = new Vector2(x - 1, z);
    //        return true;
    //    }
    //    if (!closeNonEmptyTiles.Contains(new Vector2(x, z+1)))
    //    {
    //        emptyTile = new Vector2(x, z + 1);
    //        return true;
    //    }
    //    if (!closeNonEmptyTiles.Contains(new Vector2(x, z -1)))
    //    { 
    //        emptyTile = new Vector2(x, z - 1);
    //        return true;
    //    }
    //    if (!closeNonEmptyTiles.Contains(new Vector2(x + 1, z + 1)))
    //    {
    //        emptyTile = new Vector2(x + 1, z + 1);
    //        return true;
    //    }
    //    if (!closeNonEmptyTiles.Contains(new Vector2(x + 1, z - 1)))
    //    {
    //        emptyTile = new Vector2(x + 1, z - 1);
    //        return true;
    //    }
    //    if (!closeNonEmptyTiles.Contains(new Vector2(x - 1, z + 1)))
    //    {
    //        emptyTile = new Vector2(x - 1, z + 1);
    //        return true;
    //    }

    //    if (!closeNonEmptyTiles.Contains(new Vector2(x - 1, z - 1)))
    //    {
    //        emptyTile = new Vector2(x - 1, z - 1);
    //        return true;
    //    }
    

    //    Debug.Log("no nearby tiles found, failed spawning item");
    //    emptyTile = new Vector2(x+3, z);
    //    return false;
    //}

    //public void MovePlayer(int x, int z)
    //{
    //    //Room startRoom = rooms.First();
    //    //Vector3 playerPos = new Vector3(startRoom.bottom_left_x + 1, 1, startRoom.bottom_left_y + 1);
    //    //player.transform.position = playerPos;

    //    //TODO find player and move him to this location
    //    GameObject player = GameObject.FindGameObjectWithTag("Player");

    //    player.transform.position = new Vector3(x,1,z);
    //}

    //public void CreateObstacle(int x, int z)
    //{
    //    //TileInfo obstacleTileInfo = roomSlots.Dequeue();
    //    //obstacleTileInfo.prefabType = PrefabType.Obstacle;
    //    Transform obstaclePrefab = GameObject.Find("DestructibleObstacle").transform;
    //    //Transform obstaclePrefab = obstacleTiles[Random.Range(0, obstacleTiles.Length - 1)];
    //    Transform newObstacle = Instantiate(obstaclePrefab, new Vector3(x, 1, z), Quaternion.identity);
    //    NoSpawnTilesList.Add(newObstacle);

    //}
    //private void CreateFloor(int x, int z)
    //{
    //    if (boardHolder == null)
    //        boardHolder = new GameObject().transform;


    //    Transform floorPrefab = GameObject.Find("Floor").transform;
    //    //Transform floorPrefab = floorTiles[Random.Range(0, floorTiles.Length - 1)];
    //    Transform floorTile = Instantiate(floorPrefab, new Vector3(x, -.25f, z), Quaternion.identity);
    //    floorTile.parent = boardHolder;
    //}

    //public void CreateWall(int x, int z)
    //{
    //    if (boardHolder == null)
    //        boardHolder = new GameObject().transform;

    //    // TileInfo wallTileInfo = roomSlots.Dequeue();
    //    //wallTileInfo.prefabType = PrefabType.Wall;
    //    //Transform wallPrefab = wallTiles[Random.Range(0, wallTiles.Length - 1)];
    //    Transform wallPrefab = GameObject.Find("Wall").transform;
    //    Transform wallTile = Instantiate(wallPrefab, new Vector3(x, 1, z), Quaternion.identity);
    //    wallTile.parent = boardHolder;


    //    NoSpawnTilesList.Add(wallTile);
    //}

    //public void CreateEnemy(int x, int z)
    //{
    //    //TileInfo enemyTileInfo = roomSlots.Dequeue();
    //    //enemyTileInfo.prefabType = PrefabType.Enemy;
    //    Transform enemyPrefab = enemies[Random.Range(0, enemies.Length - 1)];
    //    Transform enemy = Instantiate(enemyPrefab, new Vector3(x, 1, z), Quaternion.identity);

    //    int enemyHealth = 5;
    //    int enemyDamage = 1;

    //    enemy.GetComponent<Enemy>().SetUpEnemy(enemyHealth, enemyDamage, enemyColor, 1, 2);

    //}



    public void DestroyLevel()
    {
        // Clear all items in the boardHolder
        foreach (Transform child in boardHolder)
        {
            Destroy(child.gameObject);
        }
    }


    // Filter the tileMap by specific properties; returns a queue.
    // Currently, this method only filters by the tile's id but it can be improved
    // to filter by any property of the TileInfo.
    private List<TileInfo> FilterTileMap(string tileId)
    {

        Debug.Log("Called Filter Tile Map");
        var query = from TileInfo tile in tileMap
                    where tile.id == tileId
                    select tile;

        return query.ToList();
    }

    // Filter the tileMap by the PrefabType and id
    private List<TileInfo> FilterTileMap(string tileId, PrefabType tileType)
    {
        Debug.Log("Called Filter Tile Map w/ tile type");

        var query = from TileInfo tile in tileMap
                    where tile.prefabType == tileType &&
                        tile.id == tileId
                    select tile;

        return query.ToList();
    }

    Queue<T> ShuffleList<T>(List<T> itemList)
    {
        int n = itemList.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = itemList[k];
            itemList[k] = itemList[n];
            itemList[n] = value;
        }

        return new Queue<T>(itemList);
    }

    public static BoardManager instance = null;

    public static BoardManager getBoardManager()
    {
        return instance;
    }

    private void Awake()
    {
        //NoSpawnTilesList = new List<Transform>();


    }

    void Start () {

        // Ensure the instance is of the type GameManager
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        // There need to be checks here. The lines below assume the objects exist.
        boardHolder = transform.Find("BoardHolder").transform;
        groundPlane = transform.Find("Ground").transform;
        groundSurface = groundPlane.GetComponent<NavMeshSurface>();
    }



    void CreateRoomsAndCorridors(LevelPerams levelParams)
    {
        // initialize the rooms List with a random size
        rooms = new List<Room>(levelParams.numRooms.RandomInt);

        // initialize the corridors List with one less the number of rooms
        corridors = new List<Corridor>(rooms.Capacity - 1);

        // create the first room and corridor
        rooms.Insert(0, new Room());
        corridors.Insert(0, new Corridor());

        // set up the first room as there is no corridor
        rooms[0].SetupRoom("start", levelParams.columns, levelParams.rows, levelParams.roomWidth, levelParams.roomHeight);

        // set up the first corridor using the first room
        corridors[0].SetupCorridor(
            rooms[0], 
            "corridor-1", 
            levelParams.corridorLength, 
            levelParams.roomWidth, 
            levelParams.roomHeight, 
            levelParams.columns, 
            levelParams.rows, 
            true);

        for (int i = 1; i < rooms.Capacity; i++)
        {
            // Create a room.
            rooms.Insert(i, new Room());

            // Setup the room based on the previous corridor.
            rooms[i].SetupRoom(
                "", 
                levelParams.roomWidth,
                levelParams.roomHeight, 
                levelParams.columns, 
                levelParams.rows, 
                corridors[i - 1]);

            // If we haven't reached the end of the corridors array...
            if (i < corridors.Capacity)
            {
                // ... create a corridor.
                corridors.Insert(i, new Corridor());

                // Setup the corridor based on the room that was just created.
                corridors[i].SetupCorridor(
                    rooms[i], "",
                    levelParams.corridorLength,
                    levelParams.roomWidth,
                    levelParams.roomHeight,
                    levelParams.columns,
                    levelParams.rows, 
                    false);
            }
        }
    }

    void SetTileValuesForRooms()
    {
        // Go through all the rooms...
        for (int i = 0; i < rooms.Count; i++)
        {
            Room currentRoom = rooms[i];

            // ... and for each room go through it's width.
            for (int j = 0; j < currentRoom.width; j++)
            {
                int xCoord = currentRoom.bottom_left_x + j;

                // For each horizontal tile, go up vertically through the room's height.
                for (int k = 0; k < currentRoom.height; k++)
                {
                    int yCoord = currentRoom.bottom_left_y + k;

                    // Room tiles are initialized with the PrefabType None to indicate they're empty
                    tileMap[xCoord, yCoord] = new TileInfo(new Coord(xCoord, yCoord), TilePosition.Room, PrefabType.None, currentRoom.id);
                }
            }
        }
    }
    void SetTileValuesForCorridors()
    {
        // Go through every corridor...
        for (int i = 0; i < corridors.Count; i++)
        {
            Corridor currentCorridor = corridors[i];

            // and go through it's length.
            for (int j = 0; j < currentCorridor.corridorLength; j++)
            {
                // Start the coordinates at the start of the corridor.
                int xCoord = currentCorridor.startXPos;
                int yCoord = currentCorridor.startYPos;

                // Depending on the direction, add or subtract from the appropriate
                // coordinate based on how far through the length the loop is.
                switch (currentCorridor.direction)
                {
                    case Direction.North:
                        yCoord += j;
                        break;
                    case Direction.East:
                        xCoord += j;
                        break;
                    case Direction.South:
                        yCoord -= j;
                        break;
                    case Direction.West:
                        xCoord -= j;
                        break;
                }
                // Set the tile at these coordinates to Floor.
                tileMap[xCoord, yCoord] = new TileInfo(new Coord(xCoord, yCoord), TilePosition.Corridor, PrefabType.None, currentCorridor.id);
            }
        }
    }

    void LayoutFloor(LevelPerams levelParams)
    {
        for (int i = 0; i < levelParams.columns; i++)
        {
            for (int j = 0; j < levelParams.rows; j++)
            {
                Transform floorPrefab = floorTiles[Random.Range(0, floorTiles.Length - 1)];
                Transform floorTile = Instantiate(floorPrefab, new Vector3(i, -.25f, j), Quaternion.identity);
                floorTile.parent = boardHolder;
            }
        }
    }

    void LayoutRoomsAndCorridors(LevelPerams levelParams)
    {
        // iterate through TileInfo and instantiate objects
        for (int i = 0; i < levelParams.columns; i++)
        {
            for (int j = 0; j < levelParams.rows; j++)
            {
                TileInfo tile = tileMap[i, j];
                switch (tile.prefabType)
                {
                    case PrefabType.Wall:
                        Transform wallPrefab = wallTiles[Random.Range(0, wallTiles.Length - 1)];
                        Transform wallTile = Instantiate(wallPrefab, new Vector3(i, 1, j), Quaternion.identity);
                        wallTile.parent = boardHolder;
                        break;
                    case PrefabType.None:
                        break;
                    case PrefabType.Obstacle:
                        break;
                    case PrefabType.Enemy:
                        break;
                    case PrefabType.Player:
                        break;
                    default:
                        break;
                }
            }
        }
    }

    void SetPlayerPosition()
    {
        // Get the first room
        Room startRoom = rooms.First();
        Vector3 playerPos = new Vector3(startRoom.bottom_left_x + 1, 1, startRoom.bottom_left_y + 1);
        player.transform.position = playerPos;
    }

    // There is a LOT of repetition in this method which calls for
    // an extract method operation. I'm too tired to think of that though.
    // TODO: extract the object instantiation into a method.
    void LayoutRoomObjects(LevelPerams levelParams)
    {
        // Generate a random enemy color
        enemyColor = Color.Lerp(Color.yellow, Color.red, Mathf.PingPong(Time.time, 1));
        float enemyDamage = Mathf.Pow(levelParams.difficulty, 2) / (2 * levelParams.difficulty);
        float enemyHealth = Mathf.Pow(levelParams.difficulty, 2) / (2 * levelParams.difficulty);

        // set up the enemies and other objects from the second room onwards.
        // The Player will be placed in the first room.
        for (int i = 1; i < rooms.Count; i++)
        {
            Room selectedRoom = rooms[i];
            Queue<TileInfo> roomSlots = ShuffleList(FilterTileMap(selectedRoom.id));

            int wallCount = levelParams.innerWallsPerRoom.RandomInt;
            for (int j = 0; j < wallCount; j++)
            {
                TileInfo wallTileInfo = roomSlots.Dequeue();
                wallTileInfo.prefabType = PrefabType.Wall;
                Transform wallPrefab = wallTiles[Random.Range(0, wallTiles.Length - 1)];
                Transform wallTile = Instantiate(wallPrefab, new Vector3(wallTileInfo.pos.x, 1, wallTileInfo.pos.y), Quaternion.identity);
                wallTile.parent = boardHolder;
            }

            int enemyCount = levelParams.enemiesPerRoom.RandomInt;
            for (int k = 0; k < enemyCount; k++)
            {
                TileInfo enemyTileInfo = roomSlots.Dequeue();
                enemyTileInfo.prefabType = PrefabType.Enemy;
                Transform enemyPrefab = enemies[Random.Range(0, enemies.Length - 1)];
                Transform enemy = Instantiate(enemyPrefab, new Vector3(enemyTileInfo.pos.x, 1, enemyTileInfo.pos.y), Quaternion.identity);

                enemy.GetComponent<Enemy>().SetUpEnemy(enemyHealth, enemyDamage, enemyColor, 1, 2);
            }

            int obstacleCount = levelParams.obstaclesPerRoom.RandomInt;
            for (int m = 0; m < obstacleCount; m++)
            {
                TileInfo obstacleTileInfo = roomSlots.Dequeue();
                obstacleTileInfo.prefabType = PrefabType.Obstacle;
                Transform obstaclePrefab = obstacleTiles[Random.Range(0, obstacleTiles.Length - 1)];
                Instantiate(obstaclePrefab, new Vector3(obstacleTileInfo.pos.x, 1, obstacleTileInfo.pos.y), Quaternion.identity);
            }
        }
    }

    // Every level has a floor boss.
    void SetFloorBoss(int level)
    {
        float bossDamage = Mathf.Pow(level, 2) / (2 * level) + 1;
        float bossHealth = Mathf.Pow(level, 2) / (2 * level) + 2;

        Room lastRoom = rooms.Last();
        Queue<TileInfo> emptyRoomSlots = ShuffleList(FilterTileMap(lastRoom.id, PrefabType.None));
        TileInfo bossTileInfo = emptyRoomSlots.Dequeue();
        bossTileInfo.prefabType = PrefabType.Boss;

        Transform boss = Instantiate(floorBoss, new Vector3(bossTileInfo.pos.x, 1, bossTileInfo.pos.y), Quaternion.identity);
        Color bossColor = boss.GetComponent<Renderer>().material.color;

        boss.GetComponent<Enemy>().SetUpEnemy(bossHealth, bossDamage, bossColor, 2, 5);
    }
}
