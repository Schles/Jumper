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

    public int offset = 0;

    public float waitDuration = 0f;
    
    private int pathIndex = 0;
    private int pathCount;

    private bool isWaiting = false;
    private float remainingWaitTime = 0f;
    
    // Start is called before the first frame update
    void Start()
    {
        
        pathCount = path.GetCount();
        pathIndex = offset;

        transform.position = path.GetWaypoint(pathIndex).position;
    }
    
    private void LateUpdate()
    {

        if (isWaiting) {
            remainingWaitTime -= Time.deltaTime;
            if (remainingWaitTime <= 0f)
            {
                isWaiting = false;
            }
            else
            {
                return;
            }
        }

        if (pathIndex< pathCount)
        {
            var nextWaypoint = path.GetWaypoint(pathIndex).transform.position; 
            transform.position = Vector2.MoveTowards(transform.position, nextWaypoint, speed * Time.deltaTime);


            if (transform.position == nextWaypoint)
            {
                pathIndex++;
                if (pathIndex == 1) {
                    isWaiting = true;
                    this.remainingWaitTime = waitDuration;
                }

            }
            
            if (pathIndex == pathCount)
            {
                isWaiting = true;
                this.remainingWaitTime = waitDuration;
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
