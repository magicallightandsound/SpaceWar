﻿/*
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

using MagicalLightAndSound.PhysicsSystem;

[RequireComponent(typeof(Rigidbody))]

public class ActsAsPlanet : MonoBehaviour, IMovableBehavior
{
    public float force = 0.33f;
    public Rigidbody rigidBody;

    Rotatable rotation;

    Rigidbody IMovableBehavior.rigidbody
    {
        get { return rigidBody; }
    }

    // Start is called before the first frame update
    void Start()
    {
        this.rigidBody = GetComponent<Rigidbody>();
        this.rigidBody.useGravity = false;
        this.rigidBody.angularDrag = 0;

        this.rotation = new Rotatable(this, new Vector3(0, 1, 0), force);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        this.rotation.Perform(force);
    }

}

