using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    [SerializeField] private float dialogueRadius;
    [SerializeField] private LayerMask playerLayer;

    void Update()
    {

    }

    bool inRadius()
    {
        return Physics2D.OverlapCircle(transform.position, dialogueRadius, playerLayer);
    }
}
