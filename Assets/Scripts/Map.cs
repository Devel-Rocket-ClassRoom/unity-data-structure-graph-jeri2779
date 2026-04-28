 
using NUnit.Framework;
using System.Linq;
using UnityEngine;

public enum TileTypes
{
    //이진법으로 표현된 타일 타입
    Empty = -1,
    //0~14 = 해안선.
    Grass = 15,
    Tree,
    Hills,
    Mountains,
    Towns,
    Castle,
    Monster,
}
public class Map
{
    public int rows = 0;
    public int cols = 0;

    public Tile[] tiles;

    public Tile[] CoastTiles => tiles.Where(t => t.autoTileId >= 0 && t.autoTileId < (int)TileTypes.Grass).ToArray();
    public Tile[] LandTiles => tiles.Where(t => t.autoTileId == (int)TileTypes.Grass).ToArray();

    public Tile startTileId;
    public Tile endTileId;

    public void Init(int rows, int cols)
    {
        this.rows = rows;
        this.cols = cols;

        tiles = new Tile[rows * cols];
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = new Tile();
            tiles[i].id = i;

        }
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                int index = r * cols + c;
                var adjacents = tiles[index].adjacents;

                if ((r - 1) >= 0)
                {
                    adjacents[(int)Sides.Top] = tiles[index - cols];
                }
                if ((c + 1) < cols)
                {
                    adjacents[(int)Sides.Right] = tiles[index + 1];
                }
                if ((c - 1) >= 0)
                {
                    adjacents[(int)Sides.Left] = tiles[index - 1];

                }
                if ((r + 1) < rows)
                {
                    adjacents[(int)Sides.Bottom] = tiles[index + cols];
                }
            }
        }
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].UpdateAutoTileId();
        }
    }

    public void ShuffleTiles(Tile[] tiles)
    {
        for (int i = tiles.Length - 1; i >= 0; --i)
        {
            int rand = Random.Range(0, i + 1);
            (tiles[rand], tiles[i]) = (tiles[i], tiles[rand]);
        }
    }
    public void DecorateTile(Tile[] tiles, float percent, TileTypes tileType)
    {
        ShuffleTiles(tiles);
        int total = Mathf.FloorToInt(tiles.Length * percent);//타일 배열에서 percent만큼의 타일을 선택하여 tileType으로 설정
        for (int i = 0; i < total; i++)
        {
            if (tileType == TileTypes.Empty)
            {
                tiles[i].ClearAdjacent();//타일 타입이 Empty인 경우 인접 타일 정보를 초기화
            }
            tiles[i].autoTileId = (int)tileType;//타일 타입을 설정
        }
    }

    public bool CreateIsland(
        float erodePercent,
        int erodeIterations,
        float lakePercent,
        float treePercent,
        float hillPercent,
        float mountainPercent,
        float townPercent,
        float monsterPercent
        )
    {

        for (int i = 0; i < erodeIterations; i++)
        {
            DecorateTile(CoastTiles, erodePercent, TileTypes.Empty);

        }
        //var castleTile = tiles.FirstOrDefault(t => t.autoTileId == (int)TileTypes.Castle);

        DecorateTile(LandTiles, townPercent, TileTypes.Towns);          //마을 생성
        DecorateTile(LandTiles, lakePercent, TileTypes.Empty);          //호수 생성
        DecorateTile(LandTiles, treePercent, TileTypes.Tree);           //나무 생성
        DecorateTile(LandTiles, hillPercent, TileTypes.Hills);          //언덕 생성
        DecorateTile(LandTiles, mountainPercent, TileTypes.Mountains);  //산 생성
        DecorateTile(LandTiles, monsterPercent, TileTypes.Monster);     //몬스터 생성

        var towns = tiles.Where(t => t.autoTileId == (int)TileTypes.Towns).ToArray();
        ShuffleTiles(towns);
        startTileId = towns[0];
        var castleTile = towns[1];
        castleTile.autoTileId = (int)TileTypes.Castle;
        return true;
    }
}

 