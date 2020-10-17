﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggeableWindow : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    [SerializeField] private RectTransform dragRectTransform;
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject contentPanel;

    private Color backgroundColor;

    private bool spawn = false;

    public RectTransform paperBin;
    public RectTransform blocksPanel;

    private Vector2 cellSize = new Vector2(45,45);

    private Vector2 lastPos = Vector2.zero;

    private bool collisionDetected;
    private bool checkedOnce;

    private void Awake()
    {
        dragRectTransform = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!spawn)
        {
            Instantiate(gameObject, panel.transform);
            spawn = true;
        }
        else
        {
            lastPos = dragRectTransform.anchoredPosition;
        }

        GetComponent<Image>().color = new Color(1, 1, 1, 0.6f);

        transform.SetParent(canvas.gameObject.transform);
        dragRectTransform.SetAsLastSibling();
        transform.localScale = contentPanel.transform.localScale;

        collisionDetected = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<Image>().color = new Color(1, 1, 1, 1f);

        if (UIUtility.GetWorldSpaceRect(paperBin).Contains(eventData.position, true))
        {
            Destroy(gameObject);
        }


        if (UIUtility.GetWorldSpaceRect(blocksPanel).Contains(eventData.position, true))
        {
            Destroy(gameObject);
        }

        transform.SetParent(contentPanel.transform);
        transform.localScale = Vector3.one;

        dragRectTransform.anchoredPosition = GridSystem.GetGridPosition(dragRectTransform.anchoredPosition);

        collisionDetected = true;
        checkedOnce = false;
        StartCoroutine(ResetCollision());
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collisionDetected && !checkedOnce ) {
            if (collision.GetComponent<DraggeableWindow>() != null || collision.GetComponent<SimpleDraggeable>() != null)
            {
                if(lastPos == Vector2.zero)
                {
                    Destroy(gameObject);
                }

                dragRectTransform.anchoredPosition = lastPos;
            }

            checkedOnce = true;
        }
    }

    IEnumerator ResetCollision()
    {
        yield return new WaitForSeconds(0.1f);

        checkedOnce = true;
    }
}
