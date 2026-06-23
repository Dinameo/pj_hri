using UnityEngine;

public class QuitButton : MonoBehaviour
{
    public void QuitApp()
    {
        Debug.Log("Thoát app");

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}