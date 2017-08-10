using System;

[Serializable]
public struct TileInfo
{
    // The position of the tile on the map's grid.
    public Coord pos;
    // The TilePosition determines if the tile is a wall or if it's in
    // a room or corridor.
    public BoardManager.TilePosition position;
    // ID of the room or corridor
    public string id;

    // The type of initial prefab placed on this tile (on the Floor tile).
    public BoardManager.PrefabType prefabType;

    public TileInfo(Coord mapPos, BoardManager.TilePosition tilePos, BoardManager.PrefabType tileType, string posId)
    {
        pos = mapPos;
        position = tilePos;
        id = posId;

        prefabType = tileType;
    }


    public TileInfo(Coord mapPos, BoardManager.PrefabType tileType, string posId)
    {
        pos = mapPos;
        position = BoardManager.TilePosition.Room;
        id = posId;

        prefabType = tileType;
    }
}
