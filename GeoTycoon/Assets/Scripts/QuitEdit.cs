using UnityEngine;

public class QuitEdit : MonoBehaviour
{
    // This method will quit the game
    public void ExitGame()
    {
        // If we are running in the Unity editor
#if UNITY_EDITOR
        // Stop playing the scene
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // If we are running in a standalone build of the game
        Application.Quit();
#endif
    }
}