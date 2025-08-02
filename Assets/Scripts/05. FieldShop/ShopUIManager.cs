using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ShopUIManager : Singleton<ShopUIManager>
{
    private List<SupporterSO> selectedSupSo;

    [SerializeField] private GameObject shopObj;
    [SerializeField] private List<GameObject> items;

    private RectTransform itemsRectTransform;
    private Vector2 startPosition = new Vector2(0, -1800);
    private Vector2 endPosition = new Vector2(0, 0);
    private float animationDuration = 0.25f;
    private Ease easeType = Ease.OutCubic;

    protected override void Awake()
    {
        base.Awake();
        itemsRectTransform = shopObj.transform.Find("Items Layout Group").GetComponent<RectTransform>();
    }

    public void ShopOpen()
    {
        shopObj.SetActive(true);
        itemsRectTransform.anchoredPosition = startPosition;
        itemsRectTransform.DOAnchorPos(endPosition, animationDuration)
            .SetUpdate(true)
            .SetEase(easeType)
            .OnComplete(() => Debug.Log("Shop UI 애니메이션 완료."));
        SetSupporters();
        GameManager.Instance.ChangeState(EGameState.Paused);
    }

    public void ShopClose()
    {
        shopObj.SetActive(false);
        GameManager.Instance.ChangeState(EGameState.Playing);
    }

    /// <summary>
    /// 상점에 중복없는 무작위 동료 3개를 세팅.
    /// </summary>
    private void SetSupporters()
    {
        selectedSupSo = SupporterManager.Instance.SupporterSos.OrderBy(x => Guid.NewGuid()).Take(3).ToList();
        for (int i = 0; i < 3; ++i)
        {
            GameObject itemObj = items[i];
            SupporterSO currentSupporter = selectedSupSo[i];

            CardUIEventHandler itemEventHandler = itemObj.GetComponent<CardUIEventHandler>();
            if (itemEventHandler == null)
            {
                Debug.LogError($"item {i}에 CardUIEventHandler 컴포넌트가 없음");
                continue;
            }
            itemEventHandler.CurrentSupporterData = currentSupporter;

            itemObj.transform.Find("Item Image").GetComponent<Image>().sprite = currentSupporter.sprite;
            itemObj.transform.Find("Name Text").GetComponent<Text>().text = currentSupporter.name;
            itemObj.transform.Find("Desc Text").GetComponent<Text>().text = currentSupporter.desc;
            itemObj.transform.Find("NEW!").gameObject.SetActive(!SupporterManager.Instance.IsSupporterOwned(currentSupporter.supporterType));

            itemEventHandler.OnItemClicked -= HandleSupporterItemClick;
            itemEventHandler.OnItemClicked += HandleSupporterItemClick;
        }
    }

    private void HandleSupporterItemClick(SupporterSO clickedSupporter)
    {
        Debug.Log($"상점에서 {clickedSupporter.name} 동료가 클릭되었음");
        SupporterManager.Instance.AddSup(clickedSupporter.supporterType);
        ShopClose();
    }
}
