using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class BoxMovement : MonoBehaviour
{
    [Range(0.1f, 20f)]
    public float speed;
    public WaypointPath path;
    
    private int pathIndex = 0;
    private int pathCount;
    
    // Start is called before the first frame update
    void Start()
    {
        transform.position = path.GetWaypoint(0).position;
        pathCount = path.GetCount();
    }
    
    private void LateUpdate()
    {
        if (pathIndex < pathCount)
        {
            var nextWaypoint = path.GetWaypoint(pathIndex).transform.position; 
            transform.position = Vector2.MoveTowards(transform.position, nextWaypoint, speed * Time.deltaTime);

            if (transform.position == nextWaypoint)
            {
                pathIndex++;
            }
            
            if (pathIndex == pathCount)
            {
                pathIndex = 0;
            }
        }
        

    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        other.transform.SetParent(transform);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        other.transform.SetParent(null);
    }
}
