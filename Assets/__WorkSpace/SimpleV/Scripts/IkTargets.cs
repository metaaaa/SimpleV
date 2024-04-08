using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleV
{
    public class IkTargets : MonoBehaviour
    {
        [SerializeField] private Transform head = null;
        [SerializeField] private Transform leftHand = null;
        [SerializeField] private Transform rightHand = null;

        public Transform Head => head;
        public Transform RightHand => rightHand;
        public Transform LeftHand => leftHand;
        
    }
}
