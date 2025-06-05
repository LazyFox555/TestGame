using UnityEngine;
using System.Collections;

public class ObjectControl : MonoBehaviour
{
    [SerializeField] SpriteRenderer MainSprite;
    [SerializeField] Renderer tmp_Digit;
    [SerializeField] Rigidbody2D RigidBody;
    [SerializeField] float TravelTime;
    [SerializeField] AnimationCurve TravelCurve;

    public int HostingCellID = -1;

    public int ObjTypeID; //ID обейкта в списке игровых обектов
    public int ObjID; //ID обекта на игровом поле
    float t = 0;
    bool Clicked = false;

    void OnMouseDown()
    {
        if (GameControl.Instance.GameOver) return;
        if (Clicked) return;
        Clicked = true;
        Vector3 pos;
        MainSprite.sortingOrder = 1;
        tmp_Digit.sortingOrder = 1;
        MainSprite.gameObject.layer = 0;
        RigidBody.bodyType = RigidbodyType2D.Kinematic;
        pos = StackControl.Instance.AddObject(gameObject, ObjTypeID);
        StartCoroutine(MoveObject(pos));
    }

    private void Update()
    {
        RigidBody.linearVelocity = Vector2.ClampMagnitude(RigidBody.linearVelocity, 100);
    }

    IEnumerator MoveObject(Vector3 new_Pos)
    {
        Vector3 init_pos = gameObject.transform.position;
        Quaternion init_rot = gameObject.transform.rotation;
        t = 0;

        RigidBody.bodyType = RigidbodyType2D.Static;

        while (t < 1)
        {
            t += Time.deltaTime / TravelTime;
            gameObject.transform.position = Vector3.Lerp(init_pos, new_Pos, TravelCurve.Evaluate(t));
            gameObject.transform.rotation = Quaternion.Lerp(init_rot, Quaternion.Euler(Vector2.zero), TravelCurve.Evaluate(t));
            yield return null;
        }

        StackControl.Instance.Cellready(HostingCellID);
        StackControl.Instance.CheckState();
    }
}
