using UnityEngine;

//4방향 enum

//0000
//0001
//0010
//0011

//1000 = 8

public enum Sides
{
    Bottom, //3
    Right,  //2
    Left,   //1
    Top     //0
}
public class Tile 
{
    public int id;
    public Tile[] adjacents = new Tile[4];
    public int autoTileId;
    public bool isVisited = false;
    public void UpdateAutoTileId()
    {
       autoTileId = 0;
        for(int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] != null)
            {
                // ex: 1 << 3 = 8
                autoTileId |= (1 << (adjacents.Length - 1 - i));

            }

        }
    }
}
