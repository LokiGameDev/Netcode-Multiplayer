using System;
using System.Collections.Generic;
using System.Linq;
using GameTasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskLight : MonoBehaviour, ITaskFinisher
{
    [SerializeField] private FinshingItemsUI[] taskCompletionThings;
    [SerializeField] private Image taskItemUI;
    [SerializeField] private TMP_Text requiredItemText;
    [SerializeField] private GameObject taskCompletionButton;
    [SerializeField] private int requiredItemIndex = 0;
    [SerializeField] private Sprite defaultStartingSprite;

    [SerializeField] private RectTransform spawnArea;
    [SerializeField] private GameObject[] taskItems;
    [SerializeField] private RectTransform[] taskItemPositions;

    private List<RectTransform> taskItemsRect = new();

    [Serializable]
    public class FinshingItemsUI
    {
        public string requiredItem;
        public Sprite completionUI;
    }

    private void Awake()
    {
        foreach(var obj in taskItems)
        {
            taskItemsRect.Add(obj.GetComponent<RectTransform>());
        }
    }

    public void OnEnable()
    {
        for(int i=0; i<taskItems.Length; i++)
        {
            taskItems[i].SetActive(true);
            //taskItems[i].GetComponent<RectTransform>().position = taskItemPositions[i].position;
        }

        TaskUIHelperFunctions.PlaceObjects(taskItemsRect, spawnArea);
        requiredItemIndex = 0;
        taskItemUI.sprite = defaultStartingSprite;
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
