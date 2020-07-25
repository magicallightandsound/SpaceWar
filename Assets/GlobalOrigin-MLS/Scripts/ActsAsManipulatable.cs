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
    /// <summary>
    /// Add to Prefab, to make it maniputable
    /// </summary>
    /// 

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(ActsAsCursorHoverable))]

    public class ActsAsManipulatable : MonoBehaviour
    {
        public bool isClonable = false;
        public bool isMovable = false;
        public bool isHoverable = false;
        public bool isKinematic = true;
        public bool isTriggerable = false;

        [HideInInspector]
        public bool isBeingMoved = false;

        /// <summary>
        /// Unsupported, use ActsAsPickerManipulator instead
        /// </summary>
        [HideInInspector]
        public bool isBeingCloned
        {
            get => throw new System.NotSupportedException("Use ActsAsPickerManipulator instead");
            set => throw new System.NotSupportedException("Use ActsAsPickerManipulator instead");
        }
        
        /// <summary>
        /// Unsupported, use ActsAsHoverable instead
        /// </summary>
        [HideInInspector]
        public bool isBeingHovered
        {
            get => throw new System.NotSupportedException("Use ActsAsHoverable instead");
            set => throw new System.NotSupportedException("Use ActsAsHoverable instead");
        }

        public delegate void TriggerDelegate(byte controllerId, float triggerValue);
        public TriggerDelegate OnTrigger = delegate { };

        private void Awake()
        {
            Rigidbody rigidBody = GetComponent<Rigidbody>();
            rigidBody.isKinematic = isKinematic;
        }

        // Start is called before the first frame update
        void Start()
        {
            if (isClonable || isMovable || isHoverable)
            {
                if (this.gameObject.GetComponent<ActsAsCursorHoverable>())
                {
                    return;
                }
                this.gameObject.AddComponent<ActsAsCursorHoverable>();
            }
        }

        private void OnEnable()
        {
            ActsAsCursorHoverable actsAsCursorHoverable = this.gameObject.GetComponent<ActsAsCursorHoverable>();
            if (actsAsCursorHoverable == null)
            {
                return;
            }
            actsAsCursorHoverable.enabled = true;
        }

        private void OnDisable()
        {
            ActsAsCursorHoverable actsAsCursorHoverable = this.gameObject.GetComponent<ActsAsCursorHoverable>();
            if (actsAsCursorHoverable == null)
            {
                return;
            }
            actsAsCursorHoverable.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

