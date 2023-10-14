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

    private LineRenderer lineRenderer;

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

    void Awake() {
        
        removeAction.performed += ctx => { OnRelease(ctx); };
        //holdAction.performed += ctx => { OnHold(ctx); };
        //holdAction.canceled += ctx => { OnHoldRelease(ctx); };
    }

    private void OnHoldRelease(InputAction.CallbackContext ctx)
    {
        this.attached = false;
    }


    private void OnHold(InputAction.CallbackContext ctx)
    {
        this.attached = true;
    }


    private void OnRelease(InputAction.CallbackContext ctx)
    {
        if( hookEntity != null) {
            hingeJoint2D.enabled = false;
            //lineRenderer.enabled = false;
            Destroy(hookEntity);
            hookEntity = null;
        }
    }


    void Start()
    {
    
        lineRenderer = GetComponent<LineRenderer>();
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

        // if (springJoint2D.enabled) {
        //     lineRenderer.SetPosition(0, transform.position);
        //     lineRenderer.SetPosition(1, hookEntity.transform.position);            
        // }

    print("holding" + holdAction.IsPressed());
        var collision = Physics2D.OverlapBox(groundCheckPoint[0].position, _ropeCheckSize, 0, ropeLayer);

        if (!attached && collision && holdAction.IsPressed()) {
            Attach(collision.GetComponent<Rigidbody2D>());
        } else if (attached && !holdAction.IsPressed()) {
            Detach();
        }

        lastTimeOnRope -= Time.deltaTime;
        // if (attached && curCollision != null && lastTimeOnRope < 0f) {
        //     hingeJoint2D.enabled = true;
        //     hingeJoint2D.connectedBody = curCollision.GetComponent<Rigidbody2D>();
        // } else if (attached == false) {
        //     hingeJoint2D.enabled = false;
        //     curCollision = null;
        // }
    }


    public void Attach(Rigidbody2D ropeBone) {
            ropeBone.GetComponent<RopeSegment>().isPlayerAttached = true;
            hingeJoint2D.connectedBody = ropeBone;
            hingeJoint2D.enabled = true;
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
        if( direction > 0) {
            if(myConnection.connectedAbove != null) {
                if(myConnection.connectedAbove.gameObject.GetComponent<RopeSegment>() != null) {
                    newSeg = myConnection.connectedAbove;
                }
            }
        } else {
            if(myConnection.connectedBelow != null) {         
                newSeg = myConnection.connectedBelow;
            }
        }

        if (newSeg != null) {
            transform.position = newSeg.transform.position;
            myConnection.isPlayerAttached = false;
            newSeg.GetComponent<RopeSegment>().isPlayerAttached = true;
            hingeJoint2D.connectedBody = newSeg.GetComponent<Rigidbody2D>();

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

    private GameObject DrawRope(Vector3 point) {
        hookEntity = Instantiate(hook, point, Quaternion.identity);

        var parent = hookEntity.GetComponent<Rigidbody2D>();

        GameObject ropeEntity = null;

        var distance = (transform.position - point).magnitude;

        for( var i = 0; i < Mathf.Floor(distance); i++) {
            var pos = hookEntity.transform.position + (Vector3.down * i);
            ropeEntity = Instantiate(rope, pos, Quaternion.identity);
            ropeEntity.GetComponent<HingeJoint2D>().connectedBody = parent;
            parent = ropeEntity.GetComponent<Rigidbody2D>();

            ropeEntity.transform.parent = hookEntity.transform;
        }

        return ropeEntity;
    }

    public void OnReset(Dictionary<string, object> message)
    {
        hingeJoint2D.enabled = false;
        Destroy(hookEntity);
        hookEntity = null;
    }


    private void OnTriggerEnter2D(Collider2D other) {
        // if(!attached) {
        //     if(other.gameObject.CompareTag("Rope")) {
        //         if (attachedTo != other.gameObject.transform.parent) {
        //             if (disregard == null || other.gameObject.transform.parent.gameObject != disregard) {
        //                 print("Attaching");
        //                 Attach(other.gameObject.GetComponent<Rigidbody2D>());
        //             }
                    
        //         }
        //     }
        // }

        if (other.gameObject.CompareTag("Rope")) {
            curCollision = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        // if (other.gameObject.CompareTag("Rope") && curCollision == other.gameObject) {
        //     curCollision = null;
        // }
    }

}
