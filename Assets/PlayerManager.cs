using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Cinemachine;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{ 
    // Start is called before the first frame update
    public Vector2 inputVec;
    public float speed;

    public Rigidbody2D rigid;
    public SpriteRenderer sprit;
    public Animator animator;
    public PhotonView PV;
    public Text NickNameText;

    public int currnet_dir = -1;
    Vector2 curPos;

    void Awake()
    {


        NickNameText.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        NickNameText.color = PV.IsMine ? Color.green : Color.red;

        rigid = GetComponent<Rigidbody2D>();
        sprit = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if (PV.IsMine)
        {
            var CM = GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>();
            CM.Follow = transform;
            CM.LookAt = transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine)
        {
            inputVec.x = Input.GetAxisRaw("Horizontal");
            inputVec.y = Input.GetAxisRaw("Vertical");

            Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
            rigid.MovePosition(rigid.position + nextVec);

            animator.SetBool("is_move", inputVec.magnitude > 0);
            animator.SetFloat("x", inputVec.x);
            animator.SetFloat("y", inputVec.y);
            animator.SetFloat("Speed", inputVec.magnitude);

            if (inputVec.magnitude != 0)
            {
                sprit.flipX = inputVec.x < 0 ? true : false;
                if (sprit.flipX)
                {
                    currnet_dir = 0; // ¿ÞÂÊ
                }
                else
                {
                    currnet_dir = 1;
                }
            }

            PV.RPC("FlipXRPC", RpcTarget.Others, currnet_dir);
            if (Input.GetKeyDown(KeyCode.Space)) {
                PV.RPC("ActionRPC", RpcTarget.All, true);
            }
            else
            {
                PV.RPC("ActionRPC", RpcTarget.All, false);
            }
        }
    }

    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }



    [PunRPC]
    void ActionRPC(bool state)
    {
        if(state == true)
        {
            animator.SetTrigger("Action");
        }
        else
        {
            animator.SetTrigger("DeAction");
        }
    }

    [PunRPC]
    void FlipXRPC(int x)
    {
        if (x == -1) return;
        sprit.flipX = (x != 1);
    }





    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            curPos = (Vector2)stream.ReceiveNext();
        }
    }
}
