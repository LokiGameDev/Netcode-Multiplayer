using UnityEngine;

public class SpringPad : MonoBehaviour
{
    [SerializeField] private float launchForce = 15f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = new Vector3(
                    rb.linearVelocity.x,
                    launchForce,
                    rb.linearVelocity.z
                );
            }
        }
    }
}