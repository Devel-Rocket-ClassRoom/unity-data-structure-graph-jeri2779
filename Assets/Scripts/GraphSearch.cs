using System.Collections.Generic;
using UnityEngine;

public class GraphSearch
{
    private Graph graph;

    public List<GraphNode> path = new();

    public void Init(Graph graph)
    {
        this.graph = graph;
    }

    public void DFS(GraphNode startNode)
    {
        path.Clear();

        var visited = new HashSet<GraphNode>();
        var stack = new Stack<GraphNode>();

        stack.Push(startNode);
        visited.Add(startNode);

        while (stack.Count > 0)
        {
            var currentNode = stack.Pop();
            path.Add(currentNode);

            foreach (var adjust in currentNode.adjusts)
            {
                if (!adjust.CanVisit || visited.Contains(adjust))
                    continue;

                visited.Add(adjust);
                stack.Push(adjust);
            }
        }
    }
    //BFS 구현
    public void BFS(GraphNode startNode)
    {
        path.Clear();
        var visited = new HashSet<GraphNode>();
        var queue = new Queue<GraphNode>();
        queue.Enqueue(startNode);
        visited.Add(startNode);
        while (queue.Count > 0)
        {
            var currentNode = queue.Dequeue();
            path.Add(currentNode);
            foreach (var adjust in currentNode.adjusts)
            {
                if (!adjust.CanVisit || visited.Contains(adjust))
                    continue;
                visited.Add(adjust);
                queue.Enqueue(adjust);
            }
        }
    }
    public void DFSRecursive(GraphNode currentNode, HashSet<GraphNode> visited)
    {
        //DFS 재귀 구현
        if (visited == null || !currentNode.CanVisit || visited.Contains(currentNode))
        {
            return;
        }

        path.Add(currentNode);
        visited.Add(currentNode);

        foreach (var adjust in currentNode.adjusts)
        {
            if (!adjust.CanVisit || visited.Contains(adjust))
            {
                continue;
            }
            DFSRecursive(adjust, visited);
        }
    }
    public void DFSRecursive(GraphNode startNode)
    {
        path.Clear();
        var visited = new HashSet<GraphNode>();
        DFSRecursive(startNode, visited);
    }
    //public void PathFindingBFS(GraphNode startNode, GraphNode endNode)
    //{
    //    //BFS로 경로 찾기 구현
    //    var queue = new Queue<GraphNode>();
    //    var visit = new HashSet<GraphNode>();
    //    var parentMap = new Dictionary<GraphNode, GraphNode>();

    //    visit.Add(startNode);
    //    parentMap[startNode] = null;
    //    queue.Enqueue(startNode);

    //    while (queue.Count > 0)
    //    {
    //        var currentNode = queue.Dequeue();
    //        if (currentNode == endNode)
    //        {
    //            path.Clear();
    //            var node = endNode;
    //            while (node != null)
    //            {
    //                path.Add(node);
    //                node = parentMap[node];
    //            }
    //            path.Reverse();

    //            return;
    //        }
    //        foreach (var adjust in currentNode.adjusts)
    //        {
    //            if (!adjust.CanVisit || visit.Contains(adjust))
    //            {
    //                continue;
    //            }
    //            visit.Add(adjust);
    //            parentMap[adjust] = currentNode;
    //            queue.Enqueue(adjust);
    //        }


    //    }

    //}
    public bool PathFindingBFS(GraphNode startNode, GraphNode endNode)
    {
        path.Clear();
        graph.ResetNodePrevious();

        var visited = new HashSet<GraphNode>();
        var queue = new Queue<GraphNode>();
        
        queue.Enqueue(startNode);
        visited.Add(startNode);
        bool success = false; 
        while (queue.Count > 0)
        {
            var currentNode = queue.Dequeue();
            if (currentNode == endNode)
            {
                success = true;
                break;
            }
            foreach (var adjust in currentNode.adjusts)
            {
                if (!adjust.CanVisit || visited.Contains(adjust))
                    continue;

                visited.Add(adjust);
                adjust.previous = currentNode;
                queue.Enqueue(adjust);
            }
        }
        if(!success)
        {
            path.Clear();
            return false;
        }
        GraphNode step = endNode;
        while(step != null)
        {
            path.Add(step);
            step = step.previous;
        }

        path.Reverse();
        return true;

        
    }
    public bool Dijkstra(GraphNode startNode, GraphNode endNode)
    { 
        path.Clear();
        graph.ResetNodePrevious();
        int nodeCount = graph.nodes.Length;

        var distance = new int[nodeCount];
        var previous = new GraphNode[nodeCount];  
        var visited =  new bool[nodeCount];
        var pq = new PriorityQueue<GraphNode, int>();

        for (int i = 0; i < nodeCount; i++)
        {
            distance[i] = int.MaxValue;
        }

        distance[startNode.id] = 0;

        pq.Enqueue(startNode, 0);

        bool success = false;
        while (pq.Count > 0)
        {
            var currentNode = pq.Dequeue();

            if (visited[currentNode.id])
            {
                continue;
            }
            visited[currentNode.id] = true;

            if (currentNode == endNode)
            {
                var current = endNode;
                while(current != null)
                {
                    path.Add(current);
                    current = previous[current.id];
                }
                path.Reverse();
                return true;
            }

            foreach(var adjacent in currentNode.adjusts)
            {
                if (!adjacent.CanVisit || visited[adjacent.id])
                {
                    continue;
                }
                int newDist = distance[currentNode.id] + adjacent.weight;
                
                if (newDist < distance[adjacent.id])
                {
                    distance[adjacent.id] = newDist;
                    previous[adjacent.id] = currentNode;
                    pq.Enqueue(adjacent, distance[adjacent.id]);
                }
            }
        }
        if(!success)
        {
            return false;

        }
        GraphNode step = endNode;
        while (step != null)
        {
            path.Add(step);
            step = step.previous;
        }
        return true;
    }

    public bool AStar(GraphNode startNode, GraphNode endNode)
    {
        path.Clear();
        int nodeCount = graph.nodes.Length;
        var startCost = new int[nodeCount]; 
        var totalCost = new int[nodeCount];
        var previous = new GraphNode[nodeCount];
        var visited = new bool[nodeCount];

        for (int i = 0; i < nodeCount; i++)
        {
            startCost[i] = int.MaxValue;
            totalCost[i] = int.MaxValue;
            previous[i] = null;
        }

        startCost[startNode.id] = 0;
        totalCost[startNode.id] = Heuristic(startNode, endNode);
        var pq = new PriorityQueue<GraphNode, int>();
        pq.Enqueue(startNode, totalCost[startNode.id]);
        while (pq.Count > 0)
        {
            var currentNode = pq.Dequeue();
            if (visited[currentNode.id])
            {
                continue;
            }
            visited[currentNode.id] = true;
            if (currentNode == endNode)
            {
                var current = endNode;
                while (current != null)
                {
                    path.Add(current);
                    current = previous[current.id];
                }
                path.Reverse();
                return true;
            }
            foreach (var adjacent in currentNode.adjusts)
            {
                if (!adjacent.CanVisit || visited[adjacent.id])
                {
                    continue;
                }
                int tentativeStartCost = startCost[currentNode.id] + adjacent.weight;
                if (tentativeStartCost < startCost[adjacent.id])
                {
                    startCost[adjacent.id] = tentativeStartCost;
                    totalCost[adjacent.id] = tentativeStartCost + Heuristic(adjacent, endNode);
                    previous[adjacent.id] = currentNode;
                    pq.Enqueue(adjacent, totalCost[adjacent.id]);
                }
            }
        }
        return false;
    }

    private int Heuristic(GraphNode a, GraphNode b)
    {
        //휴리스틱 함수 구현 (예: 맨해튼 거리)
        int ax = a.id % graph.col;
        int ay = a.id / graph.col;
        int bx = b.id % graph.col;
        int by = b.id / graph.col;
        return Mathf.Abs(ax - bx) + Mathf.Abs(ay - by);
    }

 



}