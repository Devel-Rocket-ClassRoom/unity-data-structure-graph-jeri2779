using UnityEngine;

//4방향 enum

//0000
//0001
//0010
//0011

//1000 = 8

public enum Sides
{

    None = -1,
    Top,    // 0 : 0001
    Left,   // 1 : 0010
    Right,  // 2 : 0100
    Bottom,  // 3 : 1000

}
public class Tile
{
    public int id;
    public Tile[] adjacents = new Tile[4];
    public int autoTileId;
    public int fowAutoTileId;
    public bool isVisited = false;
    public bool CanMove => autoTileId != (int)TileTypes.Empty;

    public void UpdateAutoTileId()
    {
        autoTileId = 0;
        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] != null)
            {
                autoTileId |= (1 << i);
                // i=0 (Top)    → 1 << 0 = 1 : 0001
                // i=1 (Left)   → 1 << 1 = 2 : 0010  
                // i=2 (Right)  → 1 << 2 = 4 : 0100
                // i=3 (Bottom) → 1 << 3 = 8 : 1000
            }
        }
    }
 
    //fow타일 업데이트는 인접 타일이 방문된 경우에만 고려
    //인접 타일이 null이 아니고 방문된 경우에만 autoTileId 업데이트
    //인접 타일이 null이거나 방문되지 않은 경우에는 autoTileId 업데이트하지 않음
    //이렇게 하면 시야 타일이 주변 타일의 상태에 따라 올바르게 업데이트되고, 방문되지 않은 타일은 시야 타일로 간주되지 않도록 할 수 있음
    //인접 타일이 null이거나 방문되지 않은 경우에는 시야 타일로 간주되지 않도록 함
   


    public void RemoveAdjacent(Tile tile)//특정 방향의 인접 타일 제거
    {

        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] == null) //이미 제거된 인접 타일은 건너뛰기
            {
                continue;
            }
            if (adjacents[i].id == tile.id)//인접 타일이 일치하는 경우
            {
                adjacents[i] = null;
                UpdateAutoTileId();
                break;
            }
        }

    }
    public void ClearAdjacent()//모든 인접 타일 제거
    {
        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] == null) //이미 제거된 인접 타일은 건너뛰기
            {
                continue;
            }
            adjacents[i].RemoveAdjacent(this);//인접 타일에서 현재 타일 제거
            adjacents[i] = null;
        }
        UpdateAutoTileId();
    }

}
 