/**
Copyright 2020 Rodney Degracia

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.

 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MagicalLightAndSound
{
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(Rigidbody))]

    public class ActsAsController : MonoBehaviour
    {
        public bool enable6DOF = true;
        public bool enabledCFUIDTracking = true;

        private MLInput.Controller ActiveController
        {
            get
            {
                if (!MLInput.IsStarted)
                {
                    return null;
                }

                MLInput.Controller leftHandController = MLInput.GetController(MLInput.Hand.Left);
                MLInput.Controller rightHandController = MLInput.GetController(MLInput.Hand.Right);

                if (leftHandController != null)
                {
                    return leftHandController;
                }

                if (rightHandController != null)
                {
                    return rightHandController;
                }

                return null;
            }
        }

        public static MLInput.Controller.ControlType determineControllerType(byte controllerId)
        {
            //
            // The first controller is considered the left handed controller, even though
            // we may hold the controller in our right hand :)
            //
            MLInput.Controller leftHandController = MLInput.GetController(MLInput.Hand.Left);
            MLInput.Controller rightHandController = MLInput.GetController(MLInput.Hand.Right);

            bool isMobileAppLeft = (leftHandController != null && leftHandController.Type == MLInput.Controller.ControlType.MobileApp);
            bool isMobileAppRight = (rightHandController != null && rightHandController.Type == MLInput.Controller.ControlType.MobileApp);

            bool isControllerLeft = (leftHandController != null && leftHandController.Type == MLInput.Controller.ControlType.Control);
            bool isControllerRight = (rightHandController != null && rightHandController.Type == MLInput.Controller.ControlType.Control);

            bool isMobileApp = (isMobileAppLeft || isMobileAppRight);
            bool isController = (isControllerLeft || isControllerRight);

            if (isMobileApp)
            {
                return MLInput.Controller.ControlType.MobileApp;
            }
            if (isController)
            {
                return MLInput.Controller.ControlType.Control;
            }
            return MLInput.Controller.ControlType.None;
        }

        private void Awake()
        {
            Rigidbody rigidBody = GetComponent<Rigidbody>();
            rigidBody.isKinematic = true;

            gameObject.tag = "GameController";
        }

        private void Start()
        {
            if (!MLInput.IsStarted)
            {
                MLInput.Configuration configuration = new MLInput.Configuration(enable6DOF)
                {
                    EnableCFUIDTracking = enabledCFUIDTracking
                };

                MLResult inputStartResult = MLInput.Start(configuration);
                switch (inputStartResult.Result)
                {
                    case MLResult.Code.Ok:
                        break;
                    case MLResult.Code.InvalidParam:
                        throw new System.NotImplementedException();
                    case MLResult.Code.PrivilegeDenied:
                        throw new System.NotImplementedException();
                }
            }
        }

        private void OnDestroy()
        {
            if (MLInput.IsStarted)
            {
                MLInput.Stop();
            }
        }

    }
}


