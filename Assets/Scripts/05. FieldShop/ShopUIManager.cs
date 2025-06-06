using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class ShopUIManager : Singleton<ShopUIManager>
{
    [Header("UI 참조")]
    public GameObject shopRootUI;
    public List<GameObject> itemSlotPrefabs; // ItemSlotPrefab(0~4)
    public Button returnButton;

    [Header("애니메이션 설정")]
    public float appearDelay = 0.1f;

    public PlayerController player;

    protected override void Awake()
    {
        base.Awake();
        shopRootUI.SetActive(false);
        if (returnButton != null)
            returnButton.gameObject.SetActive(false);
    }

    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>();

        if (returnButton != null)
            returnButton.onClick.AddListener(OnClickConfirm);
    }

    public void OpenShop()
    {
        foreach (var slotPrefab in itemSlotPrefabs)
        {
            var toggle = slotPrefab.transform.Find("ItemSlot")?.GetComponent<Toggle>();
            var background = slotPrefab.transform.Find("ItemSlot/Background")?.gameObject;
            if (toggle != null && background != null)
            {
                toggle.isOn = false;
                background.SetActive(false); // 초기화

                toggle.onValueChanged.RemoveAllListeners(); // 중복 방지
                toggle.onValueChanged.AddListener((isOn) =>
                {
                    Debug.Log($"[DEBUG] {slotPrefab.name} 클릭됨 - 토글 상태: {isOn}");
                    background.SetActive(isOn);
                });
            }
        }

        shopRootUI.SetActive(true);
        StartCoroutine(PlayOpenAnimation());

        if (returnButton != null)
            returnButton.gameObject.SetActive(true);
        returnButton.interactable = true;
    }

    public void CloseShop()
    {
        returnButton.interactable = false;
        if (returnButton != null)
            returnButton.gameObject.SetActive(false);

        StartCoroutine(PlayCloseAnimation());
        shopRootUI.SetActive(false);
    }

    private IEnumerator PlayOpenAnimation()
    {
        foreach (var slotPrefab in itemSlotPrefabs)
        {
            var blackCircle = slotPrefab.transform.Find("blackCircle")?.GetComponent<RectTransform>();
            var itemSlot = slotPrefab.transform.Find("ItemSlot")?.gameObject;

            if (blackCircle == null || itemSlot == null) continue;

            itemSlot.SetActive(false);
            blackCircle.localScale = Vector3.zero;
            blackCircle.gameObject.SetActive(true);
        }

        yield return null;

        foreach (var slotPrefab in itemSlotPrefabs)
        {
            var blackCircle = slotPrefab.transform.Find("blackCircle")?.GetComponent<RectTransform>();
            var itemSlot = slotPrefab.transform.Find("ItemSlot")?.gameObject;

            if (blackCircle == null || itemSlot == null) continue;

            blackCircle
                .DOScale(1f, 0.3f)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    blackCircle.gameObject.SetActive(false);
                    itemSlot.SetActive(true);
                });

            yield return new WaitForSeconds(appearDelay);
        }

        returnButton.interactable = true;
    }

    private IEnumerator PlayCloseAnimation()
    {
        foreach (var slotPrefab in itemSlotPrefabs)
        {
            var blackCircle = slotPrefab.transform.Find("blackCircle")?.GetComponent<RectTransform>();
            var itemSlot = slotPrefab.transform.Find("ItemSlot")?.gameObject;

            if (blackCircle == null || itemSlot == null) continue;

            itemSlot.SetActive(false);
            blackCircle.localScale = Vector3.one;
            blackCircle.gameObject.SetActive(true);
        }

        yield return null;

        foreach (var slotPrefab in itemSlotPrefabs)
        {
            var blackCircle = slotPrefab.transform.Find("blackCircle")?.GetComponent<RectTransform>();

            if (blackCircle == null) continue;

            blackCircle
                .DOScale(0f, 0.3f)
                .SetEase(Ease.InBack);

            yield return new WaitForSeconds(appearDelay);
        }

        shopRootUI.SetActive(false);
    }

    public void OnClickConfirm()
    {
        int total = 0;

        foreach (var slotPrefab in itemSlotPrefabs)
        {
            var toggle = slotPrefab.transform.Find("ItemSlot")?.GetComponent<Toggle>();
            if (toggle != null && toggle.isOn)
            {
                // 가격은 추후 동적으로 넣을 수 있도록 구조를 열어둠 (임시로 10)
                total += 10;
            }
        }

        if (!player.SpendGold(total))
        {
            Debug.Log("골드 부족!");
            return;
        }

        Debug.Log("구매 성공!");
        CloseShop();
    }
}
