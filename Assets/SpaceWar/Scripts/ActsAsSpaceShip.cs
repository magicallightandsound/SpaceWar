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

using MagicalLightAndSound.ParticleSystem;
using MagicalLightAndSound.CombatSystem;
using MagicalLightAndSound.PhysicsSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]

public class ActsAsSpaceShip : MonoBehaviour, IMovableBehavior
{
    public int HitPoints = 100;
    public float thrust = 0;
    public AnimationCurve animationCurve = new AnimationCurve();
    public Movable.Type movableType;

    private Targetable spaceShip;
    private Movable motion;
    private float animationTime = 0;

    private Rigidbody rigidBody;
    private readonly string torpedoTag = Weapon.Type.Torpedo.ToString();
    private readonly string spaceShipTag = Targetable.Type.SpaceShip.ToString();

    private ParticleSystem spaceShipExplosionParticles;
    private ParticleSystem spaceShipExhaustParticles;

    private BoxCollider boxCollider;

    Rigidbody IMovableBehavior.rigidbody
    {
        get
        {
            return rigidBody;
        }
    }

    private void Awake()
    {
        this.boxCollider = GetComponent<BoxCollider>();
        this.rigidBody = GetComponent<Rigidbody>();

        Explosion spaceShipExplosion = new Explosion(Explosion.Type.Large);
        GameObject goExplosionParticleSystem = spaceShipExplosion.particleSystem;
        this.spaceShipExplosionParticles = goExplosionParticleSystem.GetComponent<ParticleSystem>();
        this.spaceShipExplosionParticles.transform.parent = this.transform;

        Exhaust spaceshipExhaust = new Exhaust(Exhaust.Type.Spaceship);
        GameObject goExhaustParticleSystem = spaceshipExhaust.particleSystem;
        this.spaceShipExhaustParticles = goExhaustParticleSystem.GetComponent<ParticleSystem>();
        this.spaceShipExhaustParticles.transform.parent = this.transform;
    }




    // Start is called before the first frame update
    void Start()
    {
        this.spaceShip = new Targetable(Targetable.Type.SpaceShip, Targetable.Status.Healthy, HitPoints);

        this.motion = new Movable(
            this,
            Movable.Type.LinearMotion,
            Vector3.zero,
            Vector3.zero,
            Movable.Status.InActive,
            thrust,
            animationCurve);

        boxCollider.isTrigger = true;

        this.rigidBody.useGravity = false;
        this.rigidBody.isKinematic = false;
    }

    // Update is called once per frame
    void Update()
    {
        switch (spaceShip.status)
        {
            case Targetable.Status.Healthy:

                break;
            case Targetable.Status.Damaged:

                break;
            case Targetable.Status.OKToDestroy:
                {
                    ParticleSystem exp = spaceShipExplosionParticles.GetComponent<ParticleSystem>();
                    exp.Play();

                    spaceShip.status = Targetable.Status.Destroyed;
                    motion.status = Movable.Status.InActive;

                    Destroy(gameObject, exp.main.duration);
                }
                break;
            case Targetable.Status.Destroyed:
                break;
            default:
                break;
        }

        
    }

    private void FixedUpdate()
    {
        switch (this.motion.status)
        {
            case Movable.Status.Active:
                {
                    switch (this.motion.type)
                    {
                        case Movable.Type.LinearMotion:
                            {
                                motion.Perform(animationTime);
                                animationTime += Time.fixedDeltaTime;
                                Debug.Log("animation time =" + animationTime.ToString());

                                if (animationTime >= 1.0f)
                                {
                                    this.motion.status = Movable.Status.InActive;
                                    animationTime = 0;
                                }
                            }
                            break;
                        case Movable.Type.Teleport:
                            {
                                motion.Perform(Time.fixedDeltaTime);
                                this.motion.status = Movable.Status.InActive;
                                animationTime = 0;
                            }
                            break;
                        case Movable.Type.Newtonian:
                            {
                                motion.Perform(animationTime);
                                animationTime += Time.fixedDeltaTime;
                                Debug.Log("animation time =" + animationTime.ToString());

                                if (animationTime >= 1.0f)
                                {
                                    this.motion.status = Movable.Status.InActive;
                                    animationTime = 0;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                break;
            case Movable.Status.InActive:
                // Do not hide the Spaceship
                break;
            default:
                break;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == torpedoTag)
        {
            ActsAsTorpedo actsAsTorpedo = other.GetComponent<ActsAsTorpedo>();
            actsAsTorpedo.torpedo.ApplyDamage(spaceShip);
            actsAsTorpedo.torpedo.status = Weapon.Status.OKToDestroy;
            return;
        }

        if (other.tag == spaceShipTag)
        {
            ActsAsSpaceShip actsAsSpaceShip = other.GetComponent<ActsAsSpaceShip>();
            actsAsSpaceShip.spaceShip.status = Targetable.Status.OKToDestroy;

            this.spaceShip.status = Targetable.Status.OKToDestroy;

        }
    }


    public void navigateTo(Vector3 targetPosition)
    {
        this.motion.source = this.transform.position;
        this.motion.target = targetPosition;
        this.motion.type = this.movableType;
        this.motion.thrust = this.thrust;
        this.animationTime = 0;
        this.spaceShip.status = Targetable.Status.Healthy;
        this.motion.status = Movable.Status.Active;
        this.spaceShipExhaustParticles.Play();

    }
}





