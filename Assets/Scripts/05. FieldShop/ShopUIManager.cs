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
            
            // 업그레이드된 능력치 표시
            string descText = GetSupporterDescription(currentSupporter);
            itemObj.transform.Find("Desc Text").GetComponent<Text>().text = descText;
            
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

    /// <summary>
    /// 현재 보유 여부에 따라 업그레이드 표시를 포함한 설명 텍스트 반환
    /// </summary>
    private string GetSupporterDescription(SupporterSO supporterSO)
    {
        const float UPGRADE_MULTIPLIER = 1.5f;
        
        bool owned = SupporterManager.Instance.IsSupporterOwned(supporterSO.supporterType);
        string template = supporterSO.desc;

        // 현재 스탯: owned이면 런타임 스탯, 아니면 원본
        float curCooldown;
        int curDamage;
        float curRange;

        if (owned)
        {
            var runtimeStats = SupporterManager.Instance.GetRuntimeStats(supporterSO.supporterType);
            if (runtimeStats != null)
            {
                curCooldown = runtimeStats.attackCooldown;
                curDamage = runtimeStats.attackDamage;
                curRange = runtimeStats.attackRange;
            }
            else
            {
                // fallback - SO 원본 사용
                curCooldown = supporterSO.attackCooldown;
                curDamage = supporterSO.attackDamage;
                curRange = supporterSO.attackRange;
            }
        }
        else
        {
            curCooldown = supporterSO.attackCooldown;
            curDamage = supporterSO.attackDamage;
            curRange = supporterSO.attackRange;
        }

        // 다음 업그레이드 예상 값 (preview) – owned일 때 사용
        int nextDamage = Mathf.CeilToInt(curDamage * UPGRADE_MULTIPLIER);
        float nextCooldown = curCooldown > 0f ? curCooldown / UPGRADE_MULTIPLIER : 0f;
        float nextRange = curRange * UPGRADE_MULTIPLIER;

        int firstLineEnd = template.IndexOf('\n');
        if (owned && firstLineEnd > 0)
        {
            int currentLevel = SupporterManager.Instance.GetSupporterLevel(supporterSO.supporterType);
            int nextLevel = currentLevel + 1; // 업그레이드 후 레벨 표시
            template = template.Insert(firstLineEnd, $" [Lv.{nextLevel}]");
        }

        // 토큰 구성: (현재값->다음값) 또는 (현재값) 형태
        string cooldownToken;
        if (curCooldown > 0f)
        {
            cooldownToken = owned ? $"{curCooldown:0.#}->{nextCooldown:0.##}초" : $"{curCooldown:0.#}초";
        }
        else
        {
            cooldownToken = "상시발동"; // 0은 상시 발동
        }

        // 데미지 토큰 (KontraBass는 감속 퍼센트이지만 표시는 동일)
        string damageToken = owned ? $"{curDamage}->{nextDamage}" : curDamage.ToString();

        string rangeToken = owned ? $"{curRange:0.#}->{nextRange:0.#}" : curRange.ToString("0.#");

        // 플레이스홀더 치환
        if (template.Contains("{cooldown}") || template.Contains("{damage}") || template.Contains("{range}"))
        {
            template = template
                .Replace("{cooldown}", cooldownToken)
                .Replace("{damage}", damageToken)
                .Replace("{range}", rangeToken);
            return template;
        }

        // 플레이스홀더 없을 경우(이전 형식) – 이미 템플릿화 했으므로 거의 미사용 경로
        return template;
    }
}
