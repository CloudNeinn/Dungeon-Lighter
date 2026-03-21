using UnityEngine;
using Cinemachine;

public class ParallaxEffect : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [Range(0f, 1f)]
    [SerializeField] private float _parallaxFactorX = 0.5f;
    [Range(0f, 1f)]
    [SerializeField] private float _parallaxFactorY = 0.5f;

    private Vector2 _startPosition;
    private Vector2 _travel => (Vector2)_camera.transform.position - _startPosition;

    void Start()
    {
        if (_camera == null)
            _camera = Camera.main;

        _startPosition = transform.position;
        CinemachineCore.CameraUpdatedEvent.AddListener(OnCameraUpdated);
    }

    private void OnDestroy()
    {
        CinemachineCore.CameraUpdatedEvent.RemoveListener(OnCameraUpdated);
    }

    void OnCameraUpdated(CinemachineBrain brain)
    {
        Vector2 travel = _travel;
        float newPosX = _startPosition.x - travel.x * _parallaxFactorX;
        float newPosY = _startPosition.y - travel.y * _parallaxFactorY;
        transform.position = new Vector3(newPosX, newPosY, transform.position.z);
    }
}