using UnityEngine;

public interface ISupporterPattern
{
    void ActPattern(Transform transform, Transform player, SupporterSO supporterData);
}

public class TrumpetPattern : ISupporterPattern
{
    public void ActPattern(Transform transform, Transform player, SupporterSO supporterData)
    {
        GameObject[] activatingMonsters = GameObject.FindGameObjectsWithTag("Monster");
        float angleRange = 90f;     // ªÁ¿’∞¢

        foreach (GameObject monster in activatingMonsters)
        {
            Vector3 interV = monster.transform.position - transform.position;

            if (interV.magnitude <= supporterData.attackRange)
            {
                float dot = Vector2.Dot(interV.normalized, transform.forward);
                float theta = Mathf.Acos(dot);
                float degree = Mathf.Rad2Deg * theta;

                if (degree <= angleRange / 2f)
                {
                    monster.GetComponent<Monster>().TakeDamage(supporterData.attackDamage);
                }
            }
        }
    }
}

public class PianoPattern : ISupporterPattern
{
    public void ActPattern(Transform transform, Transform player, SupporterSO supporterData)
    {
        PlayerController playerCon = player.GetComponent<PlayerController>();
        playerCon.Heal(supporterData.attackDamage);
    }
}
