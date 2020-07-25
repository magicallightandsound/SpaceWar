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
    [RequireComponent(typeof(LineRenderer))]
    public class ActsAsControllerRay : MonoBehaviour
    {
        private LineRenderer lineRenderer = null;

        public GameObject cursor;
        public Color startColor = Color.white;
        public Color endColor = Color.white;

        private void Awake()
        {
            this.lineRenderer = GetComponent<LineRenderer>();
            this.lineRenderer.startWidth = 0.01f;
            this.lineRenderer.endWidth = 0.01f;
            this.lineRenderer.startColor = startColor;
            this.lineRenderer.endColor = endColor;
        }

        // Update is called once per frame
        void Update()
        {
            if (MLInput.IsStarted)
            {
                this.lineRenderer.SetPosition(0, transform.position);
                this.lineRenderer.SetPosition(1, cursor.transform.position);

                if (this.lineRenderer.startColor == this.startColor || this.lineRenderer.endColor == this.endColor)
                {
                    return;
                }

                this.lineRenderer.startColor = startColor;
                this.lineRenderer.endColor = endColor;
            }
        }
    }
}


