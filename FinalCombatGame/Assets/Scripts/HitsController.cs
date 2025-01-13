using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitsController : MonoBehaviour
{
    public BoxCollider collider;

    private void Start()
    {
        collider.enabled = false;

    }
    public void EnableCollider()
    {
        collider.enabled = true;
    }
    
    public void DisableCollider()
    {
        collider.enabled = false;
    }
}
