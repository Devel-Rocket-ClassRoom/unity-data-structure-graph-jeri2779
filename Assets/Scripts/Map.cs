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
    public int row = 0;
    public int col = 0;
    public Tile[] tiles;
    public void Init(int rows, int cols)
    {
        row = rows;
        col = cols;
        tiles = new Tile[row * col];
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = new Tile();
            tiles[i].id = i;
        }

        for (int r = 0; r < rows; ++r)
        {
            for (int c = 0; c < cols; ++c)
            {
                int index = r * col + c;
                var adjacents = tiles[index].adjacents;
                if (r - 1 >= 0)
                {

                    adjacents[(int)Sides.Top] = tiles[index - cols];
                }

                if (c + 1 < col)
                {
                    adjacents[(int)Sides.Right] = tiles[index + 1];
                }
                if (c - 1 >= 0)
                {
                    adjacents[(int)Sides.Left] = tiles[index - 1];
                }
                if (r + 1 < row)
                {
                    adjacents[(int)Sides.Bottom] = tiles[index + col];
                }
            }
        }
        for(int i = 0; i<tiles.Length; i++)
        {
            tiles[i].UpdateAutoTileId();
        }
    }

}
