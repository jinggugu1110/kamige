using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

/*
�o�O
3���[�v�ɂӂꂽ�������

��邱��
2�����ۂɃt�Z����������s���ƐL�тċC��
1�s���ƐL�т�

*/

public class Enemy : MonoBehaviour
{
    [Header("�ړ��ݒ�")]
    public float moveSpeed = 2f; // �G�̈ړ����x

    [Header("�ǌ��m�ݒ�")]
    public Transform wallCheckPoint;         // Ray�̔��ˈʒu�i�G�̑O���ɔz�u�j
    public float wallCheckDistance = 0.5f;   // Ray�̋���
    public LayerMask wallLayer;              // �ǃ��C���[

    //�G�̑�
    private Rigidbody2D rb;
    private Collider2D HeadCol;//�����蔻�����������邽�߁A���Ƒ̂𕪂���

    private bool movingRight = true;//�i�ޕ���
    public int bodyCount = 4;
    public GameObject BodyPrefab;
    private List<GameObject> BodyParts = new List<GameObject>();
    private List<Vector3> PositionHistory = new List<Vector3>();
    public int Gap;

    public bool isTail = false; // �����ۂ��ǂ����t���O


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        HeadCol = GetComponent<Collider2D>(); // ������Collider�擾

        //body��1/3���d�Ȃ�
        float bodyWidth = BodyPrefab.transform.localScale.x;
        float stepPerFrame = moveSpeed * Time.fixedDeltaTime;
        Gap = Mathf.RoundToInt((bodyWidth * (2f / 3f)) / stepPerFrame);

        for (int i = 0; i < bodyCount; i++)
        {
            GrowBody();
        }
    }

    private void FixedUpdate()
    {
        Move();
        CheckWall();
    }

    private void Move()
    {
        float moveDir = movingRight ? 1f : -1f;
        rb.velocity = new Vector2(moveDir * moveSpeed, rb.velocity.y);

        //�V�����ʒu��ۑ�
        PositionHistory.Insert(0, transform.position);

        //Body�𓮂���
        int index = 0;
        foreach (var body in BodyParts)
        {
            Vector3 point = PositionHistory[Mathf.Min(index * Gap, PositionHistory.Count - 1)];
            body.transform.position = point;
            index++;
        }

    }

    // ======= �ǌ��m =======
    private void CheckWall()
    {
        Vector2 direction = movingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(wallCheckPoint.position, direction, wallCheckDistance, wallLayer);

        if (hit.collider != null)
        {
            Flip();
        }
    }

    // ======= �������] =======
    private void Flip()
    {
        movingRight = !movingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    // ======= �f�o�b�O���� =======
    private void OnDrawGizmos()
    {
        if (wallCheckPoint == null) return;

        Gizmos.color = Color.red;
        Vector3 direction = (movingRight ? Vector3.right : Vector3.left) * wallCheckDistance;
        Gizmos.DrawLine(wallCheckPoint.position, wallCheckPoint.position + direction);
    }

    // ======= �̂𐶐� =======
    private void GrowBody()
    {
        GameObject body = Instantiate(BodyPrefab);
        BodyParts.Add(body);//���X�g�ɒǉ�

        // �Փ˂𖳌����i�����̓��Ƃ̂݁j
        Collider2D bodyCol = body.GetComponent<Collider2D>();
        if (HeadCol != null && bodyCol != null)
        {
           // bodyCol.isTrigger = (BodyParts.Count == bodyCount);//()��������,�Ō������true�ɂ���
            Physics2D.IgnoreCollision(HeadCol, bodyCol);
        }
    }

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    //if (!enabled) return; // ���������͖���

    //    if (other.CompareTag("PostIt"))
    //    {
    //        // �Ō���� Body �������ꍇ�̂ݎ��s
    //        Collider2D tailCollider = BodyParts[BodyParts.Count - 1].GetComponent<Collider2D>();
    //        if (other == tailCollider)
    //        {
    //            Debug.Log("�����ۂɃt�Z�����G�ꂽ�I");
    //            // TODO: �s���ƐL�т鏈���E�C�⏈���Ȃ�
    //        }
    //    }
    //}

}
