using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class CardUIEventHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite hoverSprite;

    private Image uiImage;

    public event Action<SupporterSO> OnItemClicked;

    public SupporterSO CurrentSupporterData { get; set; }

    private void Awake()
    {
        uiImage = transform.Find("Card Image").GetComponent<Image>();
        if (uiImage == null)
        {
            Debug.LogError("Image 컴포넌트를 찾을 수 없음.");
            enabled = false;
            return;
        }
        uiImage.sprite = defaultSprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSprite != null)
        {
            uiImage.sprite = hoverSprite;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (defaultSprite != null)
        {
            uiImage.sprite = defaultSprite;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (CurrentSupporterData != null)
        {
            OnItemClicked?.Invoke(CurrentSupporterData);
        }
        else
        {
            Debug.LogWarning("클릭된 아이템에 SupporterSO 데이터가 설정되지 않았음");
        }
    }
}
