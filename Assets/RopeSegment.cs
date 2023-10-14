using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class RopeSegment : MonoBehaviour
{

    public GameObject connectedBelow, connectedAbove;

    public bool isPlayerAttached = false;

    // Start is called before the first frame update
    void Start()
    {
        connectedAbove = GetComponent<HingeJoint2D>().connectedBody.gameObject;
        RopeSegment aboveSegment = connectedAbove.GetComponent<RopeSegment>();
        if(aboveSegment != null) {
            aboveSegment.connectedBelow = gameObject;
            //var spriteBottom = connectedAbove.GetComponent<SpriteRenderer>().bounds.size.y;
            GetComponent<HingeJoint2D>().connectedAnchor = new Vector2(0, 1);   
        } else {
            GetComponent<HingeJoint2D>().connectedAnchor = new Vector2(0, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
