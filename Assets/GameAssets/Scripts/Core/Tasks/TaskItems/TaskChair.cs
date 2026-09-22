using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskChair : MonoBehaviour, ITaskFinisher
{
    [SerializeField] private FinshingItemsUI[] taskCompletionThings;
    [SerializeField] private Image taskItemUI;
    [SerializeField] private TMP_Text requiredItemText;
    [SerializeField] private GameObject taskCompletionButton;
    [SerializeField] private int requiredItemIndex = 0;

    [SerializeField] private GameObject[] taskItems;
    [SerializeField] private RectTransform[] taskItemPositions;

    [Serializable]
    public class FinshingItemsUI
    {
        public string requiredItem;
        public Sprite completionUI;
    }

    public void OnEnable()
    {
        for(int i=0; i<taskItems.Length; i++)
        {
            taskItems[i].SetActive(true);
            taskItems[i].GetComponent<RectTransform>().position = taskItemPositions[i].position;
        }
        requiredItemIndex = 0;
        requiredItemText.text = "Require: " + taskCompletionThings[requiredItemIndex].requiredItem;
        taskCompletionButton.SetActive(false);
    }

    public void Start()
    {
        requiredItemText.text = "Require: " + taskCompletionThings[requiredItemIndex].requiredItem;
        taskCompletionButton.SetActive(false);
    }

    public void ItemIsInside(ITaskFinishItem item)
    {
        CheckIfRequiredItem(item);
    }

    private void CheckIfRequiredItem(ITaskFinishItem item)
    {
        if(requiredItemIndex >= taskCompletionThings.Length) return;

        if(item.itemName == taskCompletionThings[requiredItemIndex].requiredItem)
        {
            item.ItemTaken();
            taskItemUI.sprite = taskCompletionThings[requiredItemIndex].completionUI;
            requiredItemIndex++;

            if(requiredItemIndex >= taskCompletionThings.Length)
            {
                requiredItemText.gameObject.SetActive(false);
                taskCompletionButton.SetActive(true);
                return;
            }

            requiredItemText.text = "Require: " + taskCompletionThings[requiredItemIndex].requiredItem;
        }
    }
}