using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MagicalLightAndSound.PhysicsSystem;

namespace MagicalLightAndSound
{
    namespace SpaceWar
    {
        namespace PropSystem
        {
            public interface IPropComponents
            {
                Obstacle obstacle { get; }
            }

            public struct Obstacle
            {
                public enum Type
                {
                    None,
                    Unknown,
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
                                    actsAsPlanet.obstacle.type = this.type;
                                     

                                    return go;
                                }

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

                public IPhysicalComponents physicalComponents;

                public Obstacle(Type type, IPhysicalComponents physicalComponents)
                {
                    this.type = type;
                    this.physicalComponents = physicalComponents;
                }

                public static Int32 layerMask
                {
                    get
                    {
                        return 0x01 << 0x0008;  // 0000 0000 0000 1000
                    }
                }
            }

        }
    }


}