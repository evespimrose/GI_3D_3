using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Cell[] cells;
    public Dictionary<string, Cell> CellDictionary = new Dictionary<string, Cell>();

    public GameObject blueMarkPrefab;
    public GameObject redMarkPrefab;

    public bool isHost;
    public int turnCount;
    public bool isMyTurn;

    public enum CellType
    {
        None,
        Blue,
        Red
    };

    private void Awake()
    {
        cells = GetComponentsInChildren<Cell>();

        int cellNum = 0;
        for(int y = 0; y < 8; ++y)
        {
            for(int x = 0; x < 8; ++x)
            {
                cells[cellNum].coodinate = $"{(char)(x + 65)}{y + 1}";
                cellNum++;
            }
        }

        foreach(Cell cell in cells)
        {
            cell.board = this;
            cell.cellType = CellType.None;
            CellDictionary.Add(cell.coodinate, cell);
        }
    }

    public void PlaceMark(bool isBlue, string coodinate)
    {
        GameObject prefab = isBlue? blueMarkPrefab : redMarkPrefab;
        Cell targetCell = CellDictionary[coodinate];

        targetCell.cellType = isBlue ? CellType.Blue : CellType.Red;

        Instantiate(prefab, targetCell.transform, false);
    }

    public void SelectCell(Cell cell)
    {
        Turn turn = new Turn()
        {
            isHostTurn = isHost,
            coodinate = cell.coodinate
        };

        FirebaseManager.Instance.SendTurn(turnCount, turn);
    }

    public bool isGameOver(bool isMyTurn)
    {
        bool result = false;

        // 로직 구현

        return result;
    }
}
