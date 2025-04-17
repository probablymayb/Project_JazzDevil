using UnityEngine;
using System.Collections;

public class TitleSceneManager : MonoBehaviour
{
    private ScreenFader fader;

    void Start()
    {
        fader = FindFirstObjectByType<ScreenFader>();
        if (fader == null)
        {
            Debug.LogWarning("ScreenFader가 씬에 없습니다!");
        }
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            StartCoroutine(StartGame());
        }
    }

    IEnumerator StartGame()
    {
        if (fader != null)
        {
            yield return fader.FadeOut();
        }

        SceneLoader.LoadScene(SceneLoader.SceneName.Main);
    }
}
