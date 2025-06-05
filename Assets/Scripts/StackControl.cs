using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class Cell
{
    public Transform CellTransform;
    [HideInInspector]
    public int ObjectId;
    [HideInInspector]
    public GameObject ObjectInCell;
    [HideInInspector]
    public bool Ready;
}

public class StackControl : MonoBehaviour
{
    [SerializeField] Cell[] Cells;
    [SerializeField] float TravelTime;
    [SerializeField] AnimationCurve TravelCurve;
    [SerializeField] GameControl Game;

    int StackID = 0; //Курсор стека
    float t = 0;

    public static StackControl Instance;

    private void Awake()
    {
        Instance = this;

        for(int i = 0; i < Cells.Length; i++)
        {
            Cells[i].ObjectId = -1;
            Cells[i].Ready = false;
        }
    }

    public void ClearStack()
    {
        for (int i = 1; i < Cells.Length; i++)
        {
            Cells[i].ObjectId = -1;
            Cells[i].ObjectInCell = null;
            Cells[i].Ready = false;
        }
        StackID = 0;
    }

    public Vector3 AddObject(GameObject obj, int objectID)
    {
        Vector3 pos = Cells[StackID].CellTransform.position;
        Cells[StackID].ObjectId = objectID;
        Cells[StackID].ObjectInCell = obj;
        obj.GetComponent<ObjectControl>().HostingCellID = StackID;
        StackID++;
        return pos;
    }

    public void CheckState()
    {
        int counter = 0;
        int objID = -1;
        objID = Cells[0].ObjectId;

        for (int i = 1; i < Cells.Length; i++)
        {
            if (Cells[i].ObjectId > -1 && Cells[i].Ready)
            {
                if (objID == Cells[i].ObjectId)
                {
                    counter++;
                }
                else
                {
                    objID = Cells[i].ObjectId;
                    counter = 0;
                }
            }
            else
            {
                break;
            }

            if(counter == 2)
            {
                //COLLECT OBJECTS
                Collect(i);
                return;
            }
        }

        if(StackID == Cells.Length)
        {
            //GAME_OVER
            Game.ShowGameOver();
            return;
        }
    }

    public void Cellready(int CellID)
    {
        Cells[CellID].Ready = true;
    }

    void Collect(int CellID)
    {
        for(int i = CellID - 2; i < CellID + 1; i++)
        {
            Destroy(Cells[i].ObjectInCell);
            Cells[i].ObjectId = -1;
            Cells[i].Ready = false;
        }
        StackID = StackID - 3;
        GameControl.Instance.CheckGame();
    }
}
