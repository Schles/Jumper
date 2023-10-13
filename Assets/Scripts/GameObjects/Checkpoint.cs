using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour, IDataPersistance
{
    [SerializeField] private string id;

    public static Checkpoint active;
    public bool IsActive = false;
    
    private bool visitied = false;

    [ContextMenu("Set ID")]
    private void GenrateGuid()
    {
        id = Guid.NewGuid().ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Animator>().StopPlayback();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate()
    {
        this.IsActive = true;
        GetComponent<Animator>().StartPlayback();
        GetComponent<AudioSource>().Play();
    }

    public void Deactivate()
    {
        this.IsActive = false;
        GetComponent<Animator>().StopPlayback();
        
    }
    
    public void OnTriggerEnter2D(Collider2D other)
    {

        if (Checkpoint.active == this)
        {
            return;
        }
        
        if (Checkpoint.active)
        {
            Checkpoint.active.Deactivate();
        }
        
        Checkpoint.active = this;
        
        this.Activate();
    }

    
    public void LoadGame(GameData data)
    {
        data.checkpoints.TryGetValue(id, out visitied);
        if (data.activeCheckpointId.Equals(id))
        {
            Checkpoint.active = this;
            this.Activate();
        }
    }

    public void SaveGame(ref GameData data)
    {
        if (data.checkpoints.ContainsKey(id))
        {
            data.checkpoints.Remove(id);
        }
        
        data.checkpoints.Add(id, visitied);

        if (Checkpoint.active == this)
        {
            data.activeCheckpointId = id;
        }
        
        
        
    }

}
