using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class DragDropWindow : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{

    [SerializeField]
    Transform windowParent;

    Vector2 offset;

    void Start()
    {
        if (windowParent)
        {
            offset = this.transform.position - windowParent.position;
        }
        else
        {
            Debug.LogError("DragDropWindow needs assigned windowParent (Transform)");
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
		// Set ordering to top when dragging / clicking a window
		GetComponentInParent<Canvas>().sortingOrder = UICanvasSorting.sorting++;
	}



    public void OnDrag(PointerEventData eventData)
    {
        if (windowParent)
        {
            windowParent.position = eventData.position - offset;
        }
    }



    public void OnEndDrag(PointerEventData eventData)
    {


    }

	public void OnPointerClick(PointerEventData eventData)
	{
		// Set ordering to top when dragging / clicking a window
		GetComponentInParent<Canvas>().sortingOrder = UICanvasSorting.sorting++;
	}
}
