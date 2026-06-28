using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField] private int _sceneIndex;
    [SerializeField] private float _interacivityRadius;
    [SerializeField] private Vector3 _interactiovityOffset;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private int _returnCurrency;
    [SerializeField] private Animator _animator;
    [SerializeField] private bool _isAbyss;
    [SerializeField] private float _encloseCooldownTimer;
    [SerializeField] private string _levelName;

    void Start()
    {
        if(_animator == null) _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(_isAbyss && _encloseCooldownTimer < 0) _animator.SetTrigger("Enclose");
        else if(_isAbyss) _encloseCooldownTimer -= Time.deltaTime;
        if(UserInput.Instance.use1Input && InRange()) EnterDungeon();
    }

    public void EnterDungeon()
    {
        if (_returnCurrency > 0)
        {
            CurrencyManager.Instance.addCurrency(_returnCurrency);
            GameManager.Instance.SetLevelComplete(this.gameObject.scene.name);
        }
        if (SceneLoading.Instance.currentSceneType != SceneLoading.SceneType.Level) SceneLoading.Instance.SetReturnDoor(this.transform.position);
        //else ShotManager.Instance.activeShot = ShotManager.Shots.None;
        SceneLoading.Instance.LoadScene(_sceneIndex);
        SceneLoading.Instance.SetLevelName(_levelName);
    }

    public void RemoveDoor()
    {
        gameObject.SetActive(false);
    }

    bool InRange()
    {
        return Physics2D.OverlapCircle(transform.position + _interactiovityOffset, _interacivityRadius, _playerLayer); 
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + _interactiovityOffset, _interacivityRadius);
    }
}
