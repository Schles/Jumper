using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Hook : MonoBehaviour
{
    private Rigidbody2D rb;

    public GameObject ropeSegment;

    public float distanceTraveled = 0f;
    private float segmentLength = 0f;

    public int maxDistance = 5;
    private Rigidbody2D curParent;
    private Rigidbody2D startNode;

    private int curSegments = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        curParent = null;

        CreateSegmentAbove();

        startNode = curParent;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        distanceTraveled += rb.velocity.magnitude * Time.deltaTime;
        

        if(Input.GetKeyDown(KeyCode.L)) {
            AddSegmentBelow();
        }        

        if( rb.bodyType != RigidbodyType2D.Static ) {
            var curHead = curParent.transform.localToWorldMatrix.MultiplyPoint3x4(Vector3.up * 1f);
            var distance = (curHead - transform.position);

            if (distance.magnitude > 1f) {
                curParent.transform.localScale = new Vector3(1f, 1f, 1f);           
                CreateSegmentAbove();
                segmentLength = 0f;
            } else {
                curParent.transform.localScale = new Vector3(1f, segmentLength, 1f);
            }

        }

    
        //transform.position += Vector3.right * Time.deltaTime * 0.5f;

        //rb.velocity = Vector2.up * 4f;


    }

    public void CreateSegmentAbove() {
        curSegments++;

        var go = Instantiate(ropeSegment, transform.position, Quaternion.identity);

        go.transform.parent = transform.transform;
        go.transform.localScale = new Vector3(1f, 0.1f, 1f);
                
        if (curParent != null) {
            var p = curParent.transform.localToWorldMatrix.MultiplyPoint3x4(Vector3.up * 1f);
            go.transform.position = p;

            var newVector = new Vector3(p.x, p.y, p.z) - transform.position;

            var rotation = Quaternion.FromToRotation(transform.position, newVector);
            rotation *= Quaternion.Euler(0, 0, 90);

            go.transform.rotation = rotation;
            

            go.GetComponent<HingeJoint2D>().connectedBody = curParent;
            go.GetComponent<RopeSegment>().connectedBelow = curParent.gameObject;
            curParent.GetComponent<RopeSegment>().connectedAbove = go;
            curParent.GetComponent<HingeJoint2D>().connectedBody = go.GetComponent<Rigidbody2D>();
        }
        

        
        curParent = go.GetComponent<Rigidbody2D>();
    }

    public void AddSegmentBelow() {
        curSegments++;
        var go = Instantiate(ropeSegment, transform.position, Quaternion.identity);

        go.transform.parent = transform.transform;
                
        if (startNode != null) {
            var p = startNode.transform.localToWorldMatrix.MultiplyPoint3x4(Vector3.down * 1f);
            go.transform.position = p;

            // var newVector = new Vector3(p.x, p.y, p.z) - transform.position;
            // var rotation = Quaternion.FromToRotation(transform.position, newVector);
            // rotation *= Quaternion.Euler(0, 0, 90);

            // go.transform.rotation = rotation;

            go.transform.rotation = startNode.transform.rotation;
            

            go.GetComponent<HingeJoint2D>().connectedBody = startNode;
            startNode.GetComponent<RopeSegment>().connectedBelow = go;
            go.GetComponent<RopeSegment>().connectedAbove = startNode.gameObject;
            //curParent.GetComponent<HingeJoint2D>().connectedBody = go.GetComponent<Rigidbody2D>();
        }
        

        
        startNode = go.GetComponent<Rigidbody2D>();
    }

    public void RemoveSegment() {
        curSegments--;
        var newParent = startNode.GetComponent<HingeJoint2D>().connectedBody;
        Destroy(startNode.gameObject);
        startNode = newParent;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Wall")) {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;


            CreateSegmentAbove();

            curParent.GetComponent<LineRenderer>().material.color = Color.yellow;

            var dist = curParent.transform.position - transform.position;

            curParent.transform.localScale = new Vector3(1f, dist.magnitude, 1f);

            


            curParent.GetComponent<HingeJoint2D>().connectedBody = rb;
            curParent.GetComponent<HingeJoint2D>().autoConfigureConnectedAnchor = false;
            curParent.GetComponent<HingeJoint2D>().connectedAnchor = Vector2.zero;

            var childArray = GetComponentsInChildren<Rigidbody2D>();

            for (var i = 0; i < childArray.Length; i++) {
                childArray[i].velocity = Vector3.zero;
                childArray[i].angularVelocity = 0f;
      
            }
            
            rb.bodyType = RigidbodyType2D.Static;
        }
    }
}
