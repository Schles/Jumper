using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundByDistance : MonoBehaviour
{
    public Transform listenerTransform;
    public float maxDistance;
    public float minDistance;
    
    private float dist;
    private AudioSource audioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        listenerTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        dist = Vector3.Distance(transform.position, listenerTransform.position);
 print("dist" + dist + " max " + maxDistance);
        if(dist < maxDistance)
        {
            //audioSource.volume = 1;
            float val = (dist - minDistance) / (maxDistance - minDistance);
            print("factor" + val);
            audioSource.volume = 1 - (val);
        }
        else if(dist > maxDistance)
        {
            audioSource.volume = 0;
        }
        else
        {

        }   
    }
}
