using UnityEngine;

public class KontraBass : Supporter
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            Monster monster = other.GetComponent<Monster>();
            if (monster != null)
            {
                monster.AdjustSpeed(supporterData.attackDamage / 100); // (attackDamage)% 속도로 조정
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Mosnter"))
        {
            Monster monster = other.GetComponent<Monster>();
            if (monster != null)
            {
                monster.ResetSpeed();
            }
        }
    }
}
