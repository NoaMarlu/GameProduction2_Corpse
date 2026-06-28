using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Rendering;

public class Door : MonoBehaviour
{

    public List<Switch> linkSwitches = new List<Switch>();

    private int gridX;
    private int gridY;
    private bool isOpen = false;

    void Start()
    {
        //位置設定
        Vector2Int pos = GridManager.Instance.WorldToGrid(transform.position);
        gridX = pos.x;
        gridY = pos.y;
        transform.position = GridManager.Instance.GridToWorld(gridX, gridY);

        TurnManager.Instance.AddDoor(this);
        AddState(false);
    }

    //ドアの状態チェック
    public void CheckDoor()
    {
        bool isOpen = true;

        foreach(var linkSwitch in linkSwitches)
        {
            if (!linkSwitch.isOn)
            {
                isOpen = false;
                break;
            }
            if (linkSwitches.Count == 0) isOpen = false;
        }
        AddState(isOpen);

    }
    //オープンならセルの状態を変化
    void AddState(bool open)
    {
        isOpen = open;
        var cell = GridManager.Instance.GetCell(gridX, gridY);
        if (cell != null) cell.isWalk = open;
    }

}
