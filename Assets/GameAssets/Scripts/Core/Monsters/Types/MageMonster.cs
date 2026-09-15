using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class MageMonster : Monster
{
    [SerializeField] private PoisonBall posionBallPrefab;
    [SerializeField] private Transform attackPoint;

    [SerializeField] private float facingEnemySpeed = 180;

    private Coroutine coroutine;

    protected override void Attack()
    {
        if(!canAttack) return;

        if(coroutine!=null) StopCoroutine(coroutine);

        coroutine = StartCoroutine(ShootThePoison());

        base.Attack();
    }

    private IEnumerator ShootThePoison()
    {   
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

        PoisonBall ball = Instantiate(posionBallPrefab, attackPoint.position, Quaternion.identity);
        ball.SetUp(this);
        ball.gameObject.transform.LookAt(new Vector3(currentTarget.position.x, ball.transform.position.y, currentTarget.position.z));

        NetworkObject obj = ball.GetComponent<NetworkObject>();
        obj.Spawn();

        coroutine = null;
    }
}
