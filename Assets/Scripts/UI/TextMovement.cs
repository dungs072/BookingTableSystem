using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TextMovement : MonoBehaviour
{
    public RectTransform rectTransform;
    [SerializeField] private float maxX = 1926;
    [SerializeField] private float minX = -1926;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private TMP_Text notification;
    private Vector2 initialPosition;
    private Vector2 targetPosition;

    private Queue<string> notificationQueue = new Queue<string>();
    private Coroutine runTextCoroutine;

    void Start()
    {
        rectTransform.anchoredPosition = new Vector2(initialPosition.x + maxX, initialPosition.y);
        initialPosition = rectTransform.anchoredPosition;
        targetPosition = new Vector2(minX, initialPosition.y);
    }

    private IEnumerator MoveToMaxX()
    {
        while (notificationQueue.Count > 0)
        {
            float value = Mathf.Abs(rectTransform.anchoredPosition.x - targetPosition.x);
            string notificationStr = notificationQueue.Dequeue();
            notification.text = notificationStr;
            while (value > 5f)
            {
                float x = rectTransform.anchoredPosition.x - moveSpeed * Time.deltaTime;
                rectTransform.anchoredPosition = new Vector2(x, rectTransform.anchoredPosition.y);
                value = Mathf.Abs(rectTransform.anchoredPosition.x - targetPosition.x);
                yield return null;
            }

            rectTransform.anchoredPosition = initialPosition;
        }
        runTextCoroutine = null;
    }
    public void EnqueueText(string notificationText)
    {
        notificationQueue.Enqueue(notificationText);
        if(runTextCoroutine==null)
        {
            runTextCoroutine = StartCoroutine(MoveToMaxX());
        }
    }
}
