using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    [SerializeField] private float _dialogueRadius;
    [SerializeField] private LayerMask _playerLayer;

    void Update()
    {

    }

    bool inRadius()
    {
        return Physics2D.OverlapCircle(transform.position, _dialogueRadius, _playerLayer);
    }
}
