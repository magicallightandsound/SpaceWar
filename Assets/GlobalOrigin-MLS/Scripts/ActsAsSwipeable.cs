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

    public class ActsAsSwipeable : MonoBehaviour
    {
        public delegate void SwipeLeftDelegate(ActsAsSwipeable actsAsSwipeable);
        public SwipeLeftDelegate swipeLeftDelegate;

        public delegate void SwipeRightDelegate(ActsAsSwipeable actsAsSwipeable);
        public SwipeRightDelegate swipeRightDelegate;

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

        // Start is called before the first frame update
        void Start()
        {
            if (!MLInput.IsStarted)
            {
                MLResult inputStartResult = MLInput.Start();
                switch (inputStartResult.Result)
                {
                    case MLResult.Code.Ok:
                        {
                            print("ActsAsSwipeable -- started");
                        }
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

        // Update is called once per frame
        void Update()
        {
            if (!MLInput.IsStarted)
            {
                return;
            }
            switch (ActiveController.CurrentTouchpadGesture.Type)
            {
                case MLInput.Controller.TouchpadGesture.GestureType.None:
                    break;
                case MLInput.Controller.TouchpadGesture.GestureType.RadialScroll:
                    switch (ActiveController.CurrentTouchpadGesture.Direction)
                    {
                        case MLInput.Controller.TouchpadGesture.GestureDirection.Clockwise:
                            {
                                switch (ActiveController.TouchpadGestureState)
                                {
                                    case MLInput.Controller.TouchpadGesture.State.End:
                                        break;
                                    case MLInput.Controller.TouchpadGesture.State.Continue:
                                        swipeRightDelegate(this);
                                        break;
                                    case MLInput.Controller.TouchpadGesture.State.Start:
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        case MLInput.Controller.TouchpadGesture.GestureDirection.CounterClockwise:
                            {
                                switch (ActiveController.TouchpadGestureState)
                                {
                                    case MLInput.Controller.TouchpadGesture.State.End:
                                        break;
                                    case MLInput.Controller.TouchpadGesture.State.Continue:
                                        swipeLeftDelegate(this);
                                        break;
                                    case MLInput.Controller.TouchpadGesture.State.Start:
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case MLInput.Controller.TouchpadGesture.GestureType.Swipe:
                    {
                        switch (ActiveController.CurrentTouchpadGesture.Direction)
                        {
                            case MLInput.Controller.TouchpadGesture.GestureDirection.Left:
                                {
                                    switch (ActiveController.TouchpadGestureState)
                                    {
                                        case MLInput.Controller.TouchpadGesture.State.End:
                                            break;
                                        case MLInput.Controller.TouchpadGesture.State.Continue:
                                            break;
                                        case MLInput.Controller.TouchpadGesture.State.Start:
                                            swipeLeftDelegate(this);
                                            break;
                                        default:
                                            break;
                                    }
                                }

                                break;
                            case MLInput.Controller.TouchpadGesture.GestureDirection.Right:
                                {
                                    switch (ActiveController.TouchpadGestureState)
                                    {
                                        case MLInput.Controller.TouchpadGesture.State.End:
                                            break;
                                        case MLInput.Controller.TouchpadGesture.State.Continue:
                                            break;
                                        case MLInput.Controller.TouchpadGesture.State.Start:
                                            swipeRightDelegate(this);
                                            break;
                                        default:
                                            break;
                                    }
                                }

                                break; ;
                            default:
                                break;
                        }
                    }
                    break;
                case MLInput.Controller.TouchpadGesture.GestureType.Scroll:
                    {
                        switch (ActiveController.CurrentTouchpadGesture.Direction)
                        {
                            case MLInput.Controller.TouchpadGesture.GestureDirection.Left:
                                {
                                    switch (ActiveController.TouchpadGestureState)
                                    {
                                        case MLInput.Controller.TouchpadGesture.State.End:
                                            break;
                                        case MLInput.Controller.TouchpadGesture.State.Continue:

                                            break;
                                        case MLInput.Controller.TouchpadGesture.State.Start:
                                            swipeLeftDelegate(this);
                                            break;
                                        default:
                                            break;
                                    }
                                }

                                break;
                            case MLInput.Controller.TouchpadGesture.GestureDirection.Right:
                                {
                                    switch (ActiveController.TouchpadGestureState)
                                    {
                                        case MLInput.Controller.TouchpadGesture.State.End:
                                            break;
                                        case MLInput.Controller.TouchpadGesture.State.Continue:

                                            break;
                                        case MLInput.Controller.TouchpadGesture.State.Start:
                                            swipeRightDelegate(this);
                                            break;
                                        default:
                                            break;
                                    }
                                }

                                break; ;
                            default:
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}

