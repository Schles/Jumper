using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GatedByButton : MonoBehaviour
{
    // Start is called before the first frame update

    private Dictionary<string, bool> buttonStates;

    private Animator animator;

    private Collider2D col2D;

    public bool IsOpen = false;

    void Awake() {

        animator = GetComponent<Animator>();
        buttonStates = new Dictionary<string, bool>();        
        col2D = GetComponent<Collider2D>();
                
        for (var i = 0; i < transform.childCount; i++) {
            var child = transform.GetChild(i).GetComponent<Button>();
            buttonStates.Add(child.id, false);
            
       }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetState(string id, bool state) {
        buttonStates[id] = state;

        IsOpen = CheckAllButtonActive();
        animator.SetBool("IsOpen", IsOpen);
        col2D.enabled = !IsOpen;
        print("isGated " + !IsOpen);
    }

    bool CheckAllButtonActive() {
        foreach (var state in buttonStates) {
            if (state.Value == false) {
                return false;
            }
        }

        return true;
    }
}
