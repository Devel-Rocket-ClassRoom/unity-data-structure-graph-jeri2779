
using System.Linq;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Animator animator;
    private Map map;
    private Stage stage;
    private int currentTileId;

    public float moveSpeed = 64f;
    private bool isMoving = false;
    private Vector3 targetPos;

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
        if (isMoving)
        {
            animator.speed = 1f;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            if (transform.position == targetPos)
            {
                isMoving = false;
                stage.UpdateFow(currentTileId);
            }
            return;
        }

        animator.speed = 0f;  

        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");
        var direction = Sides.None;

        if (Input.GetKey(KeyCode.W))
        {
            direction = Sides.Top;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            direction = Sides.Bottom;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            direction = Sides.Right;
        }
        else if (Input.GetKey(KeyCode.A))
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
        //초기 위치는 towntileid에 해당하는 타일로 설정
        currentTileId = tileId.id;
        targetPos = stage.GetTilePos(currentTileId);
         
        isMoving = true;
        stage.UpdateFow(currentTileId);
    }


}

 