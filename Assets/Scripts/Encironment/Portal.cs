using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField] private int sceneIndex;
    public void TeleportPlayer()
    {
        SceneLoading.Instance.LoadScene(sceneIndex);
    }
}
