using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

public static class NavDijkstra
{
    public static bool Generate(NavNode startNode, NavNode endNode, ref List<NavNode> path)
    {
        var nodes = new SimplePriorityQueue<NavNode>();

        startNode.Cost = 0;
        nodes.Enqueue(startNode, startNode.Cost);

        while (nodes.Count != 0) {
            var currentNode = nodes.Dequeue();

            if (currentNode == endNode) {
                NavNode.CreatePath(endNode, ref path);
                return true;
            }

            foreach (var neighbor in currentNode.Neighbors) {
                float cost = currentNode.Cost + Vector3.Distance(currentNode.transform.position, neighbor.transform.position);

               if (cost < neighbor.Cost) {
                    neighbor.Cost = cost;
                    neighbor.PreviousNavNode = currentNode;

                    nodes.EnqueueWithoutDuplicates(neighbor, cost);
                }
            }
        }

        return false;
    }
}
