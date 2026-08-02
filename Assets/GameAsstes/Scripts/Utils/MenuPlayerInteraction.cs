using System.Collections;
using UnityEngine;

public class MenuPlayerInteraction : MonoBehaviour
{
    [SerializeField] private GameObject playerObject;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private TouchLookArea touchLookArea;
    private Coroutine randomActionCoroutine;

    public void OnEnable()
    {
        if(playerAnimator==null) playerAnimator = GetComponentInChildren<Animator>();

        randomActionCoroutine = StartCoroutine(PlayRandomAction());

        touchLookArea.OnLook += HandlePlayerDrag;
    }

    public void OnDisable()
    {
        touchLookArea.OnLook -= HandlePlayerDrag;
    }

    private IEnumerator PlayRandomAction()
    {
        yield return new WaitForSeconds(Random.Range(5,7));

        playerAnimator.SetInteger("Action", Random.Range(1,4));

        yield return new WaitForSeconds(Random.Range(5,7));

        playerAnimator.SetInteger("Action", 0);

        randomActionCoroutine = null;
    }

    private void Update()
    {
        if(randomActionCoroutine!=null) return;

        randomActionCoroutine = StartCoroutine(PlayRandomAction());
    }

    private void HandlePlayerDrag(Vector2 drag)
    {
        playerObject.transform.rotation *= Quaternion.Euler(0, -drag.x, 0);
    }
}
