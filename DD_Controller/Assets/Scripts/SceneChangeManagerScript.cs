using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneName
{
    Lobby
}

public static class SceneList
{
    public static readonly Dictionary<SceneName, string> SceneNames = new()
    {
        { SceneName.Lobby, "LobbyTutorial_Done" }
    };
}

public class SceneManagerScript : MonoBehaviour
{
    public static void ChangeToScene(SceneName sceneToLoad)
    {
        SceneManager.LoadScene(SceneList.SceneNames[sceneToLoad]);
    }
}