
using System.Linq;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Animator animator;
    private Map map;
    private Stage stage;
    private int currentTileId;

    //private Tile currentTile;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.speed = 1f;
        var findGo = GameObject.FindWithTag("Map");
        stage = findGo.GetComponent<Stage>();
        map = stage.Map;
    }

    private void Update()
    {
        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");
        var direction = Sides.None;

        if (Input.GetKeyDown(KeyCode.W))
        {
            direction = Sides.Top;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            direction = Sides.Bottom;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            direction = Sides.Right;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            direction = Sides.Left;
        }
        if (direction != Sides.None)
        {
            var targetTile = map.tiles[currentTileId].adjacents[(int)direction];
            if (targetTile != null && targetTile.CanMove)
            {
                MoveTo(targetTile);
            }
        }
    }


    public void MoveTo(Tile tileId)
    {
        currentTileId = tileId.id;
        transform.position = stage.GetTilePos(currentTileId);
    }


}

 