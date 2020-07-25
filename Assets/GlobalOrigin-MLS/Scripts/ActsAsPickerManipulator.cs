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
using UnityEngine.SceneManagement;

namespace MagicalLightAndSound
{
    /// <summary>
    /// Add to Controller game object
    /// </summary>
    /// 

    [RequireComponent(typeof(ActsAsController))]

    public class ActsAsPickerManipulator : MonoBehaviour
    {
        public LayerMask layerMask; // We hit-test against objects that have the UI layer mask
        public float maxDistance = 5;
        public GameObject cursor;

        private GameObject hoveredGameObject = null;
        private GameObject pickedGameObject = null;
        private bool isTriggerDown = false;
        private ActsAsCursorHoverable actsAsCursorHoverable = null;

        private MLRaycast.QueryParams queryParams = new MLRaycast.QueryParams();
        private Vector3 mlRaycastHitPoint;
        private Vector3 mlRaycastHitNormal;

        private ActsAsControllerRay actsAsControllerRay = null;
        private float distanceWhenPicked = 0;

        private void Awake()
        {
            this.actsAsControllerRay = GetComponent<ActsAsControllerRay>();
        }

        private void Start()
        {
            if (!MLInput.IsStarted)
            {
                bool enable6DOF = true;
                bool enabledCFUIDTracking = true;

                MLInput.Configuration configuration = new MLInput.Configuration(enable6DOF)
                {
                    EnableCFUIDTracking = enabledCFUIDTracking
                };

                MLResult inputStartResult = MLInput.Start(configuration);
                switch (inputStartResult.Result)
                {
                    case MLResult.Code.Ok:
                        commonStart();
                        break;
                    case MLResult.Code.InvalidParam:
                        throw new System.NotImplementedException();
                    case MLResult.Code.PrivilegeDenied:
                        throw new System.NotImplementedException();
                }
            }
            else
            {
                commonStart();
            }
        }

        private void commonStart()
        {
            MLInput.OnTriggerDown += MLInput_OnTriggerDown;
            MLInput.OnTriggerUp += MLInput_OnTriggerUp;

            if (!MLRaycast.IsStarted)
            {
                MLResult raycastStart = MLRaycast.Start();
                switch (raycastStart.Result)
                {
                    case MLResult.Code.Ok:
                        startRayCasting();
                        break;
                    case MLResult.Code.UnspecifiedFailure:
                        throw new System.NotImplementedException();
                    default:
                        break;
                }
            }
            else
            {
                startRayCasting();
            }
        }

        private void startRayCasting()
        {
            this.queryParams = new MLRaycast.QueryParams()
            {
                CollideWithUnobserved = false,
                Direction = transform.forward,
                Height = 1,
                HorizontalFovDegrees = 0,
                Position = transform.position,
                UpVector = transform.up,
                Width = 1
            };
            MLResult raycastResult = MLRaycast.Raycast(queryParams, OnRaycastResult);
            switch (raycastResult.Result)
            {
                case MLResult.Code.Ok:
                    //
                    break;
                case MLResult.Code.InvalidParam:
                    throw new System.NotImplementedException();
                case MLResult.Code.PrivilegeDenied:
                    throw new System.NotImplementedException();
            }
        }

        public void OnRaycastResult(MLRaycast.ResultState state, Vector3 hitpoint, Vector3 normal, float confidence)
        {

            if (MLRaycast.IsStarted)
            {
                switch (state)
                {
                    case MLRaycast.ResultState.RequestFailed:
                        // Do nothing
                        break;
                    case MLRaycast.ResultState.HitUnobserved:
                        break;
                    case MLRaycast.ResultState.NoCollision:
                        // World Mesh miss
                        mlRaycastHitNormal = -(transform.forward);
                        mlRaycastHitPoint = transform.position + (transform.forward * 10.0f);
                        this.actsAsControllerRay.startColor = Color.red;
                        this.actsAsControllerRay.endColor = Color.red;
                        break;
                    case MLRaycast.ResultState.HitObserved:
                        // World Mesh Hit
                        this.actsAsControllerRay.startColor = Color.green;
                        this.actsAsControllerRay.endColor = Color.green;
                        mlRaycastHitNormal = normal;
                        mlRaycastHitPoint = hitpoint;
                        break;
                    default:
                        break;
                }


                queryParams.Position = transform.position;
                queryParams.UpVector = transform.up;
                queryParams.Direction = transform.forward;

                MLRaycast.Raycast(queryParams, OnRaycastResult);
            }
        }

        private void MLInput_OnTriggerUp(byte controllerId, float triggerValue)
        {
            if (triggerValue > 0.2)
            {
                return;
            }
            if (this.pickedGameObject == null && isTriggerDown == false)
            {
                return;
            }

            ActsAsManipulatable actsAsManipulatable = this.pickedGameObject.GetComponent<ActsAsManipulatable>();
            if (actsAsManipulatable != null)
            {
                actsAsManipulatable.isBeingMoved = false;
            }

            isTriggerDown = false;
            this.pickedGameObject = null;
        }

        private void MLInput_OnTriggerDown(byte controllerId, float triggerValue)
        {
            if (triggerValue < 0.8)
            {
                return;
            }

            if (this.hoveredGameObject == null)
            {
                return;
            }


            ///
            /// Manipulatable
            /// 
            ActsAsManipulatable actsAsManiputable = this.hoveredGameObject.GetComponent<ActsAsManipulatable>();
            if (actsAsManiputable == null)
            {
                return;
            }

            ///
            /// Triggerdown clones immediately
            /// 
            if (actsAsManiputable.isClonable)
            {
                isTriggerDown = true;

                ///
                /// Clone the hovered Game Object, turn off hovering for the hovered game object
                ///
                this.pickedGameObject = Object.Instantiate(this.hoveredGameObject);
                this.actsAsCursorHoverable = hoveredGameObject.GetComponent<ActsAsCursorHoverable>();
                this.actsAsCursorHoverable.isCursorHovering = false;

                ///
                /// The picked Game Object is now the hovering game object
                ///
                this.hoveredGameObject = this.pickedGameObject;

                ActsAsManipulatable pickedActsAsManiputable = this.pickedGameObject.GetComponent<ActsAsManipulatable>();
                pickedActsAsManiputable.isClonable = false;
                pickedActsAsManiputable.isMovable = true;

                distanceWhenPicked = Vector3.Distance(transform.position, pickedGameObject.transform.position);
                return;
            }

            ///
            /// if not Clonable, then Triggerdown will pick 
            /// the hovered object for moving.
            /// 
            if (actsAsManiputable.isMovable)
            {
                if (this.hoveredGameObject == this.pickedGameObject && isTriggerDown == true)
                {
                    return;
                }

                isTriggerDown = true;
                this.pickedGameObject = this.hoveredGameObject;
                distanceWhenPicked = Vector3.Distance(transform.position, pickedGameObject.transform.position);
            }

            ///
            /// If triggerable, then Triggerdown 
            ///
            if (actsAsManiputable.isTriggerable)
            {
                actsAsManiputable.OnTrigger(controllerId, triggerValue);
            }
        }

        private void OnDestroy()
        {
            if (MLInput.IsStarted)
            {
                MLInput.OnTriggerDown -= MLInput_OnTriggerDown;
                MLInput.OnTriggerDown -= MLInput_OnTriggerUp;

                MLInput.Stop();
            }

            if (MLRaycast.IsStarted)
            {
                MLRaycast.Stop();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (MLInput.IsStarted)
            {


                ///
                /// Move
                /// 
                if (isTriggerDown)
                {
                    Vector3 cursorPosition = transform.position + (transform.forward * distanceWhenPicked);
                    cursor.transform.position = cursorPosition;
                    this.pickedGameObject.transform.position = cursorPosition;

                    ActsAsManipulatable actsAsManipulatable = this.pickedGameObject.GetComponent<ActsAsManipulatable>();
                    if (actsAsManipulatable != null)
                    {
                        actsAsManipulatable.isBeingMoved = true;
                    }

                    return;
                }

                ///
                /// Hover
                ///
                Vector3 origin = transform.position;
                Vector3 direction = transform.forward;
                Ray ray = new Ray(origin, direction);

                if (Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance, layerMask))
                {
                    Debug.DrawRay(ray.origin, ray.direction, Color.red);
                    Debug.Assert(hitInfo.rigidbody != null, "rigidbody should not be null");



                    this.hoveredGameObject = hitInfo.rigidbody.gameObject;
                    cursor.transform.position = this.hoveredGameObject.transform.position;

                    ///
                    /// Manipulatable and Hoverable
                    /// 
                    ActsAsManipulatable actsAsManiputable = this.hoveredGameObject.GetComponent<ActsAsManipulatable>();
                    if (actsAsManiputable == null || !actsAsManiputable.isHoverable)
                    {
                        return;
                    }

                    ///
                    /// Be sure to un-hover the previously hovered Game Object because they may be
                    /// parallel, in line withthe User
                    ///
                    if (this.actsAsCursorHoverable)
                    {
                        this.actsAsCursorHoverable.isCursorHovering = false;
                    }


                    this.actsAsCursorHoverable = hoveredGameObject.GetComponent<ActsAsCursorHoverable>();
                    this.actsAsCursorHoverable.isCursorHovering = true;
                }
                else
                {
                    // Let the MLRaycast.Raycast attempt to collide with the mesh
                    // in the meantime, re-position the cursor and the 
                    // ActsAsRay script will extend the Line of the LineRenderer
                    cursor.transform.position = mlRaycastHitPoint;
                    cursor.transform.LookAt(mlRaycastHitPoint + mlRaycastHitNormal);
                    cursor.transform.localScale = Vector3.one;

                    if (this.actsAsCursorHoverable == null)
                    {
                        return;
                    }

                    this.actsAsCursorHoverable.isCursorHovering = false;
                    this.hoveredGameObject = null;
                    this.actsAsCursorHoverable = null;
                }
            }
        }
    }
}



