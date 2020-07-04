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

using MagicalLightAndSound.CombatSystem;
using MagicalLightAndSound.ParticleSystem;

public class TorpedoTestHarness : MonoBehaviour
{
    public GameObject torpedo;
    public Transform target1;
    public Transform target2;
    public Transform target3;
    public Transform target4;

    private bool flag1 = false;
    private bool flag2 = false;

    [HideInInspector]
    public static WeaponsPool torpedoPool = null;

    [HideInInspector]
    public static ExplosionPool explosionPool = null;

    private void Awake()
    {
        TorpedoTestHarness.torpedoPool = new WeaponsPool(new Weapon(Weapon.Type.Torpedo, Weapon.Status.Disarmed), 3);
        TorpedoTestHarness.explosionPool = new ExplosionPool(new Explosion(Explosion.Type.Large), 3);
    }

    // Start is called before the first frame update
    void Start()
    {
        //InvokeRepeating("flyTarget", 2.0f, 5.0f);
        InvokeRepeating("flyAndExplode", 2.0f, 5.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void flyTarget()
    {
        if (flag1)
        {
            torpedo.GetComponent<ActsAsTorpedo>().ConfigureDud(target1.position);
            torpedo.SetActive(true);
        } else
        {
            torpedo.GetComponent<ActsAsTorpedo>().ConfigureDud(target2.position);
            torpedo.SetActive(true);
        }
        flag1 = !flag1;
    }

    void flyAndExplode()
    {
        if (flag2)
        {
            GameObject gameObject = TorpedoTestHarness.torpedoPool.objectFromPool;
            ActsAsTorpedo actsAsTorpedo = gameObject.GetComponent<ActsAsTorpedo>();
            actsAsTorpedo.motion.type = MagicalLightAndSound.PhysicsSystem.Movable.Type.LinearMotion;
            actsAsTorpedo.ConfigureSelfDestruct(target3.position);
            gameObject.SetActive(true);
        }
        else
        {
            GameObject gameObject = TorpedoTestHarness.torpedoPool.objectFromPool;
            ActsAsTorpedo actsAsTorpedo = gameObject.GetComponent<ActsAsTorpedo>();
            actsAsTorpedo.motion.type = MagicalLightAndSound.PhysicsSystem.Movable.Type.LinearMotion;
            actsAsTorpedo.ConfigureSelfDestruct(target4.position);
            gameObject.SetActive(true);
        }
        flag2 = !flag2;
    }
}
