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
    public Rigidbody rigidBody;

    private ParticleSystem exhaustParticles;
    private ParticleSystem torpedoExplosionParticles;

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

        motion = new MagicalLightAndSound.PhysicsSystem.Movable(this, Movable.Type.LinearMotion, targetPosition, Movable.Status.InActive, animationCurve);
        torpedo = new Weapon(Weapon.Type.Torpedo, Weapon.Status.InActive);

        Exhaust torpedoExhaust = new Exhaust(Exhaust.Type.Torpedo);
        GameObject goParticleSystem = torpedoExhaust.particleSystem;
        this.exhaustParticles = goParticleSystem.GetComponent<ParticleSystem>();
        Debug.Assert(this.exhaustParticles != null, "exhaustParticles should not be null");

        Explosion torpedoExplosion = new Explosion(Explosion.Type.Large);
        GameObject goExplosionParticleSystem = torpedoExplosion.particleSystem;
        this.torpedoExplosionParticles = goExplosionParticleSystem.GetComponent<ParticleSystem>();
        Debug.Assert(this.torpedoExplosionParticles != null, "torpedoExplosionParticles should not be null");

        GetComponent<BoxCollider>().tag = torpedo.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public virtual void Update()
    {
        switch (torpedo.status)
        {
            case Weapon.Status.InActive:
                break;
            case Weapon.Status.Active:
                break;
            case Weapon.Status.Destroyed:
                break;
            case Weapon.Status.OKToDestroy:
                {
                    this.torpedoExplosionParticles.Play();

                    motion.status = Movable.Status.InActive;

                    torpedo.status = Weapon.Status.Destroyed;
                    Destroy(gameObject, torpedoExplosionParticles.main.duration);
                }
                break;
            default:
                break;
        }


    }

    private void FixedUpdate()
    {
        switch (motion.status)
        {
            case Movable.Status.Active:
                {
                    motion.Perform(animationTime);
                    animationTime += Time.deltaTime;
                    Debug.Log("animation time =" + animationTime.ToString());

                    ///
                    /// Torpedo has reached targetVector without exploding
                    /// Destory the torpedo without any explosions
                    ///
                    if (animationTime >= 1.0f)
                    {
                        motion.status = Movable.Status.InActive;

                        ///
                        /// Only destroy a torpedo if it is active
                        ///
                        if (torpedo.status == Weapon.Status.Active)
                        {
                            torpedo.status = Weapon.Status.OKToDestroy;
                            Destroy(gameObject);
                        }
                    }
                }
                break;
            case Movable.Status.InActive:
                {
                    // Don't hide the torpedo when inactive
                }
                break;
            default:
                break;
        }
    }

    public void DeployWeapon(Vector3 targetVector, Dictionary<string, string> parameters)
    {
        this.targetPosition = targetVector;
        this.exhaustParticles.Play();
        this.torpedo.status = Weapon.Status.Active;
        this.motion.status = Movable.Status.Active;
    }

    public void DeployDud(Vector3 targetVector)
    {
        this.targetPosition = targetVector;
        this.exhaustParticles.Play();
        this.torpedo.status = Weapon.Status.InActive;
        this.motion.status = Movable.Status.Active;
    }
}





