using UnityEngine;
using System.Collections.Generic;

public class NavNode : MonoBehaviour
{
    [SerializeField] protected List<NavNode> neighbors;
    public List<NavNode> Neighbors { get { return neighbors; } set { neighbors = value; } }
    public float Cost { get; set; } = 0;
    public NavNode PreviousNavNode { get; set; } = null;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        foreach (NavNode n in neighbors)
        {
            Gizmos.DrawLine(transform.position, n.transform.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<NavPathMovement>(out NavPathMovement navMovement))
        {
            navMovement.OnEnterNavNode(this);
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent<NavPathMovement>(out NavPathMovement navMovement))
        {
            navMovement.OnEnterNavNode(this);
        }
    }

    #region helper_functions

    public static NavNode[] GetAllNavNodes()
    {
        return FindObjectsByType<NavNode>(FindObjectsSortMode.None);
    }
    public static NavNode GetRandomNavNode()
    {
        var navNodes = GetAllNavNodes();
        return (navNodes.Length == 0) ? null : navNodes[Random.Range(0, navNodes.Length)];
    }

    public static NavNode GetNearestNavNode(Vector3 position)
    {
        NavNode nearestNavNode = null;
        float nearestdistance = float.MaxValue;

        var navNodes = GetAllNavNodes();
        foreach (NavNode n in navNodes)
        {
            float distance = Vector3.Distance(n.transform.position, position);
            if (distance < nearestdistance)
            {
                nearestNavNode = n;
                nearestdistance = distance;
            }
        }

        return nearestNavNode;
    }

    public static void ResetNavNodes()
    {
        var navNodes = GetAllNavNodes();

        foreach(NavNode n in navNodes)
        {
            n.Cost = float.MaxValue;
            n.PreviousNavNode = null;
        }
    }

    public static void CreatePath(NavNode navNode, ref List<NavNode> path)
    {
        while (navNode != null)
        {
            path.Add(navNode);
            navNode = navNode.PreviousNavNode;
        }
        path.Reverse();
    }

    #endregion
}
