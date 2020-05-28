using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MagicalLightAndSound
{
    namespace ParticleSystem
    {
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
                            return GameObject.Instantiate(Resources.Load("Exhaust/TorpedoExhaust") as GameObject);
                        case Type.Spaceship:
                            return GameObject.Instantiate(Resources.Load("Exhaust/Spaceship") as GameObject);
                        default:
                            return GameObject.Instantiate(Resources.Load("Exhaust/Torpedo") as GameObject);

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
                            return GameObject.Instantiate(Resources.Load("Explosions/LargeExplosion") as GameObject);
                        default:
                            return GameObject.Instantiate(Resources.Load("Explosions/SmallExplosion") as GameObject);

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
