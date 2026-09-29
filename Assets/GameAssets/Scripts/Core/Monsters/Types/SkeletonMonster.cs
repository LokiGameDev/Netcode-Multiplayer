using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SkeletonMonster : Monster
{
    [SerializeField] protected GameObject attackHitBox;

    [SerializeField] private GameObject attackEffects;
    [SerializeField] private float facingEnemySpeed = 180;

    NetworkVariable<bool> isAttackTirggered = new NetworkVariable<bool>
    (
        false,
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server
    );

    private Coroutine coroutine;

    private void OnEnable()
    {
        isAttackTirggered.OnValueChanged += MonsterAttacked;
    }

    private void OnDisable()
    {
        isAttackTirggered.OnValueChanged -= MonsterAttacked;
    }

    private void MonsterAttacked(bool previousValue, bool newValue)
    {   
        attackEffects.SetActive(newValue);
    }

    protected override void Start()
    {
        attackEffects.SetActive(false);

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
        if(currentTarget == null) yield return null;

        yield return new WaitForSeconds(reloadingTime/2);

        isAttackTirggered.Value = true;
        attackHitBox.SetActive(true);
        GameStateManager.Instance.PlayAudioClientRpc(AudioID.SkeletonPunch, transform.position);

        yield return new WaitForSeconds(0.5f);
        attackHitBox.SetActive(false);

        while (true)
        {
            if(currentTarget == null) break;
            
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


        yield return new WaitForSeconds(0.5f);
        isAttackTirggered.Value = false;
    }
}
