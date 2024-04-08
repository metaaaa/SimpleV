using System;
using System.Collections;
using System.Collections.Generic;
using Mediapipe.Tasks.Vision.FaceLandmarker;
using Mediapipe.Tasks.Components.Containers;
using UnityEngine;

namespace SimpleV
{
    public class VrIkHandler
    {
        private readonly IkTargets _ikTargets = null;
        private readonly int _landmarkIndex = 0;
        private List<NormalizedLandmark> _faceLandmarks = null;
        private TrackingParameter _trackingParameter;

        private Quaternion _headRot = Quaternion.identity;

        public VrIkHandler(IkTargets ikTargets, TrackingParameter trackingParameter, int landmarkIndex = 0)
        {
            _ikTargets = ikTargets;
            _landmarkIndex = landmarkIndex;
            _trackingParameter = trackingParameter;
        }

        public void Update()
        {
            _ikTargets.Head.localRotation = Quaternion.Slerp(_ikTargets.Head.localRotation, _headRot,
                _trackingParameter.ikTrackingLerp);
        }

        public void FaceTrack(FaceLandmarkerResult faceLandmarkResult)
        {
            if (faceLandmarkResult.faceLandmarks == null) return;
            if (faceLandmarkResult.faceLandmarks.Count < _landmarkIndex) return;
            _faceLandmarks = faceLandmarkResult.faceLandmarks[_landmarkIndex].landmarks;

            _headRot = GetLookAtRotation();
        }

        private Quaternion GetLookAtRotation()
        {
            var faceVertical = (GetFaceLandmarkPosition(LandmarkDefinition.FaceLeft) -
                                GetFaceLandmarkPosition(LandmarkDefinition.FaceRight)).normalized;
            var faceHorizontal = (GetFaceLandmarkPosition(LandmarkDefinition.FaceBottom) -
                                  GetFaceLandmarkPosition(LandmarkDefinition.FaceTop)).normalized;

            var lookRollRad = Mathf.Acos(Vector2.Dot(Vector2.right, faceHorizontal)) - 90f * Mathf.Deg2Rad;
            var lookRoll = Quaternion.Euler(0, 0, lookRollRad * Mathf.Rad2Deg);
            var lookVec = Vector3.Cross(faceVertical.normalized, faceHorizontal.normalized);
            var lookQua = lookRoll * Quaternion.LookRotation(lookVec);
            return lookQua;
        }

        private Vector3 GetFaceLandmarkPosition(int index)
        {
            var landmark = _faceLandmarks[index];
            return new Vector3(landmark.x, landmark.y, landmark.z);
        }
    }
}