using Priority_Queue;
using System.Collections.Generic;
using UnityEngine;

public static class NavAStar
{
    public static bool Generate(NavNode startNode, NavNode endNode, ref List<NavNode> path)
    {
        var nodes = new SimplePriorityQueue<NavNode>();

        startNode.Cost = 0;
        float heuristic = Vector3.Distance(startNode.transform.position, endNode.transform.position);
        nodes.Enqueue(startNode, startNode.Cost + heuristic);

        while (nodes.Count != 0)
        {
            var currentNode = nodes.Dequeue();

            if (currentNode == endNode)
            {
                NavNode.CreatePath(endNode, ref path);
                return true;
            }

            foreach (var neighbor in currentNode.Neighbors)
            {
                float cost = currentNode.Cost + Vector3.Distance(currentNode.transform.position, neighbor.transform.position);

                if (cost < neighbor.Cost)
                {
                    neighbor.Cost = cost;
                    neighbor.PreviousNavNode = currentNode;

                    heuristic = Vector3.Distance(neighbor.transform.position, endNode.transform.position);

                    nodes.EnqueueWithoutDuplicates(neighbor, cost + heuristic);
                }
            }
        }

        return false;
    }
}
