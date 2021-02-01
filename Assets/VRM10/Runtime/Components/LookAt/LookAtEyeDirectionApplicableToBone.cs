﻿using System.Collections.Generic;
using UnityEngine;

namespace UniVRM10
{
    internal sealed class LookAtEyeDirectionApplicableToBone : ILookAtEyeDirectionApplicable
    {
        private readonly Transform _leftEye;
        private readonly Transform _rightEye;
        private readonly CurveMapper _horizontalOuter;
        private readonly CurveMapper _horizontalInner;
        private readonly CurveMapper _verticalDown;
        private readonly CurveMapper _verticalUp;
        
        public LookAtEyeDirectionApplicableToBone(Transform leftEye, Transform rightEye,
            CurveMapper horizontalOuter, CurveMapper horizontalInner, CurveMapper verticalDown, CurveMapper verticalUp)
        {
            _leftEye = leftEye;
            _rightEye = rightEye;
            _horizontalOuter = horizontalOuter;
            _horizontalInner = horizontalInner;
            _verticalDown = verticalDown;
            _verticalUp = verticalUp;
        }
        
        /// <summary>
        /// LeftEyeボーンとRightEyeボーンに回転を適用する
        /// </summary>
        public IEnumerable<KeyValuePair<ExpressionKey, float>> Apply(LookAtEyeDirection eyeDirection)
        {
            // FIXME
            var yaw = eyeDirection.LeftYaw;
            var pitch = eyeDirection.LeftPitch;
            
            // horizontal
            float leftYaw, rightYaw;
            if (yaw < 0)
            {
                leftYaw = -_horizontalOuter.Map(-yaw);
                rightYaw = -_horizontalInner.Map(-yaw);
            }
            else
            {
                rightYaw = _horizontalOuter.Map(yaw);
                leftYaw = _horizontalInner.Map(yaw);
            }

            // vertical
            if (pitch < 0)
            {
                pitch = -_verticalDown.Map(-pitch);
            }
            else
            {
                pitch = _verticalUp.Map(pitch);
            }

            // Apply
            SetYawPitchToBones(new LookAtEyeDirection(leftYaw, pitch, rightYaw, pitch));
            
            yield break;
        }

        public void Restore()
        {
            SetYawPitchToBones(new LookAtEyeDirection(0, 0, 0, 0));
        }

        private void SetYawPitchToBones(LookAtEyeDirection actualEyeDirection)
        {
            if (_leftEye != null && _rightEye != null)
            {
                _leftEye.localRotation = Matrix4x4.identity.YawPitchRotation(actualEyeDirection.LeftYaw, actualEyeDirection.LeftPitch);
                _rightEye.localRotation = Matrix4x4.identity.YawPitchRotation(actualEyeDirection.RightYaw, actualEyeDirection.RightPitch);
            }
        }
    }
}