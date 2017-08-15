using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class PathfindingManager {

    static PathfindingManager instance;

    GridGraph[,] mapNodeAStarGraphs = new GridGraph[3,3];
    Coord PlayerMapNode;

    Queue<GridGraph> GridGraphsToScanQueue;

    AstarPath aStarPath;

    public bool ScanNextQueue()
    {
        if (GridGraphsToScanQueue.Count == 0)
        {
            return false;
        }
        AstarPath.active.Scan(GridGraphsToScanQueue.Dequeue());
        return true;
    }

    public static PathfindingManager getPathfindingManager()
    {
        if (instance == null)
        {
            GridGraph[,] mapNodeAStarGraphs = new GridGraph[3, 3];
            instance = new PathfindingManager();
        }
        return instance;
    }

    private PathfindingManager()
    { 
        GridGraphsToScanQueue = new Queue<GridGraph>();
    }
    
    public void Init(Coord MapNode)
    {
        Debug.Log("OnInit");
        PlayerMapNode = MapNode; 
        aStarPath = GameObject.Find("_A*").GetComponent<AstarPath>();
        if (aStarPath.graphs.Length != 9)
        {
            Debug.Log("num of graphs need to be 9");

        }



        for (int i = 0; i < 9; i++)
        {
            int CurX = i / 3;
            int CurZ = i % 3;

            mapNodeAStarGraphs[CurX, CurZ] = aStarPath.graphs[i] as GridGraph;

            int CurSectorX = MapNode.x + CurX - 1;
            int CurSectorZ = MapNode.y + CurZ - 1;

            mapNodeAStarGraphs[CurX, CurZ].center = new Vector3(40* CurSectorX + 19.5f, - 0.1f, 40 * CurSectorZ + 19.5f);
            AstarPath.active.Scan(mapNodeAStarGraphs[i / 3, i % 3]);

        }

    }

    public void MovePlayerMapNodeUp()
    {
        PlayerMapNode = new Coord(PlayerMapNode.x, PlayerMapNode.y + 1);

        for (int i = 0; i < 3; i++)
        {
            GridGraph temp = mapNodeAStarGraphs[i, 0];
            mapNodeAStarGraphs[i, 0] = mapNodeAStarGraphs[i, 1];
            mapNodeAStarGraphs[i, 1] = mapNodeAStarGraphs[i, 2];
            mapNodeAStarGraphs[i, 2] = temp;

            mapNodeAStarGraphs[i, 2].center = new Vector3(40 * (PlayerMapNode.x + i - 1) + 19.5f, -0.1f, 40 * (PlayerMapNode.y + 1) + 19.5f);
            GridGraphsToScanQueue.Enqueue(mapNodeAStarGraphs[i, 2]);
            //AstarPath.active.Scan(mapNodeAStarGraphs[i, 2]);
            
        }
    }

    public void MovePlayerMapNodeDown()
    {
        PlayerMapNode = new Coord(PlayerMapNode.x, PlayerMapNode.y - 1);

        for (int i = 0; i<3; i++)
        {
            GridGraph temp = mapNodeAStarGraphs[i, 2];
            mapNodeAStarGraphs[i, 2] = mapNodeAStarGraphs[i, 1];
            mapNodeAStarGraphs[i, 1] = mapNodeAStarGraphs[i, 0];
            mapNodeAStarGraphs[i, 0] = temp;

            mapNodeAStarGraphs[i, 0].center = new Vector3(40 * (PlayerMapNode.x + i -1) + 19.5f, -0.1f, 40 * (PlayerMapNode.y -1) + 19.5f);
            GridGraphsToScanQueue.Enqueue(mapNodeAStarGraphs[i, 0]);
            //AstarPath.active.Scan(mapNodeAStarGraphs[i, 0]);
        }

    }

    public void MovePlayerMapNodeRight()
    {
        PlayerMapNode = new Coord(PlayerMapNode.x +1, PlayerMapNode.y);

        for (int i = 0; i < 3; i++)
        {
            GridGraph temp = mapNodeAStarGraphs[0, i];
            mapNodeAStarGraphs[0, i] = mapNodeAStarGraphs[1, i];
            mapNodeAStarGraphs[1, i] = mapNodeAStarGraphs[2, i];
            mapNodeAStarGraphs[2, i] = temp;

            mapNodeAStarGraphs[2, i].center = new Vector3(40 * (PlayerMapNode.x + 1) + 19.5f, -0.1f, 40 * (PlayerMapNode.y + i - 1) + 19.5f);
            GridGraphsToScanQueue.Enqueue(mapNodeAStarGraphs[2, i]);
            //AstarPath.active.Scan(mapNodeAStarGraphs[2, i]);
        }
    }

    public void MovePlayerMapNodeLeft()
    {
        PlayerMapNode = new Coord(PlayerMapNode.x -1, PlayerMapNode.y);

        for (int i = 0; i < 3; i++)
        {
            GridGraph temp = mapNodeAStarGraphs[2, i];
            mapNodeAStarGraphs[2, i] = mapNodeAStarGraphs[1, i];
            mapNodeAStarGraphs[1, i] = mapNodeAStarGraphs[0, i];
            mapNodeAStarGraphs[0, i] = temp;

            mapNodeAStarGraphs[0, i].center = new Vector3(40 * (PlayerMapNode.x - 1) + 19.5f, -0.1f, 40 * (PlayerMapNode.y + i - 1) + 19.5f);
            GridGraphsToScanQueue.Enqueue(mapNodeAStarGraphs[0, i]);
            //AstarPath.active.Scan(mapNodeAStarGraphs[0, i]); 
        }
    }


}
