//using UnityEngine;

//public class DraggableObject : MonoBehaviour
//{
//    private bool isDragging = false;
//    private Vector3 offset;

//    void OnMouseDown()
//    {
//        // マウス位置とオブジェクトの位置の差分を記録
//        offset = transform.position - GetMouseWorldPosition();
//        isDragging = true;
//    }

//    void OnMouseDrag()
//    {
//        if (isDragging)
//        {
//            transform.position = GetMouseWorldPosition() + offset;
//        }
//    }

//    void OnMouseUp()
//    {
//        isDragging = false;
//    }

//    private Vector3 GetMouseWorldPosition()
//    {
//        Vector3 mousePoint = Input.mousePosition;
//        mousePoint.z = Camera.main.WorldToScreenPoint(transform.position).z;
//        return Camera.main.ScreenToWorldPoint(mousePoint);
//    }
//}

using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;
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
        // `R` キーを押したら 90° 回転
        if (Input.GetKeyDown(KeyCode.R))
        {
            RotateObject();
        }
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
