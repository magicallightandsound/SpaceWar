using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MagicalLightAndSound;

public class ActsAsHelpScreen : MonoBehaviour
{
    PackagedActions.AlwaysFaceUser alwaysFaceUser;

    // Start is called before the first frame update
    void Start()
    {
        this.alwaysFaceUser = new PackagedActions.AlwaysFaceUser(this);

    }

    // Update is called once per frame
    void Update()
    {
        alwaysFaceUser.Perform();
    }
}

namespace MagicalLightAndSound
{
    interface IPackagedAction
    {
        void Perform();
    }

    public struct PackagedActions
    {
        public struct AlwaysFaceUser : IPackagedAction
        {
            private MonoBehaviour script;

            public AlwaysFaceUser(MonoBehaviour script)
            {
                this.script = script;
            }

            public void Perform()
            {
                script.transform.LookAt(Camera.main.transform.position);
            }
        }
    }
}

