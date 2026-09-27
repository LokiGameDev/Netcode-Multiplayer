using UnityEngine;

public class PoisonBall : MonoBehaviour
{
    [SerializeField] private GameObject UISprites;
    [SerializeField] private GameObject hitEffects;
    private bool isAttacked = false;
    private Monster monster;

    public void SetUp(Monster monster)
    {
        this.monster = monster;
    }
    private void Start()
    {
        hitEffects.SetActive(false);
        Destroy(gameObject, 4);
    }
    private void Update()
    {
        if(isAttacked) return;

        transform.position += transform.forward * Time.deltaTime * 10;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.CompareTag("Monsters") || collider.CompareTag("Tasks")) return;

        if(collider.CompareTag("Player") && !isAttacked)
        {
            if(monster!=null) monster.AttackedPlayer(collider.gameObject);
            AudioManager.Instance.Play(AudioID.PoisonBallHit);
            Debug.Log("Hit Player");
        }
        else
        {
            AudioManager.Instance.Play(AudioID.PoisonBallImpact);
            Debug.Log("Hit Something");
        }
        
        isAttacked = true;
        UISprites.SetActive(false);
        hitEffects.SetActive(true);
        Destroy(gameObject,1);
    }
}
