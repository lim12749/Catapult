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
        [ReadOnly] public bool isThrow = false; //������ ������ üũ��
        [ReadOnly] public bool isLanched = false; //���� �տ��� �߻簡 ����� ���߿� �ʱ�ȭ�� ����
        [SerializeField] private const float canonBallWeight = 1f; //Canonball Mass = Weight 
        [SerializeField] private const float DEFALT_LAUNCANGLE = 45;//�߻簢�� �⺻�� 
        [SerializeField] private const float DEFALT_LAUNCARMROTSPEED = 5f; //�߻簢 �ӵ� �⺻�� 
        [SerializeField] private const float DEFALT_CANONBOLLWEIGHT =10f; //���� �⺻ ����
        public CanonBall CanonBall; //�߻��ϴ�����ü
        public GameObject CatapultArm; //������Ʈ �� Transfrom���� �ص� ����
        public Transform LauncherVector;// ���� ��ġ�ϰ� �߻�� ��ġ

        [Range(250, 1000)] public int SpringK;
        [SerializeField] private float lanchSpeed; //Arm������ ���������� ��ȯ�ϴ� �ӵ�
        [SerializeField] private float CurrentArmAngle = 0f; //��ȯ ���� Ȯ�ο�
        [SerializeField] private float launcAngle; //���� ���� ����


        private void Awake()
        {
            
        }
        private void Update()
        {
            if (isThrow)
            {
                Debug.Log("������Ʈ ����");

                //������� �ִ� ������ ������ �������� �ٸ�������
                if (CurrentArmAngle >= launcAngle)
                {
                    isThrow = false; //����ִ� ���¸� ������
                    CanonBall.transform.rotation = Quaternion.identity; //ȸ���� ����
                    return;
                }
                //�� �ޱ۰��� ������Ŵ
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
            launcAngle = DEFALT_LAUNCANGLE; //�ʱⰪ ����
            CanonBall.CanonbollRigidbody.mass = DEFALT_CANONBOLLWEIGHT;
            CanonBall.transform.parent = CatapultArm.transform;//�θ� ��ü ���� ��������� 1��

            CanonBall.transform.position = LauncherVector.position; //������ ���۵Ǹ� �θ���ü�� ��ġ�� ����
            CanonBall.CanonbollRigidbody.useGravity = false;//�ö��ִµ��� �߷��� ������ �ȹް���       
        }
        
        public void ThrowButton()
        {
            isThrow = true;
            StartCoroutine(DoLuaunch());
        }
        IEnumerator DoLuaunch()
        {
            //���Ǻν� ���� ���ٴ� ������Ʈ ���� �츮�� �����ϵ� �Լ��� �̸��� ���� ��������� �����̴�.
            // (�Ű�����)=>(������ ��ȣ){statement;} 
            //waitwhile�� ������ �븮�ڰ� true�� �� �ɶ����� �ڷ�ƾ�� ������ �Ͻ� �ߴ� ��ŵ�ϴ�.
            // ���Ǻΰ� �ϼ��� ��� yield �Ʒ��� �ڵ尡 �����ϰԵȴ�. 
            yield return new WaitWhile(() => { return isThrow; }); 
            float _velocity = Velocity_At_Time_Of_Launch(); 

            ThrowBall(LauncherVector.up, _velocity);
        }
        // Force that is the combined Normal and Centrifugal force of the catapult spoon on the cannonball as the arm rises
        // Formula: (Nx+Cx, Ny+Cy)
        // ���� �ö� �� ź�� �������� ������ ��Ǭ�� �����°� ���ɷ��� ���� ��
        public float Velocity_At_Time_Of_Launch()
        {
            //�ӵ��� mathf.������((��������/����) * ����(45(ȣ��)*����,2))-(�߷� * ������(2f))
            float velocity = Mathf.Sqrt(((SpringK / CanonBall.CanonballMass) * Mathf.Pow((DEFALT_LAUNCANGLE * Mathf.Deg2Rad), 2)) - (Physics.gravity.y * Mathf.Sqrt(2f)));

            return velocity;
        }
        private void ThrowBall(Vector3 forceVector, float _velocity)
        {
            isLanched = true;
            CanonBall.transform.SetParent(null); //�ͼӵǾ��ִ� �θ� ��ü���� �и�
                                                 //������� ������ ������ ������� �� üũ�ڽ� �ɼ��� ���� ������ 
            CanonBall.CanonbollRigidbody.constraints = RigidbodyConstraints.None;

            //���󰥶��� ����
            CanonBall.CanonbollRigidbody.useGravity = true;

            //���� ���� ������ �����ݴϴ� (���� ����  * (�ӵ� * ���� ����), ���� �ִ� ���)
            CanonBall.CanonbollRigidbody.AddForce(forceVector * (_velocity * CanonBall.CanonballMass), ForceMode.Impulse);

        }
    }
}
