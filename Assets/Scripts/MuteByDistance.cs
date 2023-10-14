using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundByDistance : MonoBehaviour
{
    private Transform Listener;
    private AudioSource source;
 
    private float sqrMinDist;
    private float sqrMaxDist;
    
    // Start is called before the first frame update
    void Start()
    {
        Listener = FindFirstObjectByType<AudioListener>().transform;
        source = GetComponent<AudioSource>();
        if (!source) return;
 
        sqrMinDist = source.minDistance * source.minDistance;
        sqrMaxDist = source.maxDistance * source.maxDistance;
    }

    // Update is called once per frame
    void Update()
    {
        if (!source || !Listener) return;
        
        
        var t = Vector2.Distance(transform.position, Listener.position);

        if (t > source.maxDistance)
        {
            source.mute = true;
        }
        else
        {
            source.mute = false;
        }
        
        t = (t*t  - sqrMinDist) / (sqrMaxDist - sqrMinDist);
        source.spatialBlend = Mathf.Lerp(10f,22000f,t);
    }
}
