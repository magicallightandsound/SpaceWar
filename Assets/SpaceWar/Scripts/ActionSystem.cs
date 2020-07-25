using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MagicalLightAndSound
{
    namespace ActionSystem
    {
        public interface IActionable
        {
            GameObject gameObject { get; }
        }

        public struct Action
        {
            public enum Type
            {
                None,
                LoadScene
            }
            Type type;

            public enum PropertyType
            {
                NextScene 
            }

            Dictionary<PropertyType, string> properties;

            public Action(Type type, Dictionary<PropertyType, string> properties)
            {
                this.type = type;
                this.properties = properties;
            }

            public void Perform()
            {
                switch (this.type)
                {
                    case Type.None:
                        break;
                    case Type.LoadScene:
                        string nextScene = properties[PropertyType.NextScene];
                        SceneManager.LoadScene(nextScene);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}