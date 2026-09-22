using UnityEngine;

public class TaskFinishItem : MonoBehaviour, ITaskFinishItem
{
    [SerializeField] private string FinshItemName;
    public string itemName { get { return FinshItemName;} }

    public void ItemTaken()
    {
        gameObject.SetActive(false);
    }
}
