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
    public class ActsAsCursorHoverable : MonoBehaviour
    {
        public Color hoverColor = Color.blue;
        public bool shouldColorChange = true;

        [HideInInspector]
        public bool isCursorHovering = false;

        private Color saveColor;
        private bool flipflop = false;

        // Update is called once per frame
        void Update()
        {
            if (MLInput.IsStarted)
            {
                if (!shouldColorChange)
                {
                    return;
                }

                if (isCursorHovering)
                {
                    if (!flipflop)
                    {
                        Renderer meshRenderer = GetComponent<Renderer>();
                        this.saveColor = meshRenderer.material.color;

                        meshRenderer.material.color = hoverColor;
                        flipflop = true;
                    }
                }
                else
                {
                    if (flipflop)
                    {
                        Renderer meshRenderer = GetComponent<Renderer>();
                        meshRenderer.material.color = this.saveColor;
                        flipflop = false;
                    }
                    
                }
            }
        }
    }

}

