using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingStarter : MonoBehaviour
{
    public float force = 5f;
    public float torqueStrength = 1f;
    public float swingSpeed = 2f;

    void Start()
    {
        GetComponent<Rigidbody2D>().AddForce(transform.right * force, ForceMode2D.Impulse);
    }

    void FixedUpdate()
    {
        float torque = Mathf.Sin(Time.time * swingSpeed) * torqueStrength;
        GetComponent<Rigidbody2D>().AddTorque(torque);
    }
}
