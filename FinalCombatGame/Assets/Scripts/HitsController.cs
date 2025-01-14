using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitsController : MonoBehaviour
{
    public enum Attacks
    {
        QuickHigh,
        QuickLow,
        SlowHigh,
        SlowLow,
        None
    }
    public BoxCollider QuickHighCollider;
    public BoxCollider QuickLowCollider;
    public BoxCollider SlowHighCollider;
    public BoxCollider SlowLowCollider;

    private void Start()
    {
        QuickHighCollider.enabled = false;
        QuickLowCollider.enabled = false;
        SlowHighCollider.enabled = false;
        SlowLowCollider.enabled = false;

    }
    public void EnableCollider(Attacks attacks)
    {
        switch (attacks)
        {
            case Attacks.QuickHigh:
                QuickHighCollider.enabled = true;
                break;
            case Attacks.QuickLow:
                QuickLowCollider.enabled = true;
                break;
            case Attacks.SlowHigh:
                SlowHighCollider.enabled = true;
                break;
            case Attacks.SlowLow:
                SlowLowCollider.enabled = true;
                break;
        }
    }

    public void DisableCollider(Attacks attacks)
    {
        switch (attacks)
        {
            case Attacks.QuickHigh:
                QuickHighCollider.enabled = false;
                break;
            case Attacks.QuickLow:
                QuickLowCollider.enabled = false;
                break;
            case Attacks.SlowHigh:
                SlowHighCollider.enabled = false;
                break;
            case Attacks.SlowLow:
                SlowLowCollider.enabled = false;
                break;
        }
    }

}
