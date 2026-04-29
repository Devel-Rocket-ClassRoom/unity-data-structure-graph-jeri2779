using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Animator animator;
    private Map map;
    private Stage stage;
    public int currentTileId;
    public int targetTileId = -1;

    public float moveSpeed = 64f;  
    private bool isMoving = false;
    private Coroutine coMove = null;     
    private Coroutine coMovePath = null; 

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
        stage.OnTileVisited(currentTileId);
    }
   
    public void Spawn(Tile tile)
    {
        currentTileId = tile.id;
        transform.position = stage.GetTilePos(currentTileId);
        stage.OnTileVisited(currentTileId);
    }
    public void Warp(int tileId)
    {
        
        if(coMove != null)
        {
            StopCoroutine(coMove);
            coMove = null;
        }
        isMoving = false;
        targetTileId = -1;

        animator.speed = 0f;
        currentTileId = tileId;
        transform.position = stage.GetTilePos(currentTileId);
        stage.OnTileVisited(currentTileId);

    }
   
    public void MoveTo(Tile tile)
    {
        if(isMoving) return;
        targetTileId = tile.id;
        //var targetPos = stage.GetTilePos(currentTileId);
        //isMoving = true;
        //StartCoroutine(MoveRoutine(transform.position, targetPos));

        if (coMove != null)
        {
             StopCoroutine(coMove);
        }   
        coMove = StartCoroutine(CoMove());
    }

    private IEnumerator CoMove()
    {
        isMoving = true;
        animator.speed = 1f;
        //int path = stage.Map.AStar(currentTileId, targetTileId);
        var pathIndex = -1;
       
        var startPos = transform.position; 
        var endPos = stage.GetTilePos(targetTileId);
     
        var duration = Vector3.Distance(startPos, endPos) / moveSpeed;
    
        var t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;

        }
        currentTileId = targetTileId;
        transform.position = endPos;
        stage.OnTileVisited(currentTileId);
        isMoving = false;
        animator.speed = 0f;
        coMove = null;
         yield return null;
    }

    //길찾기 알고리즘을 이용하여 플레이어가 이동할 수 있는 타일까지의 경로를 계산하는 메서드
    public void SearchMove(int targetTileId)
    {

        var tile = map.tiles[targetTileId];
        if (tile == null || !tile.CanMove) return;


        if(coMove != null)
        {
            StopCoroutine(coMove);
            coMove = null;
            isMoving = false;
            currentTileId = stage.WorldPosToTileId(transform.position);
           
        }

        if(coMovePath != null)
        {
            StopCoroutine(coMovePath);
            coMovePath = null;
            isMoving = false;
           
        }

        isMoving = false;
        animator.speed = 0f;

        var search = new GraphSearch();
        search.Init(graph);
        if(!search.AStar(graph.nodes[currentTileId], graph.nodes[targetTileId]))
        {
            Debug.Log("갈 수 없습니다.");
            return;
        }

        var path = search.path;
        stage.ShowPath(path);  // Stage가 이전 경로를 자동으로 지움
        coMovePath = StartCoroutine(MovePath(path));

    }

    private IEnumerator MovePath(List<GraphNode> path)
    {
        foreach (var node in path.Skip(1))
        {
            //var targetPos = stage.GetTilePos(node.id);
            var tile = map.tiles[node.id];
            MoveTo(tile);
            yield return new WaitUntil(() => !isMoving);
        }
        stage.ClearPath(path);
        coMovePath = null;
    }
}