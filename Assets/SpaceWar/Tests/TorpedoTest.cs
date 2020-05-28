using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TorpedoTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void NewTestScriptSimplePasses()
        {
            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator AwakeStartUpdateTest()
        {
            MonoBehaviourTest<ActsAsTorpedoTest> test = new MonoBehaviourTest<ActsAsTorpedoTest>();
 
            yield return test;

            Assert.IsTrue(test.component.torpedo.status == MagicalLightAndSound.CombatSystem.Weapon.Status.InActive);
            Assert.IsTrue(test.component.motion.status == MagicalLightAndSound.PhysicsSystem.Movable.Status.InActive);

            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }

        [UnityTest]
        public IEnumerator DeployDudTest()
        {
            MonoBehaviourTest<ActsAsTorpedoTest> test = new MonoBehaviourTest<ActsAsTorpedoTest>();

            test.component.DeployDud(Vector3.one * 10);

            yield return test;

 
            Assert.IsTrue(test.component.torpedo.status == MagicalLightAndSound.CombatSystem.Weapon.Status.InActive);
            Assert.IsTrue(test.component.motion.status == MagicalLightAndSound.PhysicsSystem.Movable.Status.InActive);
            Assert.IsTrue(Vector3.Distance(test.component.transform.position, Vector3.one * 10) > 10.0f);


            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }

        public class ActsAsTorpedoTest : ActsAsTorpedo, IMonoBehaviourTest
        {
            private int frameCount;
            public bool IsTestFinished
            {
                get { return frameCount > 100; }
            }
            override public void Update()
            {
                base.Update();
                frameCount++;
            }
        }
    }


}

