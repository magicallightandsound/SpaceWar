/*
Copyright 2020 Rodney Degracia

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to 
deal in the Software without restriction, including without limitation the 
rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
sell copies of the Software, and to permit persons to whom the Software is 
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MagicalLightAndSound
{
    namespace PhysicsSystem
    {
        public interface IMovableBehavior
        {
            Rigidbody rigidbody { get; }
            Transform transform { get; }
        }

        public struct Rotatable
        {
            private Vector3 axis;
            private float torque;
            private IMovableBehavior movableBehavior;

            public Rotatable(
                IMovableBehavior movableBehavior, 
                Vector3 axis, 
                float torque)
            {
                this.movableBehavior = movableBehavior;
                this.axis = axis;
                this.torque = torque;
            }

            public void Perform()
            {
                movableBehavior.rigidbody.AddTorque(axis * torque);
            }

            public void Perform(float t)
            {
                movableBehavior.rigidbody.AddTorque(axis * t);
            }
        }

        public struct Movable
        {
            public enum Type
            {
                LinearMotion,
                Teleport,
                Newtonian
            }

            public enum Status
            {
                Active,
                InActive
            }
            public Status status;

            
            public Vector3 target;
            public Type type;
            public Vector3 source;
            public float thrust;

            Vector3 direction
            {
                get
                {
                    return Vector3.Normalize(target - source);
                }
            }

            private IMovableBehavior movableBehavior;
            private AnimationCurve animationCurve;

            public Movable(
                IMovableBehavior movableBehavior, 
                Movable.Type type,
                Vector3 source,
                Vector3 target, 
                Movable.Status status,
                float thrust = 1.0f,
                AnimationCurve animationCurve = null)
            {
                this.movableBehavior = movableBehavior;
                this.source = source;
                this.target = target;
                this.type = type;
                this.status = status;
                this.thrust = thrust;
                this.animationCurve = animationCurve;

                switch (this.type)
                {
                    case Type.LinearMotion:
                        this.movableBehavior.rigidbody.isKinematic = true;
                        break;
                    case Type.Teleport:
                        this.movableBehavior.rigidbody.isKinematic = true;
                        break;
                    case Type.Newtonian:
                        this.movableBehavior.rigidbody.isKinematic = false;
                        break;
                    default:
                        break;
                }
            }

            public void Perform(float animationTime)
            {
                switch (type)
                {
                    case Type.LinearMotion:
                        this.movableBehavior.rigidbody.isKinematic = true;
                        Vector3 lerp = Vector3.Lerp(source, target, animationCurve.Evaluate(animationTime));
                        movableBehavior.rigidbody.MovePosition(lerp);
                        break;
                    case Type.Teleport:
                        this.movableBehavior.rigidbody.isKinematic = true;
                        movableBehavior.rigidbody.MovePosition(target);
                        break;
                    case Type.Newtonian:
                        this.movableBehavior.rigidbody.isKinematic = false;
                        movableBehavior.rigidbody.AddForce(direction * thrust * animationCurve.Evaluate(animationTime));
                        break;
                    default:
                        break;
                }
            }
        }
    }


}