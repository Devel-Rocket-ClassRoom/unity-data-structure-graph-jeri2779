using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public GameObject tilePrefab;
    private GameObject[] tileObjs;
    private SpriteRenderer[] tileRenderers;

    public int mapWidth = 20;
    public int mapHeight = 20;
    public int viewRange = 3;

    [Range(0f, 0.9f)]
    public float erodePercent = 0.5f;
    public int erodeIterations = 2;
    public float lakePercent = 0.1f;
    public float treePercent = 0.1f;
    public float hillPercent = 0.1f;
    public float mountainPercent = 0.1f;
    public float townPercent = 0.05f;
    public float monsterPercent = 0.05f;
    //castle은 1개만 생성되도록 설정



    public Vector2 tileSize = new Vector2(16, 16);
    public Sprite[] islandSprites;
    public Sprite[] fowSprites;

    public PlayerMovement playerPrefab;
    private PlayerMovement player;

    private Graph graph;
    public Graph Graph => graph;

    private List<GraphNode> currentPath = null;

    private Vector3 FirstTilePos
    {
        get
        {
            var pos = transform.position;
            pos.x -= (mapWidth * tileSize.x) * 0.5f;
            pos.y += (mapHeight * tileSize.y) * 0.5f;
            //pos.x += tileSize.x * 0.5f;
            //pos.y -= tileSize.y * 0.5f;
            return pos;
        }
    }
    private Map map;
    private int prevTileId = -1;

    public Map Map => map;

    private void Start()
    {
        ResetStage();
    }
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
             
            var tileId = ScreenPosToTileId(Input.mousePosition);
            if (map.tiles[tileId].isVisited)
            {
                player.SearchMove(tileId);
            }
            else
            {
                Debug.Log("미발견 구역.");
                return;
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetStage();
        }
        //if (Input.GetMouseButtonDown(0))
        //{
        //    var tileId = ScreenPosToTileId(Input.mousePosition);
        //    Debug.Log($"Tile ID: {tileId}");
        //}


        if (Input.GetMouseButtonDown(1))
        {
            var tilePos = GetTilePos(ScreenPosToTileId(Input.mousePosition));
            Debug.Log($"Tile Position: {tilePos}");
        }
        //if (tileObjs != null)
        //{
        //    int currentTileId = ScreenPosToTileId(Input.mousePosition);

        //    if (prevTileId != currentTileId)
        //    {
        //        tileObjs[currentTileId].GetComponent<SpriteRenderer>().color = Color.pink;
        //        if (prevTileId >= 0 && prevTileId < tileObjs.Length)
        //        {
        //            tileObjs[prevTileId].GetComponent<SpriteRenderer>().color = Color.white;
        //        }
        //        prevTileId = currentTileId;
        //    }

        //}
    }

    private void ResetStage()
    {
        do
        {
            map = new Map();
            map.Init(mapHeight, mapWidth);

        }
        while (!map.CreateIsland(
                         erodePercent,
                         erodeIterations,
                         lakePercent,
                         treePercent,
                         hillPercent,
                         mountainPercent,
                         townPercent,
                         monsterPercent
                        ));
       
        graph = new Graph();
        graph.Init(map.ToGrid());
        CreateGrid();
        //DrawPath(map.AStar(map.startTileId, map.endTileId));
        CreatePlayer();
    }

    private void DrawPath(List<Tile> path)
    {
        foreach(var tile in tileObjs)
        {
            tile.GetComponent<SpriteRenderer>().color = Color.white;
        }
        for(int i = 0; i < path.Count; i++)
        {
            float t = i / (float)(path.Count-1);
            tileObjs[path[i].id].GetComponent<SpriteRenderer>().color = 
                Color.Lerp(Color.white, Color.green, t);

        }
    }


    private void CreateGrid()
    {
        if (tileObjs != null)
        {
            foreach (var tile in tileObjs)
            {
                Destroy(tile.gameObject);
            }
        }
        tileObjs = new GameObject[mapWidth * mapHeight];

        var StartX = -(mapWidth * tileSize.x) / 2f;
        var StartY = (mapHeight * tileSize.y) / 2f;
        var position = new Vector3(StartX, StartY, 0);

        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                var tileId = i * mapWidth + j;
                var newGo = Instantiate(tilePrefab, transform);
                newGo.transform.position = position;
               
                tileObjs[tileId] = newGo;
                DecorateTile(tileId);
                position.x += tileSize.x;
            }
            position.x = StartX;
            position.y -= tileSize.y;
        }
    }
    public void DecorateTile(int tileId)
    {
        var tile = map.tiles[tileId];
        var tileGo = tileObjs[tileId];
        var ren = tileGo.GetComponent<SpriteRenderer>();
        if (tile.isVisited)
        {
            
            if (tile.autoTileId != (int)TileTypes.Empty)
            {
                ren.sprite = islandSprites[tile.autoTileId];
            }
            else
            {
                ren.sprite = null;
            }
        }
        else
        {
            if (tile.fowTileId > 0 && tile.fowTileId < fowSprites.Length)
            {
                ren.sprite = fowSprites[tile.fowTileId];
            }
            else
            {
                ren.sprite = fowSprites[15];
            }
        }
    }
    public int visitRadius = 1;
    public void OnTileVisited(int tileId)
    {
        if (tileId < 0 || tileId >= map.tiles.Length) return;
        OnTileVisited(map.tiles[tileId]);
    }
    public void OnTileVisited(Tile tile)
    {
        if (tile == null) return;
        int centerX = tile.id % mapWidth;
        int centerY = tile.id / mapWidth;

        for(int i = -visitRadius; i <= visitRadius; i++)
        {
            for (int j = -visitRadius; j <= visitRadius; j++)
            {
                int tgtX = centerX + j;
                int tgtY = centerY + i;
                if (tgtX < 0 || tgtX >= mapWidth || tgtY < 0 || tgtY >= mapHeight)
                {
                    continue;
                }
                int targetId = tgtY * mapWidth + tgtX;
                map.tiles[targetId].isVisited = true;
                DecorateTile(targetId);
            }
        }
        var radius = visitRadius + 1;
        for (int i = -radius; i <= radius; i++)
        {
            for (int j = -radius; j <= radius; j++)
            {
                if(i == radius || i == -radius || j == radius || j == -radius)
                {
                    // 테두리만 업데이트
                    int tgtX = centerX + j;
                    int tgtY = centerY + i;
                    if (tgtX < 0 || tgtX >= mapWidth || tgtY < 0 || tgtY >= mapHeight)
                    {
                        continue;
                    }
                    int targetId = tgtY * mapWidth + tgtX;
                    map.tiles[targetId].UpdateFowTileId(map);
                    DecorateTile(targetId);
                }
            }
        }


    }

    public void DrawFow(int tileId)
    {
        int row = tileId / mapWidth;
        int col = tileId % mapWidth;

        for (int i = -viewRange; i <= viewRange; i++)
        {
            for (int j = -viewRange; j <= viewRange; j++)
            {
                int tgtRow = row + i;
                int tgtCol = col + j;
                if (tgtRow < 0 || tgtRow >= mapHeight || tgtCol < 0 || tgtCol >= mapWidth)
                    continue;
                int targetId = tgtRow * mapWidth + tgtCol;
                map.tiles[targetId].VisitedCheck();
                //DecorateTile(targetId);
            }
        }

        
        for (int i = -(viewRange + 1); i <= (viewRange + 1); i++)
        {
            for (int j = -(viewRange + 1); j <= (viewRange + 1); j++)
            {
                int tgtRow = row + i;
                int tgtCol = col + j;
                if (tgtRow < 0 || tgtRow >= mapHeight || tgtCol < 0 || tgtCol >= mapWidth)
                    continue;
                int targetId = tgtRow * mapWidth + tgtCol;
                map.tiles[targetId].UpdateFowTileId(map);
                DecorateTile(targetId);//
            }
        }

        //for (int i = 0; i < tileObjs.Length; i++)
        //{
        //    DecorateTile(i);
        //}
    }

    private void CreatePlayer()
    {
        if (player != null)
        {
            Destroy(player.gameObject);
        }
        player = Instantiate(playerPrefab);
        player.Warp(map.startTileId.id);
        //player.transform.position = GetTilePos(map.startTileId.id);

        //player.MoveTo(map.startTileId);  
    }

    // 1. stage 게임 오브젝트의 포지션이 그리드의 중점이 되도록 수정
    // 2. 아래 4개 메소드 구현


    public void ShowPath(List<GraphNode> path)
    {
        if (path == null || path.Count <= 1) return;

        
        if(currentPath != null && currentPath.Count > 0)
        {
            ClearPath(currentPath);
        }

        
        currentPath = path;

        for (int i = 0; i < path.Count; i++)
        {
            var color = Color.Lerp(Color.green, Color.red, (float)i / (path.Count));
            tileObjs[path[i].id].GetComponent<SpriteRenderer>().color = color;
        }
    }


    public void ClearPath(List<GraphNode> path)
    {
        if(path == null) return;

        foreach (var node in path)
        {
            if(node.id >= 0 && node.id < tileObjs.Length && tileObjs[node.id] != null)
            {
                var ren = tileObjs[node.id].GetComponent<SpriteRenderer>();
                if(ren != null)
                {
                    ren.color = Color.white;
                }
            }
        }
    }

    public void ClearCurrentPath()
    {
        if(currentPath != null && currentPath.Count > 0)
        {
            ClearPath(currentPath);
            currentPath = null;
        }
    }

    public int ScreenPosToTileId(Vector3 screenPos)
    {
        Camera cam = Camera.main;
        //screenPos.z = Mathf.Abs(transform.position.z);
        Vector3 worldPos = cam.ScreenToWorldPoint(screenPos);
        worldPos.z = 0; // z축은 0으로 고정

        return WorldPosToTileId(worldPos);


    }

    public int WorldPosToTileId(Vector3 worldPos)
    {
        var first = FirstTilePos;

        var xTile = Mathf.FloorToInt((worldPos.x - first.x) / tileSize.x + 0.5f);
        var yTile = Mathf.FloorToInt((first.y - worldPos.y) / tileSize.y + 0.5f);
        xTile = Mathf.Clamp(xTile, 0, mapWidth - 1);
        yTile = Mathf.Clamp(yTile, 0, mapHeight - 1);
        return yTile * mapWidth + xTile;
    }

    //public Vector3 GetTilePos(int x,  int y)
    //{
    //    var worldX = -(mapWidth * tileSize.x) / 2f + (x * tileSize.x);
    //    var worldY = (mapHeight * tileSize.y) / 2f - (y * tileSize.y);
    //    return new Vector3(worldX, worldY, 0);
    //}

    public Vector3 GetTilePos(int y, int x)
     => FirstTilePos + new Vector3(x * tileSize.x, -y * tileSize.y, 0);

    public Vector3 GetTilePos(int tileId)
    {
        var x = tileId % mapWidth;
        var y = tileId / mapWidth;
        return GetTilePos(y, x);
    }


}