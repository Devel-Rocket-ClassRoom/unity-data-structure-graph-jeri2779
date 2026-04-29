using UnityEngine;

//4방향 enum
 

public enum Sides
{
    None = -1,
    Top,     
    Left,    
    Right,   
    Bottom, 
}

public class Tile
{
    public int id;
    public Tile[] adjacents = new Tile[4];
    public int autoTileId;
    public int fowTileId = 15;
    public bool isVisited = false;
    public bool CanMove => autoTileId != (int)TileTypes.Empty;
    //public bool CanMove => Weight != int.MaxValue;

    //public static readonly int[] tableWeights =
    //{
    //    1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
    //    2,4,int.MaxValue,1,1,1,
    //};


    //public int Weight => tableWeights[autoTileId];
    public Tile previousTile = null;

    public void ClearPrevousTile()
    {
        previousTile = null;
    }   


    public void UpdateAutoTileId()
    {
        if (autoTileId == -1) return;

        autoTileId = 0;
        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] != null && adjacents[i].autoTileId != -1)
            {
                autoTileId |= 1 << i;
            }
        }
    }


    public void UpdateFowTileId(Map map)
    {
        fowTileId = 0;

        //var row = id / map.cols;
        //var col = id % map.cols;
        //var topRow = row - 1;
        //var leftCol = col - 1;
        //var rightCol = col + 1;
        //var bottomRow = row + 1;

        //// Top
        //if (!IsVisitedTiles(map, topRow, col))
        //{
        //    fowTileId |= 1 << (int)Sides.Top;
        //}
        //// Left
        //if (!IsVisitedTiles(map, row, leftCol))
        //{
        //    fowTileId |= 1 << (int)Sides.Left;
        //}

        //// Right
        //if (!IsVisitedTiles(map, row, rightCol))
        //{
        //    fowTileId |= 1 << (int)Sides.Right;
        //}
        //// Bottom
        //if (!IsVisitedTiles(map, bottomRow, col))
        //{
        //    fowTileId |= 1 << (int)Sides.Bottom;
        //}

        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] == null || !adjacents[i].isVisited)
            {
                fowTileId |= 1 << i;
            }
        }
    }
    private bool IsVisitedTiles(Map map, int row, int col)
    {
        if (row < 0 || row >= map.rows || col < 0 || col >= map.cols)
            return false;
        return map.tiles[row * map.cols + col].isVisited;
    }

    public void VisitedCheck()
    {
        if (isVisited) return;
        isVisited = true;
    }

    public void RemoveAdjacent(Tile tile)
    {
        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] == null)
            {
                continue;
            }

            if (adjacents[i].id == tile.id)
            {
                adjacents[i] = null;
                UpdateAutoTileId();
                break;
            }
        }
    }

    public void ClearAdjacent()
    {
        autoTileId = 0;
        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] == null)
            {
                continue;
            }

            adjacents[i].RemoveAdjacent(this);
            adjacents[i] = null;
        }
        UpdateAutoTileId();
    }

}