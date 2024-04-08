using System;
using System.Threading;
using Mediapipe.Tasks.Vision.FaceLandmarker;
using Unity.VisualScripting;
using UnityEngine;
using UniVRM10;

namespace SimpleV
{
    public class AvatarController : MonoBehaviour
    {
        [SerializeField] private IkTargets ikTargets;
        [SerializeField] private Vrm10Instance vrm;
        [SerializeField] private TrackingParameter trackingParameter;

        public float dbgFloat1 = 0f;
        public float dbgFloat2 = 0f;

        private VrIkHandler _ikHandler = null;
        private VrmBlendShapeHandler _expressionHandler = null;
        private SynchronizationContext _context = null;

        void Start()
        {
            _context = SynchronizationContext.Current;
            _ikHandler = new VrIkHandler(ikTargets, trackingParameter);
            _expressionHandler = new VrmBlendShapeHandler(vrm, trackingParameter);
        }

        private void Update()
        {
            _ikHandler.Update();
            _expressionHandler.Update();
        }

        public void Track(FaceLandmarkerResult faceLandmarkResult)
        {
            _context.Post(_ =>
            {
                _ikHandler.FaceTrack(faceLandmarkResult);
                _expressionHandler.TrackExpressions(faceLandmarkResult);
            }, null);
        }
    }
}