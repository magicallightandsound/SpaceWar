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
            private float angle;
            private MonoBehaviour script;

            public Rotatable(MonoBehaviour script, Vector3 axis, float angle)
            {
                this.script = script;
                this.axis = axis;
                this.angle = angle;
            }

            public void Perform()
            {
                script.transform.Rotate(axis, Time.deltaTime * angle);
            }
        }

        public struct Movable
        {
            public enum Type
            {
                LinearMotion,
                Teleport
            }

            public enum Status
            {
                Active,
                InActive
            }
            public Status status;

            private IMovableBehavior movableBehavior;
            public Vector3 target;
            public Type type;
            private AnimationCurve animationCurve;

            public Movable(
                IMovableBehavior movableBehavior, 
                Movable.Type type, 
                Vector3 target, 
                Movable.Status status, 
                AnimationCurve animationCurve)
            {
                this.movableBehavior = movableBehavior;
                this.target = target;
                this.type = type;
                this.status = status;
                this.animationCurve = animationCurve;
            }

            public void Perform(float animationTime)
            {
                switch (type)
                {
                    case Type.LinearMotion:
                        movableBehavior.rigidbody.MovePosition(Vector3.Lerp(movableBehavior.transform.position, target, animationCurve.Evaluate(animationTime)));
                        
                        break;
                    case Type.Teleport:
                        movableBehavior.rigidbody.MovePosition(target);
                        break;
                    default:
                        break;
                }
            }
        }
    }


}