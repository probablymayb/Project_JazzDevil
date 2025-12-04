using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ComboUI : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] private Text comboText;

    [Header("파티클 참조")]
    [SerializeField] private GameObject fireParticlePrefab;
    [SerializeField] private Vector3 particleLocalPos = new Vector3(2f, 1f, 5f);  // 인스펙터에서 조정 가능
    private GameObject fireParticleInstance;
    private ParticleSystem fireParticle;

    [Header("애니메이션 설정")]
    [SerializeField] private float punchScale = 1.3f;
    [SerializeField] private float punchDuration = 0.15f;

    [Header("콤보 색상")]
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color bronzeColor = new Color(0.8f, 0.5f, 0.2f);
    [SerializeField] private Color silverColor = new Color(0.75f, 0.75f, 0.8f);
    [SerializeField] private Color goldColor = new Color(1f, 0.84f, 0f);
    [SerializeField] private Color rainbowColor = new Color(1f, 0.4f, 0.7f);

    [Header("파티클 활성화 기준")]
    [SerializeField] private int fireThreshold = 20;

    private void Start()
    {
        if (ComboManager.Instance != null)
        {
            ComboManager.Instance.OnComboChanged += UpdateComboUI;
            ComboManager.Instance.OnComboBreak += OnComboBreak;
        }

        // 카메라 자식으로 파티클 생성
        if (fireParticlePrefab != null)
        {
            Camera cam = Camera.main ?? FindFirstObjectByType<Camera>();
            if (cam != null)
            {
                fireParticleInstance = Instantiate(fireParticlePrefab, cam.transform);
                fireParticleInstance.transform.localPosition = particleLocalPos;
                fireParticle = fireParticleInstance.GetComponent<ParticleSystem>();
                fireParticleInstance.SetActive(false);
            }
        }

        UpdateComboUI(0);
    }

    private void OnDestroy()
    {
        if (ComboManager.Instance != null)
        {
            ComboManager.Instance.OnComboChanged -= UpdateComboUI;
            ComboManager.Instance.OnComboBreak -= OnComboBreak;
        }
    }

    private void UpdateComboUI(int combo)
    {
        if (combo > 0)
        {
            comboText.text = $"{combo} combo";
            comboText.color = GetComboColor(combo);
            comboText.gameObject.SetActive(true);

            comboText.transform.DOKill();
            comboText.transform.localScale = Vector3.one;
            comboText.transform.DOPunchScale(Vector3.one * (punchScale - 1f), punchDuration);

            UpdateFireParticle(combo >= fireThreshold);
        }
        else
        {
            comboText.gameObject.SetActive(false);
            UpdateFireParticle(false);
        }
    }

    private void UpdateFireParticle(bool active)
    {
        if (fireParticleInstance == null) return;

        if (active && !fireParticleInstance.activeSelf)
        {
            fireParticleInstance.SetActive(true);
            fireParticle.Play();
        }
        else if (!active && fireParticleInstance.activeSelf)
        {
            fireParticle.Stop();
            fireParticleInstance.SetActive(false);
        }
    }

    private Color GetComboColor(int combo)
    {
        if (combo >= 50) return rainbowColor;
        if (combo >= 30) return goldColor;
        if (combo >= 20) return silverColor;
        if (combo >= 10) return bronzeColor;
        return defaultColor;
    }

    private void OnComboBreak(int lastCombo)
    {
        UpdateFireParticle(false);

        comboText.DOFade(0f, 0.3f).OnComplete(() =>
        {
            comboText.gameObject.SetActive(false);
            comboText.color = new Color(comboText.color.r, comboText.color.g, comboText.color.b, 1f);
        });
    }
}
