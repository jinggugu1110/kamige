using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;
    private bool isMouseOver = false;
    private GravityFlipPostIt gravityScript;

    void Start()
    {
        gravityScript = GetComponent<GravityFlipPostIt>(); // 重力制御スクリプトを取得
    }

    void OnMouseDown()
    {
        offset = transform.position - GetMouseWorldPosition();
        isDragging = true;
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPosition() + offset;
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
    }

    void Update()
    {
        //// `R` キーを押したら 90° 回転
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    RotateObject();
        //}
        // マウスが上にあるときだけRキーで回転
        if (isMouseOver && Input.GetKeyDown(KeyCode.R))
        {
            RotateObject();
        }
    }


    //マウスが触れているかどうかの処理
    private void OnMouseEnter()
    {
        isMouseOver = true;
    }

    private void OnMouseExit()
    {
        isMouseOver = false;
    }

    private void RotateObject()
    {
        transform.Rotate(0, 0, -90); // 時計回りに 90° 回転

        // 重力変更を `GravityFlipPostIt` に通知
        if (gravityScript != null)
        {
            gravityScript.UpdateGravityBasedOnRotation(transform.eulerAngles.z);
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}
