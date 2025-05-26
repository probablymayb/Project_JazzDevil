using UnityEngine;
using UnityEngine.UI;

public class ToggleBackgroundController : MonoBehaviour
{
    public Toggle toggle;             // 자기 자신 Toggle
    public GameObject background;     // Toggle 안의 검정 Background

    void Start()
    {
        if (toggle == null) toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnToggleChanged);
        background.SetActive(toggle.isOn); // 시작 상태 반영
    }

    void OnToggleChanged(bool isOn)
    {
        background.SetActive(isOn);
    }
}
