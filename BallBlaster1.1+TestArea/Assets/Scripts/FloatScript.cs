using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class FloatScript : MonoBehaviour
{
    public float speed;

    // Update is called once per frame
    void OnTriggerStay(Collider other)
    {
        other.attachedRigidbody.AddForce(Vector3.up * speed);
    }
}
