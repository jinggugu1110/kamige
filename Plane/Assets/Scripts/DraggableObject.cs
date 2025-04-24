using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;
    private bool isMouseOver = false;
    private GravityFlipPostIt gravityScript;

    void Start()
    {
        gravityScript = GetComponent<GravityFlipPostIt>(); // �d�͐���X�N���v�g���擾
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
        //// `R` �L�[���������� 90�� ��]
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    RotateObject();
        //}
        // �}�E�X����ɂ���Ƃ�����R�L�[�ŉ�]
        if (isMouseOver && Input.GetKeyDown(KeyCode.R))
        {
            RotateObject();
        }
    }


    //�}�E�X���G��Ă��邩�ǂ����̏���
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
        transform.Rotate(0, 0, -90); // ���v���� 90�� ��]

        // �d�͕ύX�� `GravityFlipPostIt` �ɒʒm
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
