using UnityEngine;
using System.Collections;
using DG.Tweening;

public class BuildingBouncer : MonoBehaviour
{
    [Header("바운스 설정")]
    [SerializeField] private float bounceScale = 1.05f;
    [SerializeField] private float bounceDuration = 0.1f;
    [SerializeField] private float returnDuration = 0.2f;

    private Transform[] buildings;
    private Vector3[] originalScales;
    private Coroutine bounceCoroutine;

    private void Start()
    {
        // 자식 건물들 캐싱 (자기 자신 제외)
        int childCount = transform.childCount;
        buildings = new Transform[childCount];
        originalScales = new Vector3[childCount];

        for (int i = 0; i < childCount; i++)
        {
            buildings[i] = transform.GetChild(i);
            originalScales[i] = buildings[i].localScale;
        }

        RhythmManager.beatUpdated += OnBeat;
    }

    private void OnDestroy()
    {
        RhythmManager.beatUpdated -= OnBeat;
    }

    private void OnBeat()
    {
        if (bounceCoroutine != null)
            StopCoroutine(bounceCoroutine);

        bounceCoroutine = StartCoroutine(BounceAll());
    }

    private IEnumerator BounceAll()
    {
        float elapsed = 0f;

        // 커지기
        while (elapsed < bounceDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / bounceDuration;

            for (int i = 0; i < buildings.Length; i++)
            {
                buildings[i].localScale = Vector3.Lerp(
                    originalScales[i],
                    originalScales[i] * bounceScale,
                    t
                );
            }
            yield return null;
        }

        elapsed = 0f;

        // 돌아오기
        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / returnDuration;

            for (int i = 0; i < buildings.Length; i++)
            {
                buildings[i].localScale = Vector3.Lerp(
                    originalScales[i] * bounceScale,
                    originalScales[i],
                    t
                );
            }
            yield return null;
        }

        // 정확한 원본 복원
        for (int i = 0; i < buildings.Length; i++)
            buildings[i].localScale = originalScales[i];
    }
}
