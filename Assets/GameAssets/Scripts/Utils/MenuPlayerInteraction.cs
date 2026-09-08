using System.Collections;
using UnityEngine;

public class MenuPlayerInteraction : MonoBehaviour
{
    [SerializeField] private GameObject playerObject;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private TouchLookArea touchLookArea;
    [SerializeField] private RuntimeAnimatorController animatorController;
    private Coroutine randomActionCoroutine;

    public void Initialize()
    {
        Debug.Log("Menu player intiated");

        if(playerAnimator==null) playerAnimator = GetComponentInChildren<Animator>();

        if(playerObject==null) playerObject = transform.GetChild(0).gameObject;

        playerAnimator.runtimeAnimatorController = animatorController;

        randomActionCoroutine = StartCoroutine(PlayRandomAction());

        touchLookArea.OnLook += HandlePlayerDrag;
    }

    public void OnDisable()
    {
        StopAllCoroutines();

        randomActionCoroutine = null;
    }

    public void OnDestroy()
    {
        StopAllCoroutines();

        randomActionCoroutine = null;

        touchLookArea.OnLook -= HandlePlayerDrag;
    }

    public void ResetSkin(GameObject currentSkin)
    {
        playerAnimator = currentSkin.GetComponent<Animator>();

        playerObject = currentSkin;

        playerAnimator.runtimeAnimatorController = animatorController;
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
