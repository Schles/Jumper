using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{

    public string nextScene;

    public GameObject prefab;

    // Start is called before the first frame update
    void Start()
    {
        var p = transform.localToWorldMatrix.MultiplyPoint3x4(Vector3.down * 1f);
        print("start " + p );

        if (prefab) {
            var go = Instantiate(prefab, p, Quaternion.identity);

            // var newVector = new Vector3(p.x, p.y, p.z) - transform.position;
            // var rotation = Quaternion.FromToRotation(transform.position, newVector);
            // rotation *= Quaternion.Euler(0, 0, 270);

            // go.transform.rotation = rotation;
            go.transform.rotation = transform.rotation;

            //go.GetComponent<HingeJoint2D>().connectedBody = GetComponent<Rigidbody2D>();
            //go.GetComponent<HingeJoint2D>().enabled = false;
            go.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
