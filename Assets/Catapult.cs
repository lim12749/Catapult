using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.WSA;

namespace GW
{
    public class Catapult : MonoBehaviour
    {
        [ReadOnly] public bool isThrow = false; //던지는 중인지 체크용
        [ReadOnly] public bool isLanched = false; //공이 손에서 발사가 됬는지 나중에 초기화를 위함
        [SerializeField] private const float canonBallWeight = 1f; //Canonball Mass = Weight 
        [SerializeField] private const float DEFALT_LAUNCANGLE = 45;//발사각의 기본값 
        [SerializeField] private const float DEFALT_LAUNCARMROTSPEED = 5f; //발사각 속도 기본값 
        [SerializeField] private const float DEFALT_CANONBOLLWEIGHT =10f; //공의 기본 무게
        public CanonBall CanonBall; //발사하는투사체
        public GameObject CatapultArm; //케터펄트 팔 Transfrom으로 해도 무관
        public Transform LauncherVector;// 공이 위치하고 발사될 위치

        [Range(250, 1000)] public int SpringK;
        [SerializeField] private float lanchSpeed; //Arm각도가 최종각까지 변환하는 속도
        [SerializeField] private float CurrentArmAngle = 0f; //변환 각도 확인용
        [SerializeField] private float launcAngle; //최종 도착 각도


        private void Awake()
        {
            
        }
        private void Update()
        {
            if (isThrow)
            {
                Debug.Log("업데이트 들어옴");

                //공을들고 있는 구간이 지정한 각도에다 다를때까지
                if (CurrentArmAngle >= launcAngle)
                {
                    isThrow = false; //들고있는 상태를 해제함
                    CanonBall.transform.rotation = Quaternion.identity; //회전값 없음
                    return;
                }
                //암 앵글각을 증가시킴
                CurrentArmAngle += (Time.deltaTime * DEFALT_LAUNCANGLE) * lanchSpeed;
                CatapultArm.transform.Rotate(-Vector3.up, (Time.deltaTime * DEFALT_LAUNCANGLE) * lanchSpeed);
            }
        }
        private void Start()
        {
            init();
        }

        public void init()
        {
            launcAngle = DEFALT_LAUNCANGLE; //초기값 셋팅
            CanonBall.CanonbollRigidbody.mass = DEFALT_CANONBOLLWEIGHT;
            CanonBall.transform.parent = CatapultArm.transform;//부모 객체 변경 여기까지가 1번

            CanonBall.transform.position = LauncherVector.position; //게임이 시작되면 부모겍체의 위치로 변경
            CanonBall.CanonbollRigidbody.useGravity = false;//올라가있는동안 중력의 영향을 안받게함       
        }
        
        public void ThrowButton()
        {
            isThrow = true;
            StartCoroutine(DoLuaunch());
        }
        IEnumerator DoLuaunch()
        {
            //조건부식 리턴 람다는 오브젝트 변수 우리가 선언하듯 함수의 이름이 없이 만들어지는 문법이다.
            // (매개변수)=>(시퀀스 기호){statement;} 
            //waitwhile은 제공된 대리자가 true로 평가 될때까지 코루틴의 실행을 일시 중단 시킵니다.
            // 조건부가 완성될 경우 yield 아래로 코드가 실행하게된다. 
            yield return new WaitWhile(() => { return isThrow; }); 
            float _velocity = Velocity_At_Time_Of_Launch(); 

            ThrowBall(LauncherVector.up, _velocity);
        }
        // Force that is the combined Normal and Centrifugal force of the catapult spoon on the cannonball as the arm rises
        // Formula: (Nx+Cx, Ny+Cy)
        // 팔이 올라갈 때 탄에 가해지는 투석기 스푼의 수직력과 원심력을 합한 힘
        public float Velocity_At_Time_Of_Launch()
        {
            //속도는 mathf.제곱근((스프링힘/질량) * 제곱(45(호도)*라디안,2))-(중력 * 제곱근(2f))
            float velocity = Mathf.Sqrt(((SpringK / CanonBall.CanonballMass) * Mathf.Pow((DEFALT_LAUNCANGLE * Mathf.Deg2Rad), 2)) - (Physics.gravity.y * Mathf.Sqrt(2f)));

            return velocity;
        }
        private void ThrowBall(Vector3 forceVector, float _velocity)
        {
            isLanched = true;
            CanonBall.transform.SetParent(null); //귀속되어있던 부모 객체에서 분리
                                                 //제약상태 프리즈 포지션 프리즈롯 등 체크박스 옵션을 전부 해제함 
            CanonBall.CanonbollRigidbody.constraints = RigidbodyConstraints.None;

            //날라갈때는 켜줌
            CanonBall.CanonbollRigidbody.useGravity = true;

            //이제 힘의 방향을 정해줍니다 (힘의 방향  * (속도 * 공의 질량), 힘을 주는 모드)
            CanonBall.CanonbollRigidbody.AddForce(forceVector * (_velocity * CanonBall.CanonballMass), ForceMode.Impulse);

        }
    }
}
