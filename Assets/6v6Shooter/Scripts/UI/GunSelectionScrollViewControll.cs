using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GunSelectionScrollViewControll : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector2 _startPosition;
    private Vector2 _dragStartPosition;
    private bool _isDragging;

    private RectTransform _contentRectTransform;
    private ScrollRect _scrollRect;

    void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();
        _contentRectTransform = _scrollRect.content;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _isDragging = true;
        _startPosition = _contentRectTransform.anchoredPosition;
        _dragStartPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_isDragging)
        {
            Vector2 currentDragPosition = eventData.position;
            Vector2 difference = currentDragPosition - _dragStartPosition;

            _contentRectTransform.anchoredPosition = _startPosition + new Vector2(difference.x, 0);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _isDragging = false;
    }
}
