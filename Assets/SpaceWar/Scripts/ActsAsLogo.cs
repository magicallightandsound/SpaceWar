using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MagicalLightAndSound;
using MagicalLightAndSound.PhysicsSystem;

[RequireComponent(typeof(Rigidbody))]

public class ActsAsLogo : MonoBehaviour, MagicalLightAndSound.PhysicsSystem.IPhysicalComponents
{
    public GameObject logo;
    public float torque = 0;

    MagicalLightAndSound.PhysicsSystem.Rotatable rotation;
    private Rigidbody rigidbody;

    Rigidbody IPhysicalComponents.rigidbody
    {
        get
        {
            return rigidbody;
        }
    }

    Transform IPhysicalComponents.transform
    {
        get
        {
            return this.logo.transform;
        }
    }

    private void Awake()
    {
        this.rigidbody = this.logo.GetComponent<Rigidbody>();
        this.rotation = new MagicalLightAndSound.PhysicsSystem.Rotatable(
            MagicalLightAndSound.PhysicsSystem.Rotatable.Type.LocalBody,
            this
        );
    }

    // Start is called before the first frame update
    void Start()
    {
        this.rigidbody.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        this.rotation.torque = this.torque;
        this.rotation.Perform();
    }
}
