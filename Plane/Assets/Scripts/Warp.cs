using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ���[�v����X�N���v�g
   ������̃I�u�W�F�N�g�ɃA�^�b�`���Ă�������
   �o���́A������̎q�I�u�W�F�N�g�ɂ��Ă������� */
public class Warp : MonoBehaviour
{
    Transform Warp_OutPoint;
    private bool OutisinStage = false; //�o�����X�e�[�W���ɂ����true
    private float minX = -10f;
    private float maxX = 10f;
    private float minY = -10f;
    private HashSet<GameObject> WarpedList = new HashSet<GameObject>(); //���[�v�ς݃��X�g�BHashSet�́u�d�����Ȃ��v�f�v���i�[����B
    private float warpCooldown = 0.5f; // ���[�v�̃N�[���_�E������

    private void Start()
    {
        Warp_OutPoint = transform.childCount > 0 ? transform.GetChild(0) : null;
    }

    private void Update()
    {
        OutisinStage = !(Warp_OutPoint.position.x < minX || Warp_OutPoint.position.x > maxX || Warp_OutPoint.position.y < minY);

        // ����I��WarpedList���N���A����
        if (WarpedList.Count > 0)
        {
            StartCoroutine(ClearWarpedObject());
        }
    }

    private IEnumerator ClearWarpedObject()
    {
        yield return new WaitForSeconds(warpCooldown);
        WarpedList.Clear();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // ���Ƀ��[�v�����������I�u�W�F�N�g�Ȃ�X�L�b�v
        if (WarpedList.Contains(other.gameObject))
        {
            return;
        }

        if (other.CompareTag("Attachable")) //�����̃I�u�W�F�N�g
        {
            if (Warp_OutPoint != null && OutisinStage)
            {
                Transform root = FindRootObject(other.transform);//root = �ŏ�ʂ̐e
                WarpObject(root);
            }
            else
            {
                // �v���C���[���S����
            }
        }
        else if (other.CompareTag("PostIt")|| other.CompareTag("Player")) //�v���C���[�ӂ���
        {
�@          Transform root = FindRootObject(other.transform);
            if (root.CompareTag("Player")|| root.CompareTag("PostIt"))
            {
                if (Warp_OutPoint != null && OutisinStage)
                {
                    WarpObject(root);
                }
                else
                {
                    // �v���C���[���S����
                }
            }
            else
            {
                // PostIt�����ł����[�v
                //if (Warp_OutPoint != null && OutisinStage)
                //{
                //    WarpObject(other.transform);
                //}
            }
        }
        else if (other.CompareTag("Player")) // �v���C���[
        {
            if (Warp_OutPoint != null && OutisinStage)
            {
                WarpObject(other.transform);
            }
            else
            {
                // �v���C���[���S����
            }
        }
    }

    //�ŏ�ʂ̐e��������֐�
    private Transform FindRootObject(Transform obj)
    {
        Transform root = obj;
        while (root.parent != null)
        {
            root = root.parent;
        }
        return root;
    }

    //���[�v������֐�
    private void WarpObject(Transform obj)
    {
        obj.position = Warp_OutPoint.position;

        WarpedList.Add(obj.gameObject);
        foreach (Transform child in obj)
        {
            AddAllChildrenToWarpedList(child);
        }
    }

    //�q�I�u�W�F�N�g���ċA�I��WarpedList�ɒǉ�����
    private void AddAllChildrenToWarpedList(Transform obj)
    {
        WarpedList.Add(obj.gameObject);
        foreach (Transform child in obj)
        {
            AddAllChildrenToWarpedList(child);
        }
    }
}