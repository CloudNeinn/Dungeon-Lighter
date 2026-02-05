using UnityEngine;

public class CanvasIntermediary : MonoBehaviour
{
    public void OnCloseAnimationFinished()
    {        
        SceneLoading.Instance.SetCloseAnimationFinished();
    }
}
