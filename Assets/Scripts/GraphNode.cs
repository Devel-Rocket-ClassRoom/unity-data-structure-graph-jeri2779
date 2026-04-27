using System.Collections.Generic;

public class GraphNode
{
    public int id;                      //노드 번호
    public int weight = 1;              //노드 가중치
    public List<GraphNode> adjusts = new();//인접 노드 리스트

    public GraphNode previous = null;       //이전 노드

    public bool CanVisit => adjusts.Count > 0 && weight > 0;//방문 가능한 노드인지 여부
    //노드 방문 여부는 인접 노드가 존재하고, 가중치가 0보다 큰 경우로 판단
}
