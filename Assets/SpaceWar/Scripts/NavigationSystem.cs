using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


using MagicalLightAndSound.SpaceWar.PropSystem;
using MagicalLightAndSound.CombatSystem;

namespace MagicalLightAndSound
{
    namespace SpaceWar
    {
        namespace NavigationSystem
        {
            public struct Course
            {
                public Vector3 currentLocation;
                public Vector3 proposedTargetLocation;

                public Vector3 direction
                {
                    get
                    {
                        return Vector3.Normalize(proposedTargetLocation - currentLocation);
                    }
                }

                public float distance
                {
                    get
                    {
                        return Vector3.Distance(proposedTargetLocation, currentLocation);
                    }
                }

                public Course(Vector3 currentLocation, Vector3 proposedTargetLocation)
                {
                    this.currentLocation = currentLocation;
                    this.proposedTargetLocation = proposedTargetLocation;
                }

                public override string ToString()
                {
                    return (
                        "currentLocation =" + currentLocation.ToString() + " " +
                        "proposedTargetLocation =" + proposedTargetLocation.ToString() + " " +
                        "distance = " + distance.ToString()
                        ); 
                 }
            }

            public struct CourseCorrection
            {
                public enum AvoidanceType
                {
                    Unknown,
                    Obstacle,
                    Vehicle,
                    Weapon
                }
                public AvoidanceType avoidanceType;
                public GameObject gameObject;
                public Vector3 origin;
                public float radiusOffset;

                public Vehicle.Type vehicleType;
                public Weapon.Type weaponType;
                public Obstacle.Type obstacleType;

                public CourseCorrection(GameObject gameObject, float radiusOffset)
                {
                    this.gameObject = gameObject;
                    this.origin = gameObject.GetComponent<Rigidbody>().transform.position;
                    this.radiusOffset = radiusOffset;
                    this.avoidanceType = AvoidanceType.Unknown;
                    this.vehicleType = Vehicle.Type.None;
                    this.obstacleType = Obstacle.Type.None;
                    this.weaponType = Weapon.Type.None;
                }

                public override string ToString()
                {
                    return (
                        "origin =" + origin.ToString() + " " +
                        "radiusOffset =" + radiusOffset.ToString()
                        );
                }
            }

            public class Navigator
            {
                public bool calculateCourse(Course course, out CourseCorrection courseCorrection)
                {
                    Ray ray = new Ray(course.currentLocation, course.direction);
                    RaycastHit raycastHit;

                    Int32 obstacles = Obstacle.layerMask;
                    Int32 vehicles = Vehicle.layerMask;

                    bool hasRaycastHit = Physics.Raycast(ray, out raycastHit, course.distance, obstacles);
                    // Debug.Assert(hasRaycastHit, course.ToString());

                    if (hasRaycastHit)
                    {
                        Vector3 hitPoint = raycastHit.point;

                        Collider collider = raycastHit.collider;

                        float radiusDistance = Vector3.Distance(hitPoint, collider.transform.position);

                        GameObject gameObject = collider.gameObject;
                        courseCorrection = new CourseCorrection(gameObject, radiusDistance);

                        switch (collider.tag.ToString())
                        {
                            case "SpaceShip":
                                Debug.Log("Should course correct for Spaceship");
                                courseCorrection.avoidanceType = CourseCorrection.AvoidanceType.Vehicle;
                                courseCorrection.vehicleType = Vehicle.Type.SpaceShip;
                                break;
                            case "Torpedo":
                                Debug.Log("Should course correct for Torpedo");
                                courseCorrection.avoidanceType = CourseCorrection.AvoidanceType.Weapon;
                                courseCorrection.weaponType = Weapon.Type.Torpedo;
                                break;
                            case "Planet":
                                Debug.Log("Should course correct for Planet");
                                courseCorrection.avoidanceType = CourseCorrection.AvoidanceType.Obstacle;
                                courseCorrection.obstacleType = Obstacle.Type.Planet;
                                break;
                            default:
                                Debug.Log("Should course correct for Unknown collider");
                                courseCorrection.avoidanceType = CourseCorrection.AvoidanceType.Unknown;
                                courseCorrection.obstacleType = Obstacle.Type.Unknown;
                                break;
                        }

                        Debug.Log("Course correction =" + courseCorrection.ToString());
                        return true;
                    }
                    else
                    {
                        // No vehicles or obstacles, no need to correct course
                        courseCorrection = new CourseCorrection();
                        return false;
                    }
                }
            }
        }
    }
}