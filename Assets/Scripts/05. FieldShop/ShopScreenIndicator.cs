using UnityEngine;
using UnityEngine.UI;

public class ShopScreenIndicator : MonoBehaviour
{
    public Transform shopTarget;
    public Camera cam;
    public RectTransform canvasRect;
    public RectTransform arrowUI;
    public float edgeOffset = 50f;

    private Image arrowImage;

    void Awake()
    {
        arrowImage = arrowUI.GetComponent<Image>();
    }

    void Update()
    {
        if (shopTarget == null || cam == null || canvasRect == null || arrowUI == null)
        {
            if (arrowImage != null) arrowImage.enabled = false;
            return;
        }

        Vector3 screenPos = cam.WorldToScreenPoint(shopTarget.position);

        // 화면 밖 or 카메라 뒤
        bool isBehind = screenPos.z < 0f;
        if (isBehind)
        {
            // 뒤에 있으면 화면 좌표를 반대로
            screenPos.x = Screen.width - screenPos.x;
            screenPos.y = Screen.height - screenPos.y;
        }

        bool isInsideScreen = screenPos.x > 0 && screenPos.x < Screen.width &&
                              screenPos.y > 0 && screenPos.y < Screen.height && screenPos.z > 0;

        if (isInsideScreen)
        {
            if (arrowImage != null) arrowImage.enabled = false;
            return;
        }
        else
        {
            if (arrowImage != null) arrowImage.enabled = true;
        }

        // 화면 밖일 때: 화면 테두리(캔버스 엣지)에 위치
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 dir = ((Vector2)screenPos - screenCenter).normalized;

        // 캔버스 크기 얻기
        float canvasWidth = canvasRect.rect.width;
        float canvasHeight = canvasRect.rect.height;

        // 화면 경계 안에서 방향 벡터로 위치 결정 (UI 공간)
        float halfW = canvasWidth / 2f - edgeOffset;
        float halfH = canvasHeight / 2f - edgeOffset;

        // 캔버스 중앙 기준 방향
        Vector2 cappedPos = dir * Mathf.Min(
            Mathf.Abs(halfW / dir.x),
            Mathf.Abs(halfH / dir.y)
        );

        // anchoredPosition = 중앙(0,0) 기준, cappedPos
        arrowUI.anchoredPosition = cappedPos;

        // 각도 회전 (기본 이미지가 '→' 방향일 때, +90 추가)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        arrowUI.rotation = Quaternion.Euler(0, 0, angle);
    }
}
