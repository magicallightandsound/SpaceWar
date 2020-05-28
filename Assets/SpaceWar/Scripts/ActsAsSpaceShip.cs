using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

using MagicalLightAndSound.ParticleSystem;
using MagicalLightAndSound.CombatSystem;
using MagicalLightAndSound.PhysicsSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(AnimationCurve))]

public class ActsAsSpaceShip : MonoBehaviour, IMovableBehavior
{
    public int HitPoints = 100;
    public Rigidbody rigidBody;

    private Target spaceShip;
    private Movable motion;
    private float animationTime = 0;

    private readonly string torpedoTag = Weapon.Type.Torpedo.ToString();
    private readonly string spaceShipTag = Target.Type.SpaceShip.ToString();

    private ParticleSystem spaceShipExplosionParticles;
    private ParticleSystem spaceShipExhaustParticles;

    Rigidbody IMovableBehavior.rigidbody
    {
        get
        {
            return rigidBody;
        }
    }

    private void Awake()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;

        this.spaceShip = new Target(Target.Type.SpaceShip, Target.Status.Healthy, HitPoints);

        AnimationCurve animationCurve = GetComponent<AnimationCurve>();
        this.motion = new Movable(this, Movable.Type.LinearMotion, Vector3.zero, Movable.Status.InActive, animationCurve);

        Explosion spaceShipExplosion = new Explosion(Explosion.Type.Large);
        GameObject goExplosionParticleSystem = spaceShipExplosion.particleSystem;
        this.spaceShipExplosionParticles = goExplosionParticleSystem.GetComponent<ParticleSystem>();

        Exhaust spaceshipExhaust = new Exhaust(Exhaust.Type.Spaceship);
        GameObject goExhaustParticleSystem = spaceshipExhaust.particleSystem;
        this.spaceShipExhaustParticles = goExhaustParticleSystem.GetComponent<ParticleSystem>();
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

        if (other.tag == spaceShipTag )
        {
            ActsAsSpaceShip actsAsSpaceShip = other.GetComponent<ActsAsSpaceShip>();
            actsAsSpaceShip.spaceShip.status = Target.Status.OKToDestroy;

            this.spaceShip.status = Target.Status.OKToDestroy;

        }
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }



    // Update is called once per frame
    void Update()
    {
        switch (spaceShip.status)
        {
            case Target.Status.Healthy:

                break;
            case Target.Status.Damaged:

                break;
            case Target.Status.OKToDestroy:
                {
                    ParticleSystem exp = spaceShipExplosionParticles.GetComponent<ParticleSystem>();
                    exp.Play();

                    spaceShip.status = Target.Status.Destroyed;
                    motion.status = Movable.Status.InActive;

                    Destroy(gameObject, exp.main.duration);
                }
                break;
            case Target.Status.Destroyed:
                break;
            default:
                break;
        }

        switch (this.motion.status)
        {
            case Movable.Status.Active:
                {
                    switch (this.motion.type)
                    {
                        case Movable.Type.LinearMotion:
                            {
                                motion.Perform(animationTime);
                                animationTime += Time.deltaTime;

                                if (animationTime == 1.0f)
                                {
                                    animationTime = 0;
                                }
                            }
                            break;
                        case Movable.Type.Teleport:
                            {
                                motion.Perform(Time.deltaTime);
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
}





