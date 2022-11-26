using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GW
{
public class CanonBall : MonoBehaviour
{
        public Rigidbody CanonbollRigidbody;
        [SerializeField] private float mass = 30;
        public float CanonballMass
        {
            get
            {
                //값을 외부에 반환합니다.
                return mass;
            }
            set
            {
                mass = value;
            }
        }
        private void Awake()
        {
            Init();
        }
        private void Init()
        {

            CanonbollRigidbody = gameObject.GetComponent<Rigidbody>();
        }
    }
}
