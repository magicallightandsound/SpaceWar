using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MagicalLightAndSound
{
    namespace CombatSystem
    {
        public class Weapons
        {
            private List<Weapon> weapons = new List<Weapon>();
            public Weapons(List<Weapon> weapons)
            {
                this.weapons = weapons;
            }
        }

        public class Targets
        {
            private List<Target> targets = new List<Target>();
            public Targets(List<Target> targets)
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
            void DeployWeapon(Vector3 targetVector, Dictionary<string, string> parameters);
            void DeployDud(Vector3 targetVector);
        }

        public struct Weapon : IDamage
        {
            public enum Status
            {
                InActive,
                Active,
                Destroyed,
                OKToDestroy
            }
            public Status status;

            public enum Type
            {
                Torpedo
            }

            private Type type;

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
        }

        public struct Target : IDestructible
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
                SpaceShip
            }
            private Type type;

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

            private int hitPoints;

            public Target(Target.Type type, Target.Status status, int hp)
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

