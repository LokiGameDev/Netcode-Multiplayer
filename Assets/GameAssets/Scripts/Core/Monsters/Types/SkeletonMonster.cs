using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SkeletonMonster : Monster
{
    [SerializeField] protected GameObject attackHitBox;

    [SerializeField] private GameObject attackEffects;

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
        yield return new WaitForSeconds(reloadingTime/2);

        isAttackTirggered.Value = true;
        attackHitBox.SetActive(true);
        AudioManager.Instance.PlayAudioClientRpc(AudioID.SkeletonPunch, transform.position);

        yield return new WaitForSeconds(0.5f);
        attackHitBox.SetActive(false);

        yield return new WaitForSeconds((reloadingTime/2)-0.5f);
        isAttackTirggered.Value = false;
    }
}
