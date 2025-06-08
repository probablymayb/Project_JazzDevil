using UnityEngine;
using System.Collections;
using FMOD.Studio;
using FMODUnity;

public class TitleSceneManager : MonoBehaviour
{
    private ScreenFader fader;

    [SerializeField] private EventReference introBGM;

    void Start()
    {
        fader = FindFirstObjectByType<ScreenFader>();
        if (fader == null)
        {
            Debug.LogWarning("ScreenFader가 씬에 없습니다!");
        }
        AudioManager.Instance.PlayLooping(introBGM, "");

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
