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
        public interface IPhysicalComponents
        {
            Rigidbody rigidbody { get; }
            Transform transform { get; }
        }

        public struct Rotatable
        {
            public enum Status
            {
                Active,
                InActive
            }
            public Status status;

            public enum Type
            {
                None,
                LocalBody,
                ExternalBody
            }
            public Type type;

            public Vector3 localAxis;
            public Vector3 worldAxis;
            public Vector3 worldOrigin;
            public float torque;
            public IPhysicalComponents physicalComponents;
            public float orbitalAngle;
            public Vector3 targetWorldPosition;

            public Rotatable(
                Type type,
                IPhysicalComponents physicalComponents
                )
            {
                this.physicalComponents = physicalComponents;
                this.localAxis = Vector3.zero;
                this.torque = 0;
                this.orbitalAngle = 0;
                this.type = type;
                this.worldOrigin = Vector3.zero;
                this.worldAxis = Vector3.zero;
                this.status = Status.InActive;
                this.targetWorldPosition = Vector3.zero;

                switch (this.type)
                {
                    case Type.LocalBody:
                        this.physicalComponents.rigidbody.isKinematic = false;
                        break;
                    case Type.ExternalBody:
                        this.physicalComponents.rigidbody.isKinematic = false;
                        break;
                    default:
                        break;
                }


            }

            public void Perform(float animationTime)
            {
                switch (type)
                {
                    case Type.LocalBody:
                        physicalComponents.rigidbody.AddTorque(localAxis * torque * animationTime);
                        break;
                    case Type.ExternalBody:
                        Rigidbody rigidbody = physicalComponents.rigidbody;
                        float orbitRoationSpeed = 1.0f;
                        float orbitRadius = 2.0f;
                        float orbitRadiusCorrectionSpeed = 2.0f;
                        float orbitAlignToDirectionSpeed = 1.0f;

                        Vector3 previousPosition = rigidbody.transform.position;

                        //Movement
                        rigidbody.transform.RotateAround(worldOrigin, worldAxis, orbitRoationSpeed * Time.deltaTime);
                        Vector3 orbitDesiredPosition = targetWorldPosition; // (rigidbody.transform.position - worldOrigin).normalized * orbitRadius + worldOrigin;
                        rigidbody.transform.position = Vector3.Slerp(rigidbody.transform.position, orbitDesiredPosition, Time.deltaTime * orbitRadiusCorrectionSpeed);

                        //Rotation
                        Vector3 relativePos = rigidbody.transform.position - previousPosition;
                        Quaternion rotation = Quaternion.LookRotation(relativePos);
                        rigidbody.transform.rotation = Quaternion.Slerp(rigidbody.transform.rotation, rotation, orbitAlignToDirectionSpeed * Time.deltaTime);

                        break;
                    default:
                        Debug.Assert(false, "Should not assert");
                        break;
                }
                
            }

            public void Perform()
            {
                switch (type)
                {
                    case Type.LocalBody:
                        physicalComponents.rigidbody.AddTorque(localAxis * torque);
                        
                        break;
                    case Type.ExternalBody:
                        // float initV = Mathf.Sqrt(100.0f / this.physicalComponents.rigidbody.transform.position.magnitude);
                        // this.physicalComponents.rigidbody.velocity = new Vector3(0 ,0 , initV);

                        Rigidbody rigidbody = physicalComponents.rigidbody;
                        float r = Vector3.Magnitude(rigidbody.transform.position - this.worldOrigin);
                        float totalForce = -(1000f) / (r * r);
                        Vector3 force = (rigidbody.transform.position).normalized * totalForce;
                        rigidbody.AddForce(force);
                        break;
                    default:
                        Debug.Assert(false, "Should not assert");
                        break;
                }
                
            }
        }

        public struct Movable
        {
            public enum Type
            {
                None,
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

            private IPhysicalComponents movableBehavior;
            private AnimationCurve animationCurve;

            public Movable(
                IPhysicalComponents movableBehavior, 
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