using UnityEngine;
using UnityEngine.Serialization;

namespace SimpleV
{
    [CreateAssetMenu(fileName = "TrackingParameter", menuName = "ScriptableObjects/TrackingParameter")]
    public class TrackingParameter : ScriptableObject
    {
        [Range(0.001f, 1f)] public float ikTrackingLerp = 0.5f;
        [Range(0.001f, 1f)] public float faceTrackingLerp = 0.5f;
        public AnimationCurve blinkCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        public AnimationCurve horizontalIrisCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        public AnimationCurve verticalIrisCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        public AnimationCurve mouseCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    }
}