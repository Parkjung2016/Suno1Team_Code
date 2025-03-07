using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MovingPlatform : NetworkBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!IsHost) return;
        if (other.transform.CompareTag("Player"))
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (!IsHost) return;
        if (other.transform.CompareTag("Player"))
        {
            other.transform.SetParent(null);
        }
    }
}