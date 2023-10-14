using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grappler : MonoBehaviour
{
    // Start is called before the first frame update

    private HingeJoint2D hingeJoint2D;

    public InputAction aimAction;

    public InputAction removeAction;

    public InputAction holdAction;

    public GameObject hook;

    private GameObject hookEntity;

    public GameObject rope;

    public float speed = 40f;

    public float maxDistance = 100f;


    public bool attached = false;

    public Transform attachedTo;

    public GameObject disregard;

    public bool hasShoot = false;

    public GameObject curCollision;

    public Transform[] groundCheckPoint;

    public LayerMask ropeLayer;

    private Vector2 _ropeCheckSize = new Vector2(0.5f, 1f);


    void Start()
    {

        hingeJoint2D = GetComponent<HingeJoint2D>();
        hingeJoint2D.enabled = false;   
        
    }

    // Update is called once per frame
    void Update()
    {

        var aimAmount = aimAction.ReadValue<Vector2>();
        if (hasShoot == false && aimAmount.magnitude > 0.1f) {
            if (hookEntity != null) {
                Destroy(hookEntity);
                hookEntity = null;
            }

            hookEntity = Instantiate(hook, transform.position + Vector3.up, Quaternion.identity);
            hookEntity.GetComponent<Rigidbody2D>().velocity = aimAmount.normalized * speed;

            hasShoot = true;

        } 

        if (hasShoot && aimAmount.magnitude < 0.1f) {
            hasShoot = false;
        }

        var collision = Physics2D.OverlapBox(groundCheckPoint[0].position, _ropeCheckSize, 0, ropeLayer);

        if (!attached && collision && holdAction.IsPressed()) {
            Attach(collision.GetComponent<Rigidbody2D>());
        } else if (attached && !holdAction.IsPressed()) {
            Detach();
        }

        lastTimeOnRope -= Time.deltaTime;

    }


    public void Attach(Rigidbody2D ropeBone) {
            ropeBone.GetComponent<RopeSegment>().isPlayerAttached = true;
            hingeJoint2D.connectedBody = ropeBone;
            hingeJoint2D.enabled = true;
            hingeJoint2D.connectedAnchor =  ropeBone.transform.InverseTransformPoint(transform.position);
            attached = true;
            attachedTo = ropeBone.gameObject.transform.parent;
    }

    public void Detach() {
        hingeJoint2D.connectedBody.gameObject.GetComponent<RopeSegment>().isPlayerAttached = false;
        attached = false;
        hingeJoint2D.enabled = false;
        hingeJoint2D.connectedBody = null;
    }

    public bool IsHanging() {
        return attached && curCollision != null;
    }
    
    public void Decouple() {
        hingeJoint2D.enabled = false;
        curCollision = null;

        lastTimeOnRope = 0.5f;
    }

    public void Slide(int direction) {
        RopeSegment myConnection = hingeJoint2D.connectedBody.gameObject.GetComponent<RopeSegment>();
        GameObject newSeg = null;

        var speed = 0.1f;

        if( direction > 0) {

            
            hingeJoint2D.connectedAnchor = new Vector2(0f, hingeJoint2D.connectedAnchor.y + direction * speed);

            if ( hingeJoint2D.connectedAnchor.y > 1f) {
                if(myConnection.connectedAbove != null) {
                    if(myConnection.connectedAbove.gameObject.GetComponent<RopeSegment>() != null) {
                        newSeg = myConnection.connectedAbove;
                        


                        myConnection.isPlayerAttached = false;
                        newSeg.GetComponent<RopeSegment>().isPlayerAttached = true;

                        if (myConnection.connectedBelow == null) {
                            hookEntity.GetComponent<Hook>().RemoveSegment();    
                        }

                        hingeJoint2D.connectedBody = newSeg.GetComponent<Rigidbody2D>();
                        hingeJoint2D.connectedAnchor = new Vector2(0f, 0f);
                    }
                } else {
                    hingeJoint2D.connectedAnchor = new Vector2(0f, 1f);
                }
            }
        } else {
            hingeJoint2D.connectedAnchor = new Vector2(0f, hingeJoint2D.connectedAnchor.y + direction * speed);

            if (hingeJoint2D.connectedAnchor.y < 0f) {

                if( myConnection.connectedBelow == null) {
                    hookEntity.GetComponent<Hook>().AddSegment();
                }

                if(myConnection.connectedBelow != null) {         
                    newSeg = myConnection.connectedBelow;

                    myConnection.isPlayerAttached = false;
                    newSeg.GetComponent<RopeSegment>().isPlayerAttached = true;
                    hingeJoint2D.connectedBody = newSeg.GetComponent<Rigidbody2D>();
                    hingeJoint2D.connectedAnchor = new Vector2(0f, 1f);
                } 
            }
        }
    }

    private float lastTimeOnRope = 0f;

    public void OnEnable()
    {
        aimAction.Enable();
        removeAction.Enable();
        holdAction.Enable();
        EventManager.StartListening("restart", OnReset);
    }

    public void OnDisable()
    {
        aimAction.Disable();
        removeAction.Disable();
        holdAction.Disable();
        EventManager.StopListening("restart", OnReset);
    }


    public void OnReset(Dictionary<string, object> message)
    {
        hingeJoint2D.enabled = false;
        Destroy(hookEntity);
        hookEntity = null;
    }

}
