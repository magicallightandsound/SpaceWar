using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

using MagicalLightAndSound.PhysicsSystem;

public class ActsAsPlanet : MonoBehaviour
{
    Rotatable rotation;

    // Start is called before the first frame update
    void Start()
    {
        this.rotation = new Rotatable(this, new Vector3(0, 1, 0), 1);
    }

    // Update is called once per frame
    void Update()
    {
        rotation.Perform();
    }

}

