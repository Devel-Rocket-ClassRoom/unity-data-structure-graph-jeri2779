using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Animator animator;
    private Map map;
    private Stage stage;
    public int currentTileId;

    public float moveSpeed = 4f;  
    private bool isMoving = false;

    private Graph graph;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.speed = 0f;
        var findGo = GameObject.FindWithTag("Map");
        stage = findGo.GetComponent<Stage>();
        map = stage.Map;
        graph = stage.Graph;
    }

    private void Update()
    {
        if (isMoving)
        {
            return;
        }

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

    private IEnumerator MoveRoutine(Vector3 start, Vector3 end)
    {
        animator.speed = 1f;
        float duration = 1f / moveSpeed; 
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(start, end, elapsed / duration);
            yield return null;
        }

        transform.position = end;
        animator.speed = 0f;
        isMoving = false;
        stage.DrawFow(currentTileId);
    }

   
    public void Spawn(Tile tile)
    {
        currentTileId = tile.id;
        transform.position = stage.GetTilePos(currentTileId);
        stage.DrawFow(currentTileId);
    }

   
    public void MoveTo(Tile tile)
    {
        currentTileId = tile.id;
        var targetPos = stage.GetTilePos(currentTileId);
        isMoving = true;
        StartCoroutine(MoveRoutine(transform.position, targetPos));
    }

    //길찾기 알고리즘을 이용하여 플레이어가 이동할 수 있는 타일까지의 경로를 계산하는 메서드
    public void SearchMove(int targetTileId)
    {
        
        var tile = map.tiles[targetTileId];
        if (tile == null || !tile.CanMove) return;
   

        var search = new GraphSearch();
        search.Init(graph);
    }

}