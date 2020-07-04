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

using MagicalLightAndSound.ParticleSystem;
using MagicalLightAndSound.CombatSystem;
using MagicalLightAndSound.PhysicsSystem;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]

public class ActsAsSpaceShip : MonoBehaviour, IVehicle, IMovableBehavior
{
    public Vector3 targetPosition = Vector3.zero;
    public AnimationCurve animationCurve = new AnimationCurve();

    public float thrust = 1.0f;
    public Movable.Type movableType;

    private Rigidbody rigidBody;
    private ParticleSystem exhaustParticles;
    private BoxCollider boxCollider;
    private GameObject particleSystemGameObject;
    private GameObject explosionGameObject;

    [HideInInspector]
    public Vehicle spaceShip;

    [HideInInspector]
    public Movable motion;

    private float animationTime = 0;

    Rigidbody IMovableBehavior.rigidbody
    {
        get { return rigidBody; }
    }

    private void Awake()
    {
        this.rigidBody = GetComponent<Rigidbody>();

        this.motion = new MagicalLightAndSound.PhysicsSystem.Movable(
            this,
            movableType,
            this.transform.position,
            targetPosition,
            Movable.Status.InActive,
            thrust,
            animationCurve);

        this.spaceShip = new Vehicle(Vehicle.Type.SpaceShip, Vehicle.Status.Inactive, 100);
         
        Exhaust torpedoExhaust = new Exhaust(Exhaust.Type.Torpedo);
        this.particleSystemGameObject = torpedoExhaust.particleSystem;
        this.exhaustParticles = particleSystemGameObject.GetComponent<ParticleSystem>();
        this.exhaustParticles.transform.parent = this.transform;
        Debug.Assert(this.exhaustParticles != null, "exhaustParticles should not be null");
    }

    // Start is called before the first frame update
    void Start()
    {
        this.rigidBody.useGravity = false;
        this.rigidBody.isKinematic = false;

        ///
        /// Assign a tag to the box collider, therefore making this game object
        /// able to be recognized as a Ship when a collision occurs
        ///
        this.boxCollider = GetComponent<BoxCollider>();
        this.boxCollider.tag = this.spaceShip.ToString();
        this.boxCollider.isTrigger = true;

    }

    private void OnEnable()
    {
        this.exhaustParticles.Play();
    }

    private void OnDisable()
    {
        this.exhaustParticles.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (spaceShip.status)
        {
            case Vehicle.Status.Healthy:
                break;
            case Vehicle.Status.Damaged:
                break;
            case Vehicle.Status.Active:
                {
                    // Ship vs Ship destroy each other
                    if (other.tag == this.boxCollider.tag)
                    {
                        this.spaceShip.status = Vehicle.Status.OKToDestroy;
                    }
                }
                break;
            case Vehicle.Status.Inactive:
                break;
            case Vehicle.Status.OKToDestroy:
                break;
            case Vehicle.Status.Destroyed:
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    public virtual void Update()
    {
        switch (spaceShip.status)
        {
            case Vehicle.Status.Healthy:
                break;
            case Vehicle.Status.Damaged:
                break;
            case Vehicle.Status.Active:
                break;
            case Vehicle.Status.Inactive:
                break;
            case Vehicle.Status.OKToDestroy:
                {
                    this.spaceShip.status = Vehicle.Status.Inactive;
                    displayExplosion();
                    this.gameObject.SetActive(false);
                    this.gameObject.transform.position = Vector3.zero;

                    spaceShip.status = Vehicle.Status.Destroyed;
                }
                break;
            case Vehicle.Status.Destroyed:
                break;
            default:
                break;
        }
    }

    private void displayExplosion()
    {
        Explosion explosion = ActsAsObjectPool.explosionPool.prototype;
        this.explosionGameObject = explosion.particleSystem;
        this.explosionGameObject.transform.position = this.transform.position;
        ParticleSystem particleSystem = this.explosionGameObject.GetComponent<ParticleSystem>();
        particleSystem.Play();
        this.explosionGameObject.SetActive(true);
        Destroy(explosionGameObject, particleSystem.main.duration);
    }

    private void FixedUpdate()
    {
        switch (motion.status)
        {
            case Movable.Status.Active:
                {
                    switch (motion.type)
                    {
                        case Movable.Type.LinearMotion:
                            {
                                motion.Perform(animationTime);
                                animationTime += Time.fixedDeltaTime;
                                Debug.Log("LinearMotion animation time =" + animationTime.ToString());
                            }
                            break;
                        case Movable.Type.Teleport:
                            {
                                motion.Perform(Time.fixedDeltaTime);
                            }
                            break;
                        case Movable.Type.Newtonian:
                            {
                                motion.Perform(animationTime);
                                animationTime += Time.fixedDeltaTime;
                                Debug.Log("Newtonian animation time =" + animationTime.ToString());
                            }
                            break;
                        default:
                            break;
                    }
                }
                break;
            case Movable.Status.InActive:
                {

                }
                break;
            default:
                break;
        }
    }

    public void ConfigureVehicle(Vector3 targetVector)
    {
        this.targetPosition = targetVector;
        this.animationTime = 0;
        this.motion.source = transform.position;
        this.motion.target = this.targetPosition;
        this.motion.type = this.movableType;
        this.spaceShip.status = Vehicle.Status.Active;
        this.motion.status = Movable.Status.Active;
        this.transform.LookAt(targetVector);
    }

    public void navigateTo(Vector3 targetVector)
    {
        this.targetPosition = targetVector;
        this.animationTime = 0;
        this.motion.source = transform.position;
        this.motion.target = this.targetPosition;
        this.motion.type = this.movableType;
        this.spaceShip.status = Vehicle.Status.Active;
        this.motion.status = Movable.Status.Active;
        this.transform.LookAt(targetVector);
    }
}





