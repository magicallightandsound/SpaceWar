using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MagicalLightAndSound.CombatSystem;
using MagicalLightAndSound.SpaceWar.PropSystem;
using MagicalLightAndSound.SpaceWar.NavigationSystem;

public class SpaceshipPlanetTestHarness : MonoBehaviour
{
    public GameObject spaceShip;
    public GameObject planet;
    public Transform targetLocation;
    public Transform sourceLocation;

    private bool flag = true;

    private Navigator navigator;

    ActsAsPlanet actsAsPlanet
    {
        get
        {
            return planet.GetComponent<ActsAsPlanet>();
        }
    }

    ActsAsSpaceShip actsAsSpaceShip
    {
        get
        {
            return spaceShip.GetComponent<ActsAsSpaceShip>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        this.navigator = new Navigator();
        
        planet.SetActive(true);


        InvokeRepeating("moveSpaceShip", 2.0f, 5.0f);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void moveSpaceShip()
    {


        if (flag)
        {
            Course course = new Course(spaceShip.transform.position, targetLocation.position);
            CourseCorrection courseCorrection;

            bool shouldCourseCorrect = this.navigator.calculateCourse(course, out courseCorrection);
            if (shouldCourseCorrect)
            {
                print("Should apply course correction to targetLocation");

                spaceShip.SetActive(true);

                GameObject gameObject = courseCorrection.gameObject;
                Debug.Assert(gameObject != null, "gameObject should not be null");

                switch (courseCorrection.avoidanceType)
                {
                    case CourseCorrection.AvoidanceType.Unknown:
                        Debug.Assert(false, "Should not assert");
                        break;
                    case CourseCorrection.AvoidanceType.Obstacle:
                        Obstacle obstacle = Obstacle.fromGameObject(gameObject);
                        switch (obstacle.type)
                        {
                            case Obstacle.Type.Planet:
                                print("Found obstacle type planet");
                                actsAsSpaceShip.orbitAroundObstacle(obstacle);
                                break;
                            case Obstacle.Type.Asteroid:
                                break;
                            case Obstacle.Type.Moon:
                                break;
                            default:
                                break;
                        }
                        break;
                    case CourseCorrection.AvoidanceType.Vehicle:
                        Debug.Assert(false, "Should not assert");
                        break;
                    case CourseCorrection.AvoidanceType.Weapon:
                        Debug.Assert(false, "Should not assert");
                        break;
                    default:
                        Debug.Assert(false, "Should not assert");
                        break;
                }

            } else
            {
                this.actsAsSpaceShip.orbitalRotation.status = MagicalLightAndSound.PhysicsSystem.Rotatable.Status.InActive;
                print("No course correction to targetLocation");
                this.actsAsSpaceShip.navigateTo(this.targetLocation.position);
            }
        } else
        {
            Course course = new Course(spaceShip.transform.position, sourceLocation.position);
            CourseCorrection courseCorrection;

            bool shouldCourseCorrect = this.navigator.calculateCourse(course, out courseCorrection);
            if (shouldCourseCorrect)
            {
                print("Should apply course to sourceLocation");

                spaceShip.SetActive(true);

                GameObject gameObject = courseCorrection.gameObject;
                Debug.Assert(gameObject != null, "gameObject should not be null");

                switch (courseCorrection.avoidanceType)
                {
                    case CourseCorrection.AvoidanceType.Unknown:
                        Debug.Assert(false, "Should not assert");
                        break;
                    case CourseCorrection.AvoidanceType.Obstacle:
                        Obstacle obstacle = Obstacle.fromGameObject(gameObject);
                        switch (obstacle.type)
                        {
                            case Obstacle.Type.Planet:
                                print("Found obstacle type planet");
                                actsAsSpaceShip.orbitAroundObstacle(obstacle);
                                break;
                            case Obstacle.Type.Asteroid:
                                break;
                            case Obstacle.Type.Moon:
                                break;
                            default:
                                break;
                        }
                        break;
                    case CourseCorrection.AvoidanceType.Vehicle:
                        Debug.Assert(false, "Should not assert");
                        break;
                    case CourseCorrection.AvoidanceType.Weapon:
                        Debug.Assert(false, "Should not assert");
                        break;
                    default:
                        Debug.Assert(false, "Should not assert");
                        break;
                }

            }
            else
            {
                print("No course correction to sourceLocation");
                this.actsAsSpaceShip.orbitalRotation.status = MagicalLightAndSound.PhysicsSystem.Rotatable.Status.InActive;
                this.actsAsSpaceShip.navigateTo(this.sourceLocation.position);
            }
        }
        flag = !flag;
    }

    public void rotateRigidBodyAroundPointBy(Rigidbody rb, Vector3 origin, Vector3 axis, float angle)
    {
        Quaternion q = Quaternion.AngleAxis(angle, axis);
        rb.MovePosition(q * (rb.transform.position - origin) + origin);
        rb.MoveRotation(rb.transform.rotation * q);
    }
}
