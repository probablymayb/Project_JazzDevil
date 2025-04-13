using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public enum SceneName
    {
        Title,
        Main,
        Result,
        // 이후 확장 가능
    }

    public static void LoadScene(SceneName scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }
}
