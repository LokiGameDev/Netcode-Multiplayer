using System.Collections;
using UnityEngine;

public class SkeletonMonster : Monster
{
    [SerializeField] protected GameObject attackHitBox;

    [SerializeField] private float facingEnemySpeed = 180;

    private Coroutine coroutine;

    protected override void Start()
    {
        base.Start();

        attackHitBox.SetActive(false);
    }

    protected override void Attack()
    {
        if(!canAttack) return;

        if(coroutine!=null) StopCoroutine(coroutine);

        coroutine = StartCoroutine(AttackHitboxTimer());

        base.Attack();
    }

    private IEnumerator AttackHitboxTimer()
    {
        yield return new WaitForSeconds(reloadingTime/2);
        
        while (true)
        {
            Vector3 direction = currentTarget.position - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.01f)
                break;

            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                facingEnemySpeed * Time.deltaTime
            );

            if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
            {
                transform.rotation = targetRotation;
                break;
            }

            yield return null;
        }

        attackHitBox.SetActive(true);
        yield return new WaitForSeconds(reloadingTime/2);
        attackHitBox.SetActive(false);
    }
}
