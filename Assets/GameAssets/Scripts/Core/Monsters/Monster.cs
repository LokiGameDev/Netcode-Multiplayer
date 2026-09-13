using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class Monster : NetworkBehaviour
{
    [Header("Monster Settings")]
    [Tooltip("Movement speed of the monster")]
    [SerializeField] private float monsterSpeed = 7.5f;
    [Tooltip("Detection range of the monster")]
    [SerializeField] private float detectingRange = 5f;
    [Tooltip("Monster attacking range")]
    [SerializeField] private float attackingRange = 2.5f;
    [SerializeField] private float resurrectTime = 2f;

    [Tooltip("Reloading time for attacking")]
    [SerializeField] private float reloadingTime = 1.5f;

    [Header("References")]
    [Tooltip("Monster animator for animations")]
    [SerializeField] private Animator monsterAnim;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private GameObject attackHitBox;

    private bool isDead = true;
    private bool isAttacking = false;

    private Transform currentTarget;
    private bool canAttack = true;

    /// <summary>
    /// Initializes the animator for the monster
    /// </summary>
    protected virtual void Start()
    {
        if(monsterAnim==null) monsterAnim = GetComponentInChildren<Animator>();
        if(navMeshAgent==null) navMeshAgent = GetComponent<NavMeshAgent>();

        navMeshAgent.speed = monsterSpeed;
        navMeshAgent.updateRotation = true;
        navMeshAgent.stoppingDistance = attackingRange;

        GetComponent<Collider>().enabled = false;
        attackHitBox.SetActive(false);

        StartCoroutine(ResurrectTime());
    }

    /// <summary>
    /// Function used to set the monster specifications after spawn
    /// </summary>
    protected virtual void SpawnMonster()
    {
        isDead = false;
    }

    /// <summary>
    /// Freezing the monster not to do anything
    /// </summary>
    protected virtual void FreezeMonster()
    {
        isDead = true;
    }

    /// <summary>
    /// Movement, Animations and target settings
    /// </summary>
    protected virtual void Update()
    {
        if(!IsHost) return;

        if(isDead) return;

        if(currentTarget == null) currentTarget = GetNearestTarget();

        if(currentTarget == null)
        {
            navMeshAgent.isStopped = true;
            monsterAnim.SetBool("Attacking", false);
            monsterAnim.SetFloat("Speed", 0);
            return;
        }

        float distance = Vector3.Distance(currentTarget.position, transform.position);

        navMeshAgent.isStopped = false;

        if(!currentTarget.GetComponent<PlayerManager>().IsAlive.Value)
        {
            currentTarget = null;
            return;
        }

        if(distance < attackingRange)
        {
            Attack();
        }
        else if(distance < detectingRange)
        {
            Follow();
        }
        else
        {
            navMeshAgent.isStopped = true;

            monsterAnim.SetFloat("Speed", 0);

            currentTarget = null;
        }
    }

    private void Attack()
    {
        if(!canAttack) return;

        isAttacking = true;

        navMeshAgent.SetDestination(currentTarget.position);

        monsterAnim.SetFloat("Speed", 0);
        monsterAnim.SetBool("Attacking", true);

        navMeshAgent.SetDestination(transform.position);

        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;
        
        canAttack = false;

        StartCoroutine(AttackReloadingTime());
    }

    private void Follow()
    {
        if(isAttacking) return;

        if(monsterAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack")) return;
        
        navMeshAgent.isStopped = false;

        navMeshAgent.SetDestination(currentTarget.position);

        monsterAnim.SetFloat("Speed", navMeshAgent.velocity.magnitude);
    }

    /// <summary>
    /// Getting the nearest player as target
    /// </summary>
    /// <returns>Nearest player transform component</returns>
    protected Transform GetNearestTarget()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        float minDistance = Mathf.Infinity;

        Transform target = null;
        
        foreach(GameObject player in players)
        {
            if(!player.GetComponent<PlayerManager>().IsAlive.Value) continue;

            float distance = Vector3.Distance(player.transform.position, transform.position);
            if(distance < detectingRange && distance < minDistance)
            {
                target = player.transform;
                minDistance = Vector3.Distance(player.transform.position, transform.position);
            }
        }

        return target;
    }

    public void AttackedPlayer(GameObject player)
    {
        if(!IsHost) return;
        
        Debug.Log($"Attacked player: {player.GetComponent<PlayerManager>().PlayerName.Value}");

        GameStateManager.Instance.AttackedPlayerRpc(player.GetComponent<NetworkObject>().OwnerClientId);
    }

    private IEnumerator ResurrectTime()
    {
        yield return new WaitForSeconds(resurrectTime);
        GetComponent<Collider>().enabled = true;
        isDead = false;
    }

    private IEnumerator AttackReloadingTime()
    {
        yield return new WaitForSeconds(reloadingTime/2);
        attackHitBox.SetActive(true);
        yield return new WaitForSeconds(reloadingTime/2);
        monsterAnim.SetBool("Attacking", false);
        attackHitBox.SetActive(false);
        canAttack = true;
        isAttacking = false;
    }
}
