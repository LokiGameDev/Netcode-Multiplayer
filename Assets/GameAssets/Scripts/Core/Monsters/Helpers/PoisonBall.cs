using UnityEngine;

public class PoisonBall : MonoBehaviour
{
    private bool isAttacked = false;
    private Monster monster;

    public void SetUp(Monster monster)
    {
        this.monster = monster;
    }
    private void Start()
    {
        Destroy(gameObject, 4);
    }
    private void Update()
    {
        transform.position += transform.forward * Time.deltaTime * 10;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.CompareTag("Monsters") || collider.CompareTag("Tasks")) return;

        if(collider.CompareTag("Player") && !isAttacked)
        {
            isAttacked = true;
            if(monster!=null) monster.AttackedPlayer(collider.gameObject);
            Debug.Log("Hit Player");
        }
        else
        {
            Debug.Log("Hit Something");
        }

        Destroy(gameObject);
    }
}
