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
    [SerializeField] private int returnCurrency;

    void Update()
    {
        if(PlayerControl.Instance._use1Input && InRange()) EnterDungeon();
    }

    public void EnterDungeon()
    {
        if (returnCurrency > 0)
        {
            CurrencyManager.Instance.addCurrency(returnCurrency);
            GameManager.Instance.increaseCompletedLevels();
        }
        if (SceneLoading.Instance.currentSceneType != SceneLoading.SceneType.Level) SceneLoading.Instance.SetReturnDoor(this.transform.position);
        //else ShotManager.Instance.activeShot = ShotManager.Shots.None;
        SceneLoading.Instance.LoadScene(_sceneIndex);
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
