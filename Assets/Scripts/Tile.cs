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
    Top,    // 3 : 1000
    Left,   // 2 : 0100
    Right,  // 1 : 0010
    Bottom, // 0 : 0001
}
public class Tile
{
    public int id;
    public Tile[] adjacents = new Tile[4];
    public int autoTileId;
    public int fowAutoTileId = 15;
    public bool isVisited = false;
    public bool CanMove => autoTileId != (int)TileTypes.Empty;

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
    public void UpdateFowTileId()
    {
        fowAutoTileId = 0;
        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] == null || !adjacents[i].isVisited)
            {
                fowAutoTileId |= 1 << i;
            }
        }
    }
    public void VisitedTiles()
    {
        if (isVisited) return;
        isVisited = true;

        //UpdateFowTileId();

        //for (int i = 0; i < adjacents.Length; i++)
        //{
        //    if (adjacents[i] != null)
        //    {
        //        adjacents[i].UpdateFowTileId();

        //    }
        //}
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
 