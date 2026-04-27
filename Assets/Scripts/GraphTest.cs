using System.Collections.Generic;
using UnityEngine;

public class GraphTest : MonoBehaviour
{
    public enum Algoritm
    {
        DFS,
        BFS,
        DFSRecursive,
        PathFindingBFS,
        Dijkstra,
        AStar   
    }

    public Transform uiNodeRoot;

    public UiGraphNode nodePrefab;

    private List<UiGraphNode> uiNodes = new();
    private Graph graph;

    [Header("CONFIG")]
    public Algoritm algoritm;
    public int startId;
    public int endId;


    private void Start()
    {
        int[,] map = new int[5, 5]
        {
            { 1, -1, 1, 1, 1 },
            { 1, -1, 1, 1, 1 },
            { 1, -1, 1, 1, 1 },
            { 1, -1, 1, 1, 1 },
            { 1,  1, 1, 8, 1 },
        };

        graph = new Graph();
        graph.Init(map);

        InitUiNodes(graph);
    }

    private void InitUiNodes(Graph graph)
    {
        foreach (var node in graph.nodes)
        {
            var uiNode = Instantiate(nodePrefab, uiNodeRoot);
            uiNode.SetNode(node);
            uiNode.Reset();
            uiNodes.Add(uiNode);
        }
    }

    private void ResetUiNodes()
    {
        foreach (var node in uiNodes)
        {
            node.Reset();
        }
    }

    [ContextMenu("Search")]
    public void Search()
    {
        var search = new GraphSearch();
        search.Init(graph);

        switch (algoritm)
        {
            case Algoritm.DFS:
                search.DFS(graph.nodes[startId]);
                break;
            case Algoritm.BFS:
                search.BFS(graph.nodes[startId]);
                break;
            case Algoritm.DFSRecursive:
                search.DFSRecursive(graph.nodes[startId]);
                break;
            case Algoritm.PathFindingBFS:
                search.PathFindingBFS(graph.nodes[startId], graph.nodes[endId]);
                break;
            case Algoritm.Dijkstra:
                search.Dijkstra(graph.nodes[startId], graph.nodes[endId]);
                break;

            case Algoritm.AStar:
                search.AStar(graph.nodes[startId], graph.nodes[endId]);
                break;


        }

        ResetUiNodes();

        if (search.path.Count <= 1)
        {
            if (search.path.Count == 1)
            {
                var only = search.path[0];
                uiNodes[only.id].SetColor(Color.red);
            }
            return;
        }

        for (int i = 0; i < search.path.Count; i++)
        {
            var node = search.path[i];
            var color = Color.Lerp(Color.red, Color.green, (float)i / (search.path.Count - 1));

            uiNodes[node.id].SetColor(color);
            uiNodes[node.id].SetText($"ID: {node.id}\nWeight: {node.weight}\nPath:{i}");
        }
    }
}