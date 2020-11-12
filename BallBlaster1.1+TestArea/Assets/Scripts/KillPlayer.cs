using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class KillPlayer : MonoBehaviour
{
    public GameObject player;

    void OnTriggerEnter(Collider player)
    {
            player.gameObject.transform.position = new Vector3(0.0f,2.0f,0.0f);
    }
}
