using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WaypointPath : MonoBehaviour
{

    public int GetCount()
    {
        return transform.childCount;
    }
    
    public Transform GetWaypoint(int waypointIndex)
    {
        return transform.GetChild(waypointIndex);
    }

    public int GetNextWaypointIndex(int currentWaypointIndex)
    {
        int nextWaypointIndex = currentWaypointIndex + 1;

        if(nextWaypointIndex == transform.childCount)
        {
            nextWaypointIndex = 0;
        }

        return nextWaypointIndex;


    }
    
    private void OnDrawGizmos()
    {
        for (int waypointIndex = 0; waypointIndex < transform.childCount; waypointIndex++)
        {
            var waypoint = GetWaypoint(waypointIndex);

            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(waypoint.position, 0.2f);

            int nextWaypointIndex = GetNextWaypointIndex(waypointIndex);
            var nextWaypoint = GetWaypoint(nextWaypointIndex);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(waypoint.position, nextWaypoint.position);
        }
    }
}
