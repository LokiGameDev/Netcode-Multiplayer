using UnityEngine;

public class MonsterAttackHitBox : MonoBehaviour
{
    private bool isAttacked = false;

    private void OnEnable()
    {
        isAttacked = false;
    }

    public void OnTriggerEnter(Collider collider)
    {
        if(collider.CompareTag("Player") && !isAttacked)
        {
            isAttacked = true;
            GetComponentInParent<Monster>().AttackedPlayer(collider.gameObject);
        }
    }
}
