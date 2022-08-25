using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Dragable : MonoBehaviour
{
    #region Exposed
    
    #endregion

    #region Unity Lifecycle

    void Awake()
    {
        
    }

    void Start()
    {
        _transform = this.transform;
        _startPosition = _transform.position;
    }

    void Update()
    {
        if (_isDragged) {
            _transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            if (!Input.GetMouseButton(0)) EndDrag();
        }
    }

    void FixedUpdate()
    {
        
    }

    #endregion

    #region Main methods
    public void BeginDrag() {
        _startPosition = _transform.localPosition;
        _isDragged = true;
        _transform.GetComponent<Image>().raycastTarget = false;
    }

    public void EndDrag() {
        _isDragged = false;
        _transform.GetComponent<Image>().raycastTarget = true;
        _transform.localPosition = _startPosition;
    }

    public void Drop(BaseEventData eventData) {
        GameObject draggedSticker = ((PointerEventData)eventData).pointerDrag;
        draggedSticker.transform.SetSiblingIndex(_transform.GetSiblingIndex());
    }

    #endregion

    #region Private & Protected
    private bool _isDragged;
    private Vector2 _startPosition;
    private Transform _transform;
    #endregion
}
