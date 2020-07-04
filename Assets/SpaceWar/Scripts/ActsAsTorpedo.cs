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

public class ActsAsTorpedo : MonoBehaviour, IDeployable, IMovableBehavior
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
    public Weapon torpedo;

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

        this.torpedo = new Weapon(Weapon.Type.Torpedo, Weapon.Status.Disarmed);

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
        /// able to be recognized as a Torpedo when a collision occurs
        ///
        this.boxCollider = GetComponent<BoxCollider>();
        this.boxCollider.tag = torpedo.ToString();
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
        switch (torpedo.status)
        {
            case Weapon.Status.Dud:
                return;

            case Weapon.Status.Disarmed:
                return;

            case Weapon.Status.Armed:
                {
                    // Torpedo vs Torpedo destroy each other
                    if (other.tag == this.boxCollider.tag)
                    {
                        this.torpedo.status = Weapon.Status.OKToDestroy;
                    }
                }
                break;
            case Weapon.Status.Destroyed:
                return;

            case Weapon.Status.OKToDestroy:
                return;

            default:
                break;
        }
    }

    // Update is called once per frame
    public virtual void Update()
    {
        switch (torpedo.status)
        {
            case Weapon.Status.Dud:
            case Weapon.Status.Disarmed:
                break;
            case Weapon.Status.Armed:
                break;
            case Weapon.Status.Destroyed:
                
                break;
            case Weapon.Status.OKToDestroy:
                {
                    this.torpedo.status = Weapon.Status.Disarmed;
                    displayExplosion();
                    this.gameObject.SetActive(false);
                    this.gameObject.transform.position = Vector3.zero;

                    torpedo.status = Weapon.Status.Destroyed;
                }
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

                                armWhenSafe(animationTime);
                            }
                            break;
                        case Movable.Type.Teleport:
                            {
                                motion.Perform(Time.fixedDeltaTime);

                                armWhenSafe(1.0f);
                            }
                            break;
                        case Movable.Type.Newtonian:
                            {
                                motion.Perform(animationTime);
                                animationTime += Time.fixedDeltaTime;
                                Debug.Log("Newtonian animation time =" + animationTime.ToString());

                                armWhenSafe(animationTime);
                            }
                            break;
                        default:
                            break;
                    }

                    ///
                    /// Torpedo has reached targetVector without exploding
                    /// Destroy the torpedo with an explosion
                    ///
                    if (animationTime >= 1.0f && this.torpedo.status == Weapon.Status.Armed)
                    {
                        motion.status = Movable.Status.InActive;
                        torpedo.status = Weapon.Status.OKToDestroy;
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

    private void armWhenSafe(float animationTime)
    {
        if (torpedo.status == Weapon.Status.Dud)
        {
            return;
        }

        if (animationTime < 0.25)
        {
            return;
        }

        torpedo.status = Weapon.Status.Armed;
    }

    public void ConfigureWeapon(Vector3 targetVector, Dictionary<string, string> parameters)
    {
        this.targetPosition = targetVector;
        this.animationTime = 0;
        this.motion.source = transform.position;
        this.motion.target = this.targetPosition;
        this.motion.type = this.movableType;
        this.torpedo.status = Weapon.Status.Armed;
        this.motion.status = Movable.Status.Active;
        this.transform.LookAt(targetVector);
    }

    public void ConfigureDud(Vector3 targetVector)
    {
        this.targetPosition = targetVector;
        this.animationTime = 0;
        this.motion.source = transform.position;
        this.motion.target = this.targetPosition;
        this.motion.type = this.movableType;
        this.exhaustParticles.Play();
        this.torpedo.status = Weapon.Status.Dud;
        this.motion.status = Movable.Status.Active;
        this.transform.LookAt(targetVector);
    }

    public void ConfigureSelfDestruct(Vector3 targetVector)
    {
        this.targetPosition = targetVector;
        this.animationTime = 0;
        this.motion.source = transform.position;
        this.motion.target = this.targetPosition;
        this.motion.type = this.movableType;
        this.torpedo.status = Weapon.Status.Armed;
        this.motion.status = Movable.Status.Active;
        this.transform.LookAt(targetVector);
    }
}





