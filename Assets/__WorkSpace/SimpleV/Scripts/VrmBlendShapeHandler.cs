using System.Collections;
using System.Collections.Generic;
using Mediapipe.Tasks.Components.Containers;
using Mediapipe.Tasks.Vision.FaceLandmarker;
using UnityEngine;
using UniVRM10;

namespace SimpleV
{
    public class VrmBlendShapeHandler
    {
        private readonly Vrm10Instance _vrm = null;
        private readonly Vrm10RuntimeExpression _vrm10RuntimeExpression;
        private readonly Vrm10RuntimeLookAt _vrm10RuntimeLookAt;
        private readonly VRM10ObjectLookAt _vrm10ObjectLookAt;
        private readonly int _landmarkIndex = 0;
        private readonly TrackingParameter _trackingParameter;

        private float _mouseOpenWeight = 0f;
        private (float leftBlink, float rightBlink) _blinkWeights = (0f, 0f);
        private (float yaw, float pitch) _irisYawPitch = (0f, 0f);


        public VrmBlendShapeHandler(Vrm10Instance vrm, TrackingParameter trackingParameter, int landmarkIndex = 0)
        {
            _vrm = vrm;
            _vrm10RuntimeExpression = vrm.Runtime.Expression;
            _vrm10RuntimeLookAt = vrm.Runtime.LookAt;
            _vrm10ObjectLookAt = vrm.Vrm.LookAt;
            _landmarkIndex = landmarkIndex;
            _trackingParameter = trackingParameter;
        }

        public void Update()
        {
            var t = _trackingParameter.faceTrackingLerp;
            var mouseOpenWeight = Mathf.Lerp(_vrm10RuntimeExpression.GetWeight(ExpressionKey.Aa), _mouseOpenWeight, t);
            var leftBlinkWeight = Mathf.Lerp(_vrm10RuntimeExpression.GetWeight(ExpressionKey.BlinkLeft),
                _blinkWeights.leftBlink, t);
            var rightBlinkWeight = Mathf.Lerp(_vrm10RuntimeExpression.GetWeight(ExpressionKey.BlinkRight),
                _blinkWeights.rightBlink, t);
            var yaw = Mathf.Lerp(_vrm10RuntimeLookAt.Yaw, _irisYawPitch.yaw, t);
            var pitch = Mathf.Lerp(_vrm10RuntimeLookAt.Pitch, _irisYawPitch.pitch, t);
            
            _vrm10RuntimeExpression.SetWeight(ExpressionKey.Aa, mouseOpenWeight);
            _vrm10RuntimeExpression.SetWeight(ExpressionKey.BlinkLeft, leftBlinkWeight);
            _vrm10RuntimeExpression.SetWeight(ExpressionKey.BlinkRight, rightBlinkWeight);
            _vrm10RuntimeLookAt.SetYawPitchManually(yaw, pitch);
        }

        public void TrackExpressions(FaceLandmarkerResult faceLandmarkResult)
        {
            if (faceLandmarkResult.faceBlendshapes == null) return;
            if (faceLandmarkResult.faceBlendshapes.Count < _landmarkIndex) return;

            var faceBlendShapes = faceLandmarkResult.faceBlendshapes[_landmarkIndex].categories;

            _mouseOpenWeight = GetMouthOpenWeight(faceBlendShapes);
            _blinkWeights = GetBlinkWeight(faceBlendShapes);
            _irisYawPitch = GetIrisYawPitch(faceBlendShapes);
        }

        private float GetMouthOpenWeight(List<Category> faceBlendShapes)
        {
            var jawOpen = faceBlendShapes[BlendShapeDefinition.JawOpen].score;
            var mouseCurve = _trackingParameter.mouseCurve;

            return mouseCurve.Evaluate(jawOpen);
        }

        private (float, float) GetBlinkWeight(List<Category> faceBlendShapes)
        {
            var eyeBlinkLeft = faceBlendShapes[BlendShapeDefinition.EyeBlinkLeft].score;
            var eyeBlinkRight = faceBlendShapes[BlendShapeDefinition.EyeBlinkRight].score;
            var blinkCurve = _trackingParameter.blinkCurve;

            return (blinkCurve.Evaluate(eyeBlinkLeft), blinkCurve.Evaluate(eyeBlinkRight));
        }

        private (float, float) GetIrisYawPitch(List<Category> faceBlendShapes)
        {
            var eyeLookDownLeft = faceBlendShapes[BlendShapeDefinition.EyeLookDownLeft].score;
            var eyeLookDownRight = faceBlendShapes[BlendShapeDefinition.EyeLookDownRight].score;
            var eyeLookInLeft = faceBlendShapes[BlendShapeDefinition.EyeLookInLeft].score;
            var eyeLookInRight = faceBlendShapes[BlendShapeDefinition.EyeLookInRight].score;
            var eyeLookOutLeft = faceBlendShapes[BlendShapeDefinition.EyeLookOutLeft].score;
            var eyeLookOutRight = faceBlendShapes[BlendShapeDefinition.EyeLookOutRight].score;
            var eyeLookUpLeft = faceBlendShapes[BlendShapeDefinition.EyeLookUpLeft].score;
            var eyeLookUpRight = faceBlendShapes[BlendShapeDefinition.EyeLookUpRight].score;

            var horizontalIrisCurve = _trackingParameter.horizontalIrisCurve;
            var verticalIrisCurve = _trackingParameter.verticalIrisCurve;

            var horizontalOuter = _vrm10ObjectLookAt.HorizontalOuter;
            var horizontalInner = _vrm10ObjectLookAt.HorizontalInner;
            var verticalDown = _vrm10ObjectLookAt.VerticalDown;
            var verticalUp = _vrm10ObjectLookAt.VerticalUp;

            var lookLeft = (eyeLookInRight + eyeLookOutLeft) * 0.5f;
            var lookRight = (eyeLookInLeft + eyeLookOutRight) * 0.5f;
            var lookUp = (eyeLookUpLeft + eyeLookUpRight) * 0.5f;
            var lookDown = (eyeLookDownLeft + eyeLookDownRight) * 0.5f;

            var yaw = 0f;
            var pitch = 0f;

            var horizontalCurveXRangeDegree =
                (horizontalInner.CurveXRangeDegree + horizontalOuter.CurveXRangeDegree) * 0.5f;
            if (lookRight > lookLeft)
            {
                lookRight = horizontalIrisCurve.Evaluate(lookRight);
                yaw = Mathf.Lerp(0, horizontalCurveXRangeDegree, lookRight);
            }
            else
            {
                lookLeft = horizontalIrisCurve.Evaluate(lookLeft);
                yaw = Mathf.Lerp(0, horizontalCurveXRangeDegree * -1f, lookLeft);
            }

            if (lookUp > lookDown)
            {
                lookUp = verticalIrisCurve.Evaluate(lookUp);
                pitch = Mathf.Lerp(0, verticalUp.CurveXRangeDegree, lookUp);
            }
            else
            {
                lookDown = verticalIrisCurve.Evaluate(lookDown);
                pitch = Mathf.Lerp(0, verticalDown.CurveXRangeDegree * -1f, lookDown);
            }

            return (yaw, pitch);
        }
    }
}