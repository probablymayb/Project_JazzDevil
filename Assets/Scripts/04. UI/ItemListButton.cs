using UnityEngine;
using UnityEngine.UI;

public class ItemListButton : MonoBehaviour
{
    private Button button;

    void Start()
    {
        // Button 컴포넌트 가져오기
        button = GetComponent<Button>();

        if (button != null)
        {
            // 버튼 클릭 이벤트에 메서드 연결
            button.onClick.AddListener(OnButtonClick);
        }
        else
        {
            Debug.LogError("Button 컴포넌트를 찾을 수 없습니다!");
        }
    }

    // 버튼 클릭 시 호출되는 메서드
    public void OnButtonClick()
    {
        Debug.Log("클릭");
    }

    void OnDestroy()
    {
        // 메모리 누수 방지를 위해 이벤트 리스너 제거
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClick);
        }
    }
}
