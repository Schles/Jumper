using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{

    public float duration = 3f;

    public bool isActive = false;
    private float remainingTime = 0f;


    [SerializeField] public string id;

    
    [ContextMenu("Set ID")]
    private void GenrateGuid()
    {
        id = Guid.NewGuid().ToString();
    }

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate() {
        if (isActive) {
            remainingTime -= Time.deltaTime;
            if (remainingTime <= 0) {
                isActive = false;
                transform.parent.GetComponent<GatedByButton>().SetState(id, isActive);
                animator.SetBool("IsActive", isActive);
            }
        }
    }

    void OnTriggerEnter2D (Collider2D other) {
        if (other.gameObject.tag == "Player") {
            isActive = true;
            remainingTime = duration;
            transform.parent.GetComponent<GatedByButton>().SetState(id, isActive);
            animator.SetBool("IsActive", isActive);
        }
    }
}
