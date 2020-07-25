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
using MagicLeap.Core;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.XR.MagicLeap.Native;

using System;

namespace MagicalLightAndSound
{
    [RequireComponent(typeof(MeshRenderer))]

    [RequireComponent(typeof(ActsAsManipulatable))]
    [RequireComponent(typeof(ActsAsCursorHoverable))]
    [RequireComponent(typeof(ActsAsSwipeable))]

    ///
    /// This script will anchor its GameObject to the closest PCF. It will draw 
    /// a line from the GameObject to the PCF, represented as a Sphere. 
    /// 
    /// For demonstration purposes, if the GameObject is moved by the 
    /// controller (ActsAsPickerManipulator), it will be re-anchored at its new location. Normally, once a world origin
    /// is anchored for a local map, it is not moved since Magic Leap will always make sure there is a PCF for that local map.
    ///
    /// You can use this script, with modifications, to represent the world origin of your App. It is considered best practice
    /// to anchor a single object to a PCF, and then set the position of your game objects relative to that
    /// single anchored object. It is _NOT_ considered best practice to anchor all your game objects to a PCF.
    ///
    public class ActsAsGlobalOrigin : MonoBehaviour, MLPersistentCoordinateFrames.PCF.IBinding
    {
        private TransformBinding transformBinding = null;
        private MLPersistentCoordinateFrames.PCF.Status status = MLPersistentCoordinateFrames.PCF.Status.Lost;
        private MeshRenderer meshRenderer = null;
        private MagicLeapNativeBindings.MLCoordinateFrameUID coordinateFrameUID = MagicLeapNativeBindings.MLCoordinateFrameUID.EmptyFrame;
        private MLResult.Code mapQualityResult = MLResult.Code.PassableWorldSharedWorldNotEnabled;
        private ActsAsManipulatable actsAsManipulatable = null;
        private MLPersistentCoordinateFrames.PCF pcf = null;
        private bool isMovedByPickerManipulator = false;

        [HideInInspector]
        public bool isAnchoredToPCF
        {
            get
            {
                return (this.transformBinding != null);
            }
        }

        [HideInInspector]
        public string Id
        {
            get
            {
                return this.Id;
            }
        }

        [HideInInspector]
        public MLPersistentCoordinateFrames.PCF PCF
        {
            get
            {
                return this.pcf;
            }
        }

        private Color colorOfStatus
        {
            get
            {
                switch (this.status)
                {
                    case MLPersistentCoordinateFrames.PCF.Status.Lost:
                        return Color.red;

                    case MLPersistentCoordinateFrames.PCF.Status.Created:
                        return Color.yellow;

                    case MLPersistentCoordinateFrames.PCF.Status.Updated:
                        return Color.yellow;

                    case MLPersistentCoordinateFrames.PCF.Status.Regained:
                        return Color.yellow;

                    case MLPersistentCoordinateFrames.PCF.Status.Stable:
                        return Color.green;

                    default:
                        return Color.yellow;

                }
            }
        }

        private Color colorOfMapQuality
        {
            get
            {
                switch (this.mapQualityResult)
                {
                    case MLResult.Code.Ok:
                        return Color.green;

                    case MLResult.Code.PassableWorldLowMapQuality:
                        return Color.gray;

                    case MLResult.Code.PassableWorldUnableToLocalize:
                        return Color.magenta;
                    default:
                        return Color.cyan;
                }
            }
        }

        public enum AnchorResult
        {
            PrimarySuccess,
            PrimaryFail,
            SecondarySuccess,
            SecondaryFail,
            BackupSuccess,
            BackupFail,
            GeneralSuccess,
            GeneralFail,
            PassableWorldLowMapQuality,
            PassableWorldUnableToLocalize
        }

        public enum AnchorAttempt
        {
            Primary,
            Secondary,
            Backup
        }

        private ActsAsGlobalOrigin.AnchorResult resultSuccessFrom(ActsAsGlobalOrigin.AnchorAttempt attempt)
        {
            switch (attempt)
            {
                case AnchorAttempt.Primary:
                    return ActsAsGlobalOrigin.AnchorResult.PrimarySuccess;
                case AnchorAttempt.Secondary:
                    return ActsAsGlobalOrigin.AnchorResult.SecondarySuccess;
                case AnchorAttempt.Backup:
                    return ActsAsGlobalOrigin.AnchorResult.BackupSuccess;
            }
            return ActsAsGlobalOrigin.AnchorResult.GeneralSuccess;
        }

        private ActsAsGlobalOrigin.AnchorResult resultFailFrom(ActsAsGlobalOrigin.AnchorAttempt attempt)
        {
            switch (attempt)
            {
                case AnchorAttempt.Primary:
                    return ActsAsGlobalOrigin.AnchorResult.PrimaryFail;
                case AnchorAttempt.Secondary:
                    return ActsAsGlobalOrigin.AnchorResult.SecondaryFail;
                case AnchorAttempt.Backup:
                    return ActsAsGlobalOrigin.AnchorResult.BackupFail;
            }
            return ActsAsGlobalOrigin.AnchorResult.GeneralFail;
        }

        // Start is called before the first frame update
        void Start()
        {
            this.meshRenderer = GetComponent<MeshRenderer>();
            if (this.meshRenderer == null)
            {
                print("Failed to get this.meshRenderer");
                return;
            }

            this.actsAsManipulatable = GetComponent<ActsAsManipulatable>();
            if (this.actsAsManipulatable == null)
            {
                print("Failed to get this.actsAsManipulatable");
                return;
            }

            ActsAsSwipeable actsAsSwipeable = GetComponent<ActsAsSwipeable>();
            if (actsAsSwipeable != null)
            {
                actsAsSwipeable.swipeLeftDelegate = OnSwipeLeft;
                actsAsSwipeable.swipeRightDelegate = OnSwipeRight;
            }

            if (!MLPersistentCoordinateFrames.IsStarted)
            {
                MLResult resultStart = MLPersistentCoordinateFrames.Start();
                switch (resultStart.Result)
                {
                    case MLResult.Code.Ok:
                        {
                            commonStart();
                        }
                        break;
                    case MLResult.Code.PrivilegeDenied:
                        throw new System.NotImplementedException();

                    case MLResult.Code.InvalidParam:
                        throw new System.NotImplementedException();

                    case MLResult.Code.UnspecifiedFailure:
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
            print("MLPersistentCoordinateFrames started");
            MLPersistentCoordinateFrames.OnLocalized += OnLocalized;

            ///
            /// Get events when the overall PCF Subsystem changes status
            /// 
            MLPersistentCoordinateFrames.PCF.OnStatusChange += onStatusChange;
        }

        private void OnSwipeRight(ActsAsSwipeable actsAsSwipeable)
        {
            transform.Rotate(new Vector3(0, 1), 1f);
        }

        private void OnSwipeLeft(ActsAsSwipeable actsAsSwipeable)
        {
            transform.Rotate(new Vector3(0, 1), -1f);
        }

        private void OnLocalized(bool localized)
        {
            // User has lost localization or regain localization
            if (!localized)
            {
                print("We have lost localization or are attempting to regain localization");
                return;
            }
            print("Localized");

            if (this.isAnchoredToPCF)
            {
                ///
                /// Re-queue the pcf for updates, since we regained map localization
                ///
                MLPersistentCoordinateFrames.QueueForUpdates(this.pcf);
                return;
            }


            if (!locateAndReBindToPCFAnchor())
            {
                createNewBindingToPCFAnchor();
            }
        }

        private void createNewBindingToPCFAnchor()
        {
            ///
            /// This GameObject is not anchored, search for a PCF that
            /// we can anchor to via binding to the found PCF. We first try the following order of PCFs
            ///
            /// For demonstration purposes, we will downgrade until we find a PCF.
            /// 
            /// Normally, we would choose the appropriate type of PCF
            /// 
            /// MultiUserMultiSession       - Shared PCF across Users and Sessions for a mapped area
            /// SingleUserMultiSession      - NonShared (local) PCF, multi-sessions for a a mapped area
            /// SingleUserSingleSession     - NonShared (local) PCF, single-session for a mapped area
            /// 
            ///

            ActsAsGlobalOrigin.AnchorResult try1 = anchorToClosestPCF(
                ActsAsGlobalOrigin.AnchorAttempt.Primary,
                MLPersistentCoordinateFrames.PCF.Types.MultiUserMultiSession
                );

            switch (try1)
            {
                case AnchorResult.GeneralSuccess:
                case AnchorResult.PrimarySuccess:
                    print("Primary/General Success");
                    return;

                default:
                    {
                        print("Primary/General Fail - Attempting downgrade to SingleUserMultiSession");
                        ActsAsGlobalOrigin.AnchorResult try2 = anchorToClosestPCF(
                            ActsAsGlobalOrigin.AnchorAttempt.Secondary,
                            MLPersistentCoordinateFrames.PCF.Types.SingleUserMultiSession
                            );
                        switch (try2)
                        {
                            case AnchorResult.GeneralSuccess:
                            case AnchorResult.SecondarySuccess:
                                print("Secondary/General Success");
                                return;
                            default:
                                {
                                    print("Primary/General Fail - Attempting downgrade to SingleUserSingleSession");
                                    ActsAsGlobalOrigin.AnchorResult try3 = anchorToClosestPCF(
                                        ActsAsGlobalOrigin.AnchorAttempt.Secondary,
                                        MLPersistentCoordinateFrames.PCF.Types.SingleUserSingleSession
                                        );
                                    switch (try2)
                                    {
                                        case AnchorResult.GeneralSuccess:
                                        case AnchorResult.SecondarySuccess:
                                            print("Backup/General Success");
                                            return;
                                        default:
                                            {
                                                print("No anchors found :( ");
                                                return;
                                            }
                                    }
                                }
                        }
                    }
            }
        }

        private bool locateAndReBindToPCFAnchor()
        {
            bool result = false;

            ///
            /// Iterate the persisted bindings that are stored locally on the device
            ///
            TransformBinding.storage.LoadFromFile();
            List<TransformBinding> storedBindings = TransformBinding.storage.Bindings;
            List<TransformBinding> staleBindings = new List<TransformBinding>();

            foreach (TransformBinding storedBinding in storedBindings)
            {
                ///
                /// Get the Coordinate Frame Unique Identifier
                ///
                this.coordinateFrameUID = storedBinding.PCF.CFUID;

                ///
                /// Find the PCF (Persistent Coordinate Frame) aka "The Anchor"
                ///
                MLPersistentCoordinateFrames.FindPCFByCFUID(coordinateFrameUID, out MLPersistentCoordinateFrames.PCF pcf);

                /// Gets the MLResult from the last query for the PCF's pose. It could be one of the following:
                /// <c>MLResult.Code.Pending</c> - 
                /// <c>MLResult.Code.Ok</c> - Position/Orientation is reliable.
                /// Otherwise - Position/Orientation is unreliable.
                switch (pcf.CurrentResultCode)
                {
                    case MLResult.Code.Pending: // Position/Orientation does not exist.
                        staleBindings.Add(storedBinding);
                        break;
                    case MLResult.Code.Ok:  // Position/Orientation is reliable, but pcf may be null
                        {
                            if (pcf == null)
                            {
                                staleBindings.Add(storedBinding);
                                continue;
                            }
                            else
                            {
                                print("Found stored PCF -- Rebinding");
                                this.transformBinding = storedBinding;
                                this.transformBinding.Bind(pcf, this.transform, true);
                                this.pcf = pcf;

                                result = true;
                            }
                        }
                        break;
                    default:    // Position/Orientation is unreliable.
                        {

                            staleBindings.Add(storedBinding);
                            continue;
                        }
                }
            }


            foreach (TransformBinding staleBinding in staleBindings)
            {
                staleBinding.UnBind();
            }

            if (!result)
            {
                print("Failed to locate stored binding.");
            }

            return result;
        }

        private ActsAsGlobalOrigin.AnchorResult anchorToClosestPCF(ActsAsGlobalOrigin.AnchorAttempt anchorAttempt, MLPersistentCoordinateFrames.PCF.Types type)
        {
            if (MLPersistentCoordinateFrames.IsLocalized == false)
            {
                print("Localized map must first be created before finding a PCF");
                return ActsAsGlobalOrigin.AnchorResult.GeneralFail;
            }

            MLResult resultFindClosestSecondaryPCFType = MLPersistentCoordinateFrames.FindClosestPCF(transform.position, out MLPersistentCoordinateFrames.PCF closestPCF, type);
            switch (resultFindClosestSecondaryPCFType.Result)
            {
                case MLResult.Code.Ok:
                    {
                        if (closestPCF == null)
                        {
                            return resultFailFrom(anchorAttempt);
                        }
                        else
                        {
                            ///
                            /// Bind this GameObject's transform to the PCF
                            /// 
                            this.transformBinding = new TransformBinding(this.GetInstanceID().ToString(), "ActsAsGlobalOrigin");
                            if (this.transformBinding == null)
                            {
                                return resultFailFrom(anchorAttempt);
                            }

                            if (this.transformBinding.Bind(closestPCF, transform))
                            {
                                closestPCF.AddBinding(this);
                                this.pcf = closestPCF;

                                print("Added Binding");

                                return resultSuccessFrom(anchorAttempt);
                            }
                            else
                            {
                                return resultFailFrom(anchorAttempt);
                            }

                        }
                    }

                case MLResult.Code.InvalidParam:
                    throw new System.NotImplementedException();

                case MLResult.Code.PrivilegeDenied:
                    throw new System.NotImplementedException();

                case MLResult.Code.UnspecifiedFailure:
                    print("UnspecifiedFailure");
                    return ActsAsGlobalOrigin.AnchorResult.GeneralFail;

                case MLResult.Code.PassableWorldLowMapQuality:
                    print("PassableWorldLowMapQuality - quality of the room map is too low, re-map room");
                    return ActsAsGlobalOrigin.AnchorResult.PassableWorldLowMapQuality;

                case MLResult.Code.PassableWorldUnableToLocalize:
                    print("PassableWorldUnableToLocalize - room layout has changed, re-map room or adjust lighting");
                    return ActsAsGlobalOrigin.AnchorResult.PassableWorldUnableToLocalize;
            }
            return ActsAsGlobalOrigin.AnchorResult.GeneralFail;
        }

        //
        // The status of the PCF subsystem and all PCFs for the localized map
        //
        private void onStatusChange(MLPersistentCoordinateFrames.PCF.Status pcfStatus, MLPersistentCoordinateFrames.PCF pcf)
        {
            //  Retain our status so we can change the color of the cube
            if (pcf.CFUID == this.coordinateFrameUID)
            {
                this.status = pcfStatus;
            }

            ///
            /// Comment out the code below to print out the status for information purposes
            /// There is no need to adjust the position or rotation of the GameObject, since the 
            /// TransformBinding will automatically adjust that for us.
            ///

            //switch (pcfStatus)
            //{
            //    case MLPersistentCoordinateFrames.PCF.Status.Lost:
            //        print("MLPersistentCoordinateFrames.PCF.Status.Lost");
            //        break;
            //    case MLPersistentCoordinateFrames.PCF.Status.Created:
            //        print("MLPersistentCoordinateFrames.PCF.Status.Created");
            //        break;
            //    case MLPersistentCoordinateFrames.PCF.Status.Updated:
            //        print("MLPersistentCoordinateFrames.PCF.Status.Updated");
            //        break;
            //    case MLPersistentCoordinateFrames.PCF.Status.Regained:
            //        print("MLPersistentCoordinateFrames.PCF.Status.Regained");
            //        break;
            //    case MLPersistentCoordinateFrames.PCF.Status.Stable:
            //        {
            //            print("MLPersistentCoordinateFrames.PCF.Status.Stable");
            //        }

            //        break;
            //    default:
            //        print("MLPersistentCoordinateFrames status unknown");
            //        break;
            //}

        }

        private void OnDestroy()
        {
            MLPersistentCoordinateFrames.OnLocalized -= OnLocalized;

            if (MLPersistentCoordinateFrames.IsStarted)
            {
                MLPersistentCoordinateFrames.Stop();
            }
        }

        private void OnDrawGizmos()
        {
            if (this.PCF == null)
            {
                return;
            }
            Gizmos.DrawLine(transform.position, this.PCF.Position);
            Gizmos.DrawSphere(this.PCF.Position, 0.1f);
        }

        // Update is called once per frame
        void Update()
        {
            //
            //  Normally, once we are anchored to a PCF, we should not un-anchor.
            //
            //  But for demonstration purposes, we allow this GameObject to be un-anchored
            //  and moved and re-anchored.
            //
            //  Remove anchor when the GameObject is being moved by 
            //  the ActsAsPickerManipulator
            //  and re-anchor when the GameObject has stopped being moved
            //  by the ActsAsPickerManipulator
            // 
            if (this.actsAsManipulatable.isBeingMoved && this.isAnchoredToPCF)
            {
                this.isMovedByPickerManipulator = true;
                this.transformBinding.UnBind();
                this.transformBinding = null;
                return;
            }
            else if (!this.isAnchoredToPCF && !this.actsAsManipulatable.isBeingMoved && isMovedByPickerManipulator)
            {
                this.isMovedByPickerManipulator = false;
                createNewBindingToPCFAnchor();
            }
            else if (isMovedByPickerManipulator)
            {
                return;
            }

            //
            // Color the GameObject according to map quality and PCF status
            //
            if (this.mapQualityResult != MLResult.Code.Ok)
            {
                this.meshRenderer.material.color = this.colorOfMapQuality;
            }
            else if (this.meshRenderer.material.color == this.colorOfStatus)
            {
                return;
            }
            this.meshRenderer.material.color = this.colorOfStatus;
        }

        //
        // The status of the PCF, directly from the PCF 
        //
        bool MLPersistentCoordinateFrames.PCF.IBinding.Update()
        {
            print("MLPersistentCoordinateFrames.PCF.IBinding.Update() - Updated by Magic Leap");
            return true;
        }

        bool MLPersistentCoordinateFrames.PCF.IBinding.Regain()
        {
            print("MLPersistentCoordinateFrames.PCF.IBinding.Regain");
            return true;
        }

        bool MLPersistentCoordinateFrames.PCF.IBinding.Lost()
        {
            print("MLPersistentCoordinateFrames.PCF.IBinding.Los");
            return true;
        }
    }
}


