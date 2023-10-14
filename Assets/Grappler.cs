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


    public bool isHolding = false;

    public bool hasShoot = false;

    public GameObject curCollision;

    void Awake() {
        removeAction.performed += ctx => { OnRelease(ctx); };
        holdAction.performed += ctx => { OnHold(ctx); };
        holdAction.canceled += ctx => { OnHoldRelease(ctx); };
    }

    private void OnHoldRelease(InputAction.CallbackContext ctx)
    {
        this.isHolding = false;
    }


    private void OnHold(InputAction.CallbackContext ctx)
    {
        this.isHolding = true;
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
            //var cast =  Physics2D.Raycast(transform.position, transform.TransformDirection(aimAmount), maxDistance, LayerMask.GetMask("Ground"));
            //if(cast) {
                //springJoint2D.enabled = true;
                //lineRenderer.enabled = true;

            //    var go = this.DrawRope(cast.point);
                // hingeJoint2D.connectedBody = go.GetComponent<Rigidbody2D>();
                // hingeJoint2D.enabled = true;
               
           // }

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

        lastTimeOnRope -= Time.deltaTime;
        if (isHolding && curCollision != null && lastTimeOnRope < 0f) {
            hingeJoint2D.enabled = true;
            hingeJoint2D.connectedBody = curCollision.GetComponent<Rigidbody2D>();
        } else if (isHolding == false) {
            hingeJoint2D.enabled = false;
        }
    }

    public bool IsHanging() {
        return isHolding && curCollision != null;
    }
    
    public void Decouple() {
        hingeJoint2D.enabled = false;
        curCollision = null;

        lastTimeOnRope = 0.5f;
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


    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Rope") && curCollision == null) {
            curCollision = other.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if (other.gameObject.CompareTag("Rope") && curCollision == other.gameObject) {
            curCollision = null;
        }
    }
}
