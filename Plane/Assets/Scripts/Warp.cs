using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
���[�v����X�N���v�g
������̃I�u�W�F�N�g�ɃA�^�b�`���Ă�������
�o���́A������̎q�I�u�W�F�N�g�ɂ��Ă�������
*/

public class Warp : MonoBehaviour
{
    Transform Warp_OutPoint;
    private bool GoalisinStage = false;//�o�����X�e�[�W���ɂ����true
    private float minX = -10f;
    private float maxX = 10f;
    private float minY = -10f;

    private void Start()
    {
        Warp_OutPoint = transform.childCount > 0 ? transform.GetChild(0) : null;
    }

    private void Update()
    {
        GoalisinStage = !(Warp_OutPoint.position.x < minX || Warp_OutPoint.position.x > maxX || Warp_OutPoint.position.y < minY);

    }

    private void OnTriggerStay2D(Collider2D other)
    {
        //���[�v�悪�X�e�[�W���Ȃ烏�[�v
        if (other.CompareTag("Player"))
        {
            if (Warp_OutPoint != null && GoalisinStage)
            {
                other.transform.position = Warp_OutPoint.position;
            }
            else
            {
                //�v���C���[���S
            }
        }
    }
    
}
