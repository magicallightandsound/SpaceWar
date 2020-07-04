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

namespace MagicalLightAndSound
{
    namespace ParticleSystem
    {
        public class ExplosionPool
        {
            public Explosion prototype;

            private List<GameObject> pool = new List<GameObject>();
            private int index = 0;
            
            public GameObject objectFromPool
            {
                get
                {
                    if (index == pool.Count)
                    {
                        index = 0;
                    }
                    return pool[index++];
                }
            }

            public ExplosionPool(Explosion prototype, int count = 1)
            {
                this.prototype = prototype;

                GameObject gameObject = this.prototype.particleSystem;
                pool.Add(gameObject);

                for (int i = 0; i < count - 1; i++)
                {
                    pool.Add(GameObject.Instantiate(gameObject));
                }
            }
        }

        public interface IParticles
        {
            GameObject particleSystem { get; }
        }

        public struct Exhaust : IParticles
        {
            public enum Type
            {
                Torpedo,
                Spaceship
            }
            private Type type;

            public GameObject particleSystem
            {
                get
                {
                    switch (type)
                    {
                        case Type.Torpedo:
                            {
                                GameObject gameObject = GameObject.Instantiate(Resources.Load("Exhaust/TorpedoExhaust") as GameObject);
                                gameObject.SetActive(false);

                                return gameObject;
                            }
                        case Type.Spaceship:
                            {
                                GameObject gameObject = GameObject.Instantiate(Resources.Load("Exhaust/SpaceshipExhaust") as GameObject);
                                gameObject.SetActive(false);

                                return gameObject;
                            }
                        default:
                            {
                                GameObject gameObject = GameObject.Instantiate(Resources.Load("Exhaust/Exhaust") as GameObject);
                                gameObject.SetActive(false);

                                return gameObject;
                            }
                    }
                }
            }

            public Exhaust(Exhaust.Type type)
            {
                this.type = type;
            }
        }

        public struct Explosion : IParticles
        {
            public enum Type
            {
                Small,
                Large
            }
            private Type type;

            public GameObject particleSystem
            {
                get
                {
                    switch (type)
                    {
                        case Type.Large:
                            {
                                GameObject gameObject = GameObject.Instantiate(Resources.Load("Explosions/LargeExplosion") as GameObject);
                                gameObject.SetActive(false);

                                return gameObject;
                            }
                        default:
                            {
                                GameObject gameObject = GameObject.Instantiate(Resources.Load("Explosions/SmallExplosion") as GameObject);
                                gameObject.SetActive(false);
                                return gameObject;
                            }
                             

                    }
                }
            }

            public Explosion(Explosion.Type type)
            {
                this.type = type;
            }
        }

    }
}
