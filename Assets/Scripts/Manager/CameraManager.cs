using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    private Transform target;
    public float yOffset = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void LateUpdate()
    {

        if (target == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag("Player");
            if (go != null)
            {
                target = go.GetComponent<Transform>();
            }
            
        }

        if (target != null)
        {
            transform.position = new Vector3(target.position.x, target.position.y + yOffset, transform.position.z );
        }

        /*
        
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
