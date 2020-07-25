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

    public class ActsAsWorldMesh : MonoBehaviour
    {
        public MLSpatialMapper mlSpatialMapper = null;

        public MagicLeap.MeshingVisualizer meshingVisualizer = null;

        private Camera userCamera = null;

        private bool isOcclusion = true;

        void Awake()
        {
            userCamera = Camera.main;
        }


        void Start()
        {
            if (!MLInput.IsStarted)
            {
                MLResult inputStartResult = MLInput.Start();
                switch (inputStartResult.Result)
                {
                    case MLResult.Code.Ok:
                        commonStart();
                        break;
                    case MLResult.Code.InvalidParam:
                        break;
                    case MLResult.Code.PrivilegeDenied:
                    default:
                        break;
                }
            }
            else
            {
                commonStart();
            }
        }

        void commonStart()
        {
            MLInput.OnControllerButtonDown += MLInput_OnControllerButtonDown;

            meshingVisualizer.SetRenderers(MagicLeap.MeshingVisualizer.RenderMode.Occlusion);

            mlSpatialMapper.gameObject.transform.position = userCamera.gameObject.transform.position;
        }

        void Update()
        {
            mlSpatialMapper.gameObject.transform.position = userCamera.gameObject.transform.position;

        }

        void OnDestroy()
        {
            if (MLInput.IsStarted)
            {
                MLInput.OnControllerButtonDown -= MLInput_OnControllerButtonDown;
                MLInput.Stop();
            }
        }

        private void MLInput_OnControllerButtonDown(byte controllerId, MLInput.Controller.Button button)
        {
            if (button == MLInput.Controller.Button.Bumper)
            {
                isOcclusion = !isOcclusion;

                if (isOcclusion)
                {
                    meshingVisualizer.SetRenderers(MagicLeap.MeshingVisualizer.RenderMode.Occlusion);
                }
                else
                {

                    meshingVisualizer.SetRenderers(MagicLeap.MeshingVisualizer.RenderMode.Wireframe);
                }

            }
        }
    }
}


