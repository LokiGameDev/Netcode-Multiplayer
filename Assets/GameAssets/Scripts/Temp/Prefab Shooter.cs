using System.Collections;
using UnityEngine;

public class PrefabShooter : MonoBehaviour
{
    public GameObject prefab;

    private void Start()
    {
        StartCoroutine(ShootAtInterval());
    }

    private IEnumerator ShootAtInterval()
    {
        while(true)
        {
            Instantiate(prefab, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(5);
        }
    }
}
