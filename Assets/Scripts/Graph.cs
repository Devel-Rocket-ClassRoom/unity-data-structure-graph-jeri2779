using System.Collections.Generic;

public class Graph
{
    public int row = 0;
    public int col = 0;

    public GraphNode[] nodes;

    public void Init(int[,] grid)
    {
        row = grid.GetLength(0);
        col = grid.GetLength(1);

        nodes = new GraphNode[grid.Length];


        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i] = new GraphNode()
            {
                id = i,
                adjusts = new()
            };
        }

        for (int r = 0; r < row; r++)
        {
            for (int c = 0; c < col; c++)
            {
                int index = r * col + c;
                nodes[index].weight = grid[r, c];

                if (nodes[index].weight == -1)
                    continue;

                if (r - 1 >= 0 && grid[r - 1, c] >= 0)
                {
                    int connectIndex = (r - 1) * col + c;
                    nodes[index].adjusts.Add(nodes[connectIndex]);
                }

                if (r + 1 < row && grid[r + 1, c] >= 0)
                {
                    int connectIndex = (r + 1) * col + c;
                    nodes[index].adjusts.Add(nodes[connectIndex]);
                }

                if (c - 1 >= 0 && grid[r, c - 1] >= 0)
                {
                    int connectIndex = r * col + c - 1;
                    nodes[index].adjusts.Add(nodes[connectIndex]);
                }

                if (c + 1 < col && grid[r, c + 1] >= 0)
                {
                    int connectIndex = r * col + c + 1;
                    nodes[index].adjusts.Add(nodes[connectIndex]);
                }
            }
        }
    }

    public void ResetNodePrevious()
    {
        foreach (var node in nodes)
        {
            node.previous = null;
        }
    }
}

 

 
