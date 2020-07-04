using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicalLightAndSound
{
    namespace PropSystem
    {
        public struct Obstacle
        {
            public enum Type
            {
                Planet,
                Asteroid,
                Moon
            }
            public Type type;

            public GameObject gameObject
            {
                get
                {
                    switch (this.type)
                    {
                        case Type.Planet:
                            {
                                GameObject go = GameObject.Instantiate(Resources.Load("Obstacles/Planet")) as GameObject;
                                go.SetActive(false);

                                ActsAsPlanet actsAsPlanet = go.GetComponent<ActsAsPlanet>();
                                actsAsPlanet.planet.type = this.type;

                                return go;
                            }
                            break;
                        case Type.Asteroid:
                            return null;

                        case Type.Moon:
                            return null;

                        default:
                            {
                                GameObject go = GameObject.Instantiate(Resources.Load("Obstacles/Obstacle")) as GameObject;
                                go.SetActive(false);

                                return go;
                            }
                    }
                }
            }
            public Obstacle(Type type)
            {
                this.type = type;
            }
        }

    }
}