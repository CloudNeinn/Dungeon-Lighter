using UnityEngine;

public class Propeller : MonoBehaviour
{
    private enum RotationDirection
    {
        Clockwise = -1,
        CounterClockwise = 1
    }

    [SerializeField] private float _rotationSpeed;
    [SerializeField] private RotationDirection _rotationDirection = RotationDirection.Clockwise;

    void Update()
    {
        transform.Rotate(new Vector3(0, 0, _rotationSpeed) * Time.deltaTime * (int)_rotationDirection);
    }
}
