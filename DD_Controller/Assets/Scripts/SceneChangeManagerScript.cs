using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneName
{
    Lobby,
    MonoPlayer,
    Multiplayer,
}

public class ChangeToScene : MonoBehaviour
{
    public SceneName sceneName;

    public void ChangeScene()
    {
        SceneManagerScript.ChangeToScene(sceneName);
    }

}

public static class SceneList
{
    public static readonly Dictionary<SceneName, string> SceneNames = new Dictionary<SceneName, string>
    {
        { SceneName.Lobby, "LobbyTutorial_Done" },
        { SceneName.MonoPlayer, "MonoPlayerScene" },
        { SceneName.Multiplayer, "MultiplayerScene" },
    };
}

public class SceneManagerScript : MonoBehaviour
{

    public static void ChangeToScene(SceneName sceneToLoad)
    {
        SceneManager.LoadScene(SceneList.SceneNames[sceneToLoad]);
    }
}

