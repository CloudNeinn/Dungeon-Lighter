using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawMovement : MonoBehaviour
{
    public Transform[] waypoints;
    public int currentWaypointIndex = 0;
    public float speed = 2f;
    private int _direction = 1;
    [SerializeField] private bool loopMovement;

    private void FixedUpdate()
    {
        MoveSaw();
    }

    private void MoveSaw()
    {
        if (waypoints.Length == 0) return;

        Transform TargetWaypoint = waypoints[currentWaypointIndex];
        transform.position = Vector2.MoveTowards(transform.position, TargetWaypoint.position, speed * Time.fixedDeltaTime);

        if (Vector2.Distance(transform.position, TargetWaypoint.position) > 0.1f) return;
        if(loopMovement)
        {
            currentWaypointIndex += _direction;
            if (currentWaypointIndex >= waypoints.Length) currentWaypointIndex = 0;
        }
        else if(!loopMovement)
        {
            currentWaypointIndex += _direction;
            if (currentWaypointIndex >= waypoints.Length || currentWaypointIndex < 0)
            {
                _direction *= -1;
                currentWaypointIndex += _direction;
            }
        }
    }
}
