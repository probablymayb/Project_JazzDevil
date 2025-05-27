using System.Collections;
using UnityEngine;

public class BossMonster : Monster
{
    private Transform circleRange;
    private Transform circleAttack;

    protected override void Start()
    {
        base.Start();
        AttackPattern = new BossPattern();
        circleRange = transform.Find("Circle Range");
        circleAttack = transform.Find("Circle Attack");
    }

    protected override void Attack()
    {
        if (!isActiveAndEnabled) return;

        // 플레이어와의 거리
        float distance = Vector3.Distance(transform.position, player.position);

        // 공격 범위에 들어오면
        if (distance <= monsterData.attackRange)
        {
            animator.SetBool("isWindup", true); // 공격 준비 모션

            // 공격 범위 표시하기
            if (circleRange == null)
            {
                Debug.LogWarning("Circle Range를 찾지 못함.");
            }
            circleRange.gameObject.SetActive(true);

            // 공격 오브젝트 크기 증가
            StartCoroutine(CircleAttackWindup(circleAttack));

            windupTimer++;

            if (windupTimer > monsterData.attackWindup)
            {
                AttackPattern?.AttackPattern(transform, player, animator, monsterData);
                windupTimer = 0; // 공격 후 타이머 초기화
                circleRange.gameObject.SetActive(false); // 공격 범위 숨기기
                circleAttack.localScale = Vector3.zero; // 공격 오브젝트 초기화
            }
        }
        else
        {
            windupTimer = 0; // 범위 벗어나면 타이머 초기화
            circleRange.gameObject.SetActive(false); // 공격 범위 숨기기
            circleAttack.localScale = Vector3.zero; // 공격 오브젝트 초기화
            animator.SetBool("isWindup", false);
            animator.SetBool("isAttack", false);
        }
    }

    // 공격 원이 점점 커지는
    private IEnumerator CircleAttackWindup(Transform circleAttack)
    {
        Vector3 startScale = Vector3.zero;
        Vector3 targetScale = Vector3.one * 5f;
        float elapsedTime = 0f;
        float duration = 60f / RhythmManager.Instance.CurrentBpm;

        while (elapsedTime < duration)
        {
            if (this == null) yield break;

            float t = elapsedTime / (duration * monsterData.attackWindup);
            circleAttack.localScale = Vector3.Lerp(startScale, targetScale, t);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        circleAttack.localScale = targetScale;
    }
}
