using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    [Tooltip("Зона генерации объектов игры")]
    [SerializeField] BoxCollider2D GenerationArea;
    [Tooltip("Список игровых объектов")]
    [SerializeField] ObjectsSet Objects;
    [Tooltip("Размер ячейки сетки генерации")]
    [SerializeField] float CellSize;
    [SerializeField] GameObject Window_GameOver;
    [SerializeField] GameObject Window_Victory;

    //Список объектов на игровом поле
    List<GameObject> objectsInGame;

    //Начало зоны генерации
    Vector2 GenOrigin;
    //Сетка генерации объетов на уровне
    float[,] GenField;

    List<Vector2Int> GenFieldIds;

    int CellsCount = 0;
    int cols, rows;
    int obj_count;

    static public GameControl Instance;
    public bool GameOver;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Window_GameOver.SetActive(false);
        Window_Victory.SetActive(false);

        GenOrigin = GenerationArea.bounds.min;

        cols = Mathf.FloorToInt(Mathf.Abs(GenerationArea.bounds.min.x - GenerationArea.bounds.max.x) / CellSize);
        rows = Mathf.FloorToInt(Mathf.Abs(GenerationArea.bounds.min.y - GenerationArea.bounds.max.y) / CellSize);

        CellsCount = (int)(cols * rows/3f) * 3;
        GenField = new float[rows, cols];

        objectsInGame = new List<GameObject>();
        GenFieldIds = new List<Vector2Int>();
        objectsInGame.Capacity = CellsCount;
        GenFieldIds.Capacity = CellsCount;

        SetObjects(CellsCount);
    }

    public void CheckGame()
    {
        StartCoroutine(Check());
    }

    IEnumerator Check()
    {
        yield return null;
        obj_count = 0;
        for (int i = 0; i < objectsInGame.Count; i++)
        {
            if (objectsInGame[i] != null)
            {
                obj_count++;
            }
        }
        if (obj_count == 0)
        {
            ShowWin();
        }
    }

    public void RemoveObject(int ID)
    {
        objectsInGame[ID] = null;
    }

    public void ShowGameOver()
    {
        GameOver = true;
        Window_GameOver.SetActive(true);
    }

    public void ShowWin()
    {
        Window_Victory.SetActive(true);
    }

    void SetObjects(int number)
    {
        int counter = 0;
        int objID = Random.Range(0, Objects.PlayObjects.Length);
        int cellId;
        Vector2 pos = new Vector2(0, 0);
        GameObject playObject;

        GameOver = false;

        GenFieldIds.Clear();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                GenFieldIds.Add(new Vector2Int(i, j));
            }
        }

        for (int i = 0; i < number; i++)
        {
            cellId = Random.Range(0, GenFieldIds.Count);
            pos = CellPosition(GenFieldIds[cellId].x, GenFieldIds[cellId].y);

            //В идеале так не дклать, а реализовать пул объектов для заполнения уровня.
            playObject = Instantiate(Objects.PlayObjects[objID]);
            playObject.transform.position = pos;
            playObject.GetComponent<ObjectControl>().ObjTypeID = objID;
            playObject.GetComponent<ObjectControl>().ObjID = i;

            objectsInGame.Add(playObject);

            GenFieldIds.RemoveAt(cellId);

            counter++;
            if (counter == 3)
            {
                objID = Random.Range(0, Objects.PlayObjects.Length);
                counter = 0;
            }
        }
    }

    public void RestartGame()
    {
        StartCoroutine(ResetGame());
    }

    public void ResetField()
    {
        
        StartCoroutine(ClearField());
    }

    IEnumerator ResetGame()
    {
        int obj_id;

        while (objectsInGame.Count > 0)
        {
            obj_id = Random.Range(0, objectsInGame.Count);
            Destroy(objectsInGame[obj_id].gameObject);
            objectsInGame.RemoveAt(obj_id);

            yield return new WaitForSeconds(.02f);
        }
        SetObjects(CellsCount);

        StackControl.Instance.ClearStack();
        Window_GameOver.SetActive(false);
        Window_Victory.SetActive(false);
    }

    IEnumerator ClearField()
    {
        int obj_id;

        obj_count = 0;

        while (objectsInGame.Count > 0)
        {
            obj_id = Random.Range(0, objectsInGame.Count);
            if (objectsInGame[obj_id] != null)
            {
                obj_count++;
                Destroy(objectsInGame[obj_id].gameObject);
            }
            objectsInGame.RemoveAt(obj_id);

            yield return new WaitForSeconds(.02f);
        }
        StackControl.Instance.ClearStack();
        SetObjects(obj_count);
    }

    Vector2 CellPosition(int row, int col)
    {
        Vector2 pos = new Vector2(GenOrigin.x + col * CellSize, GenOrigin.y + row * CellSize);
        return pos;
    }
}
