using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.MagicLeap;

using MagicalLightAndSound;
using System;

public class ActsAsSceneLoader : MonoBehaviour
{
    public string nextScene;
    
    [HideInInspector]
    bool loadNextScene = false;

    private void Awake()
    {
        ActsAsManipulatable actsAsManipulatable = GetComponent<ActsAsManipulatable>();
        Debug.Assert(actsAsManipulatable != null, "ActsAsManipulatable is required");

        actsAsManipulatable.OnTrigger += OnTrigger;
    }

    private void OnTrigger(byte controllerId, float triggerValue)
    {
        loadNextScene = true;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!loadNextScene)
        {
            return;
        }
        SceneManager.LoadScene(nextScene);
        loadNextScene = false;
    }

    private void OnDestroy()
    {
        ActsAsManipulatable actsAsManipulatable = GetComponent<ActsAsManipulatable>();
        Debug.Assert(actsAsManipulatable != null, "ActsAsManipulatable is required");

        actsAsManipulatable.OnTrigger -= OnTrigger;
    }
}
