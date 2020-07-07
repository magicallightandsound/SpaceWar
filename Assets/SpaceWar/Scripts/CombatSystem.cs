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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MagicalLightAndSound
{
    namespace CombatSystem
    {
        public class WeaponsPool
        {
            private List<GameObject> pool = new List<GameObject>();
            private int index = 0;
            private Weapon prototype;

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

            public WeaponsPool(Weapon prototype, int count = 1)
            {
                this.prototype = prototype;

                GameObject gameObject = this.prototype.gameObject;
                pool.Add(gameObject);

                for (int i = 0; i < count - 1; i++)
                {
                    pool.Add(GameObject.Instantiate(gameObject));
                }
            }
        }

        public class Targets
        {
            private List<Vehicle> targets = new List<Vehicle>();
            public Targets(List<Vehicle> targets)
            {
                this.targets = targets;
            }
        }

        public interface IArmored
        {
            int ArmorClass { get; }
            int ReduceDamage(int amount);
        }

        public interface IDestructible : IArmored
        {
            void TakeDamage(int amount);
        }

        public interface IDamage
        {
            int DamageAmount { get; }
            void ApplyDamage(IDestructible d);
        }

        public interface IDeployable
        {
            void ConfigureWeapon(Vector3 targetVector, Dictionary<string, string> parameters);
            void ConfigureDud(Vector3 targetVector);
            void ConfigureSelfDestruct(Vector3 targetVector);
        }

        public struct Weapon : IDamage
        {
            public enum Status
            {
                Dud,
                Disarmed,
                Armed,
                Destroyed,
                OKToDestroy
            }
            public Status status;

            public enum Type
            {
                None,
                Torpedo
            }

            private Type type;

            public GameObject gameObject
            {
                get
                {
                    switch (type)
                    {
                        case Type.Torpedo:
                            {
                                GameObject go = GameObject.Instantiate(Resources.Load("Weapons/Torpedo")) as GameObject;
                                go.SetActive(false);

                                ActsAsTorpedo actAsTorpedo = go.GetComponent<ActsAsTorpedo>();
                                actAsTorpedo.torpedo.type = this.type;
                                actAsTorpedo.torpedo.status = this.status;

                                return go;
                            }
                        default:
                            {
                                GameObject go = GameObject.Instantiate(Resources.Load("weapons/Weapon")) as GameObject;

                                ActsAsTorpedo actAsTorpedo = go.GetComponent<ActsAsTorpedo>();
                                actAsTorpedo.torpedo.type = Weapon.Type.Torpedo;
                                actAsTorpedo.torpedo.status = Weapon.Status.Disarmed;

                                return go;
                            }
                    }
                }
            }

            public Weapon(Weapon.Type type, Weapon.Status status)
            {
                this.type = type;
                this.status = status;
            }

            public override string ToString()
            {
                return type.ToString();
            }

            public int DamageAmount
            {
                get
                {
                    switch (type)
                    {
                        case Type.Torpedo:
                            return 100;
                        default:
                            return 0;
                    }
                }
            }

            public void ApplyDamage(IDestructible destructible)
            {
                destructible.TakeDamage(DamageAmount);
            }


            public static Int32 layerMask
            {
                get
                {
                    return 0x000A;  // 0000 0000 0000 1010
                }
            }
        }

        public interface IVehicle
        {
            void ConfigureVehicle(Vector3 targetVector);
            void navigateTo(Vector3 targetVector);
        }

        public struct Vehicle : IDestructible
        {
            public enum Status
            {
                Healthy,
                Damaged,
                Active,
                Inactive,
                OKToDestroy,
                Destroyed
            }
            public Status status;

            public enum Type
            {
                None,
                SpaceShip
            }

            private Type type;

            public GameObject gameObject
            {
                get
                {
                    switch (type)
                    {
                        case Type.SpaceShip:
                            {
                                GameObject go = GameObject.Instantiate(Resources.Load("Vehicles/SpaceShip")) as GameObject;
                                go.SetActive(false);

                                ActsAsSpaceShip actsAsSpaceShip = go.GetComponent<ActsAsSpaceShip>();
                                actsAsSpaceShip.spaceShip.type = this.type;
                                actsAsSpaceShip.spaceShip.status = this.status;

                                return go;
                            }
                        default:
                            {
                                GameObject go = GameObject.Instantiate(Resources.Load("Vehicles/Vehicle")) as GameObject;
                                go.SetActive(false);
                                return go;
                            }
                    }
                }
            }

            public int ArmorClass
            {
                get
                {
                    switch (type)
                    {
                        case Type.SpaceShip:
                            return 10;

                        default:
                            return 0;

                    }
                }
            }

            public override string ToString()
            {
                return type.ToString();
            }

            private int hitPoints;

            public Vehicle(Vehicle.Type type, Vehicle.Status status, int hp)
            {
                this.hitPoints = hp;
                this.type = type;
                this.status = status;
            }

            public int ReduceDamage(int amount)
            {
                return amount - ArmorClass;
            }

            public void TakeDamage(int amount)
            {
                hitPoints = hitPoints - ReduceDamage(amount);

                if (hitPoints <= 0)
                {
                    status = Status.OKToDestroy;
                }
            }

            public static Int32 layerMask
            {
                get
                {
                    return 0x0009;  // 0000 0000 0000 1001
                }
            }
        }

        public struct Explosive : IDamage
        {
            public enum Type
            {
                High
            }
            private Type type;

            public int DamageAmount
            {
                get
                {
                    switch (type)
                    {
                        case Type.High:
                            return 100;

                        default:
                            return 0;

                    }
                }
            }

            private IDestructible destructible;

            public Explosive(IDestructible destructible, Explosive.Type type)
            {
                this.destructible = destructible;
                this.type = type;
            }

            public void ApplyDamage(IDestructible destructible)
            {
                destructible.TakeDamage(DamageAmount);
            }
        }
    }
}

