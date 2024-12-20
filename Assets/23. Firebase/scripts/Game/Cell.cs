using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using PED = UnityEngine.EventSystems.PointerEventData;

public class Cell : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public string coodinate;

    public bool isEmpty = true;

    [HideInInspector] public Board board;

    public Board.CellType cellType;

    public void OnPointerClick(PED eventData)
    {
        if(isEmpty || !board.isMyTurn)
        {
            board.SelectCell(this);
            isEmpty = false;
        }
    }

    public void OnPointerEnter(PED eventData)
    {
        
    }

    public void OnPointerExit(PED eventData)
    {
        
    }
}
