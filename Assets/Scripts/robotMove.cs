using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class robotMove : MonoBehaviour
{
    [SerializeField] float gravity=9.81f;
    [SerializeField] float Speed=5.0f;
    [SerializeField] float mouseSensitivity=2.0f;//카메라 마우스 감도

    Transform myTransform;
    Transform model;

    private Animator robotAnimator;

    private CharacterController RobotController;
    //private Transform robotTransform;
    
    Vector3 mouseMove;
    Vector3 move;
    Transform cameraParentTransform;
    Transform cameraTransform;
    
    // Start is called before the first frame update
    void Start()
    {
        myTransform=transform;
        RobotController=GetComponent<CharacterController>();
        model=transform.GetChild(0);
        robotAnimator=this.GetComponent<Animator>();
        cameraTransform=Camera.main.transform;
        cameraParentTransform=cameraTransform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        float verticalInput=Input.GetAxis("Vertical");
        float horizontalInput=Input.GetAxis("Horizontal");
        robotAnimator.SetFloat("Speed",verticalInput);
        robotAnimator.SetFloat("Direction",horizontalInput);
        
        if(Input.GetKeyDown(KeyCode.Space))
        {
        robotAnimator.SetTrigger("Jump");
        }
  
        MoveCalc(1.0f);
        Balance();
        CameraDistanceCtrl();
    }
     void LateUpdate()
    {
        cameraParentTransform.position = myTransform.position + Vector3.up * 1.6f;  //캐릭터의 머리 높이쯤
        mouseMove += new Vector3(-Input.GetAxisRaw("Mouse Y") * mouseSensitivity, Input.GetAxisRaw("Mouse X") * mouseSensitivity, 0);   //마우스의 움직임을 가감
        if (mouseMove.x < -40)  //높이는 제한을 둔다. 슈팅 게임이라면 거의 90에 가깝게 두는게 좋을수도 있다.
            mouseMove.x = -40;
        else if (30 < mouseMove.x)
            mouseMove.x = 30;
        //여기서 헷갈리면 안 되는게 GetAxisRaw("Mouse XY") 는 실제 마우스의 움직임의 x좌표 y좌표를 가져오지만 회전은 축 기준이라 x가 위아래고 y가 좌우이다.
 
        cameraParentTransform.localEulerAngles = mouseMove;
    }
    
     void Balance()
    {
        if (myTransform.eulerAngles.x != 0 || myTransform.eulerAngles.z != 0)   //모종의 이유로 기울어진다면 바로잡는다.
            myTransform.eulerAngles = new Vector3(0, myTransform.eulerAngles.y, 0);
    }
    void CameraDistanceCtrl()
    {
        Camera.main.transform.localPosition += new Vector3(0, 0, Input.GetAxisRaw("Mouse ScrollWheel") * 1.0f); //휠로 카메라의 거리를 조절한다.
        if (-1 < Camera.main.transform.localPosition.z)
            Camera.main.transform.localPosition = new Vector3(Camera.main.transform.localPosition.x, Camera.main.transform.localPosition.y, -2);    //최대로 가까운 수치
        else if (Camera.main.transform.localPosition.z < -5)
            Camera.main.transform.localPosition = new Vector3(Camera.main.transform.localPosition.x, Camera.main.transform.localPosition.y, -8);    //최대로 먼 수치
    }
    void MoveCalc(float ratio)
    {

        float tempMoveY = move.y;
        move.y = 0; //이동에는 xz값만 필요하므로 잠시 저장하고 빼둔다.
        Vector3 inputMoveXZ = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //대각선 이동이 루트2 배의 속도를 갖는 것을 막기위해 속도가 1 이상 된다면 노말라이즈 후 속도를 곱해 어느 방향이든 항상 일정한 속도가 되게 한다. 
        float inputMoveXZMgnitude = inputMoveXZ.sqrMagnitude; //sqrMagnitude연산을 두 번 할 필요 없도록 따로 저장
        inputMoveXZ = myTransform.TransformDirection(inputMoveXZ);
        if (inputMoveXZMgnitude <= 1)
            inputMoveXZ *= Speed;
        else
            inputMoveXZ = inputMoveXZ.normalized * Speed;
 
        //조작 중에만 카메라의 방향에 상대적으로 캐릭터가 움직이도록 한다.
        if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
        {
            Quaternion cameraRotation = cameraParentTransform.rotation;
            cameraRotation.x = cameraRotation.z = 0;    //y축만 필요하므로 나머지 값은 0으로 바꾼다.
            //자연스러움을 위해 Slerp로 회전시킨다.
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, cameraRotation, 10.0f * Time.deltaTime);
            if (move != Vector3.zero)//Quaternion.LookRotation는 (0,0,0)이 들어가면 경고를 내므로 예외처리 해준다.
            {
                Quaternion characterRotation = Quaternion.LookRotation(move);
                characterRotation.x = characterRotation.z = 0;
                model.rotation = Quaternion.Slerp(model.rotation, characterRotation, 10.0f * Time.deltaTime);
            }
 
            //관성을 위해 MoveTowards를 활용하여 서서히 이동하도록 한다.
            move = Vector3.MoveTowards(move, inputMoveXZ, ratio * Speed);
        }
        else
        {
            //조작이 없으면 서서히 멈춘다.
            move = Vector3.MoveTowards(move, Vector3.zero, (1 - inputMoveXZMgnitude) * Speed * ratio);
        }


        
    }
}
