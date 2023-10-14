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

    public float maxDistance = 13f;
    private Rigidbody2D curParent;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        curParent = rb;
    }

    // Update is called once per frame
    void Update()
    {
        distanceTraveled += rb.velocity.magnitude * Time.deltaTime;
        segmentLength += rb.velocity.magnitude * Time.deltaTime;

        const float scale = 1f;

        if (segmentLength > scale && distanceTraveled < maxDistance && rb.isKinematic == false) {
            var go = Instantiate(ropeSegment, curParent.transform.position + Vector3.down, Quaternion.identity);

            Vector3 theScale = go.transform.localScale;
            theScale.y = scale;
            go.transform.localScale = theScale;
            go.transform.parent = transform.transform;
            go.GetComponent<HingeJoint2D>().connectedBody = curParent;
            curParent = go.GetComponent<Rigidbody2D>();

            

            segmentLength = 0f;
        }

        //transform.position += Vector3.right * Time.deltaTime * 0.5f;

        //rb.velocity = Vector2.up * 4f;


    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Wall")) {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;


            var childArray = GetComponentsInChildren<Rigidbody2D>();

            for (var i = 0; i < childArray.Length; i++) {
                childArray[i].velocity = Vector3.zero;
                childArray[i].angularVelocity = 0f;
            }
            
            
        }
    }
}