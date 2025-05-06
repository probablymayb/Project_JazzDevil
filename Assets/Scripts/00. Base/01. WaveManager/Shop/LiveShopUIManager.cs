using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// 상점 UI 연출 및 아이템 구매 처리 담당 (PlayerController의 골드 사용)
/// </summary>
public class LiveShopUIManager : Singleton<LiveShopUIManager>
{
    [System.Serializable]
    public class ItemSlot
    {
        public RectTransform blackCircle; // 점 애니메이션용
        public GameObject itemUI;         // 실제 아이템 UI
        public Toggle toggle;             // 선택 여부
        public int cost;                  // 아이템 가격
    }

    [Header("UI 요소")]
    public List<ItemSlot> itemSlots;
    public GameObject returnButton;
    public float appearDelay = 0.2f;

    private PlayerController player;

    private void Start()
    {
        gameObject.SetActive(false);

        player = FindFirstObjectByType<PlayerController>();
    }


    // 상점 UI 열기 (애니메이션 포함)
    public void OpenShop()
    {
        if (player == null) player = FindFirstObjectByType<PlayerController>();

        gameObject.SetActive(true);
        StartCoroutine(PlayOpenAnimation());
    }


    // 검은 원이 커지며 아이템이 등장하는 애니메이션
    private IEnumerator PlayOpenAnimation()
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            var slot = itemSlots[i];
            slot.toggle.isOn = false;
            slot.itemUI.SetActive(false);
            slot.blackCircle.localScale = Vector3.zero;
            slot.blackCircle.gameObject.SetActive(true);

            slot.blackCircle.DOScale(1f, 0.4f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                slot.blackCircle.gameObject.SetActive(false);
                slot.itemUI.SetActive(true);
            });

            yield return new WaitForSeconds(appearDelay);
        }

        returnButton.SetActive(true);
    }


    // 리턴 버튼 클릭 시: 구매 시도 → 성공 시 UI 닫기 / 실패 시 경고
    public void OnClickReturn()
    {
        int totalCost = 0;

        // 선택된 아이템들의 총 가격 계산
        foreach (var slot in itemSlots)
        {
            if (slot.toggle.isOn)
                totalCost += slot.cost;
        }

        // 골드 부족 시 경고
        if (!player.SpendGold(totalCost))
        {
            Debug.Log("[LiveShopUIManager] 골드 부족 - 구매 불가");
            return;
        }

        // 구매 성공 시 아이템 지급 로직
        foreach (var slot in itemSlots)
        {
            if (slot.toggle.isOn)
            {
                Debug.Log($"[Shop] 아이템 구매: {slot.cost}골드");
                
                // TODO: 실제 효과 적용 (ex: 공격력 업, 체력 회복 등)
            }
        }

        CloseShop();
    }


    // 점처럼 작아지며 상점 UI 닫힘
    public void CloseShop()
    {
        returnButton.SetActive(false);
        StartCoroutine(PlayCloseAnimation());
    }


    // 닫힘 애니메이션: 점으로 축소 후 UI 비활성화
    private IEnumerator PlayCloseAnimation()
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            var slot = itemSlots[i];
            slot.itemUI.SetActive(false);
            slot.blackCircle.localScale = Vector3.one;
            slot.blackCircle.gameObject.SetActive(true);

            slot.blackCircle.DOScale(0f, 0.4f).SetEase(Ease.InBack);
            yield return new WaitForSeconds(appearDelay);
        }

        gameObject.SetActive(false);
    }
}
