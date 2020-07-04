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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipTestHarness : MonoBehaviour
{
    public GameObject spaceShip;
    public Transform target1;
    public Transform target2;

    private bool flag = true;

    private ActsAsSpaceShip actsAsSpaceShip
    {
        get
        {
            return spaceShip.GetComponent<ActsAsSpaceShip>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("moveSpaceShip", 5.0f, 5.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void moveSpaceShip()
    {
        if (flag)
        {
            this.actsAsSpaceShip.navigateTo(target1.position);
        } else
        {
            this.actsAsSpaceShip.navigateTo(target2.position);
        }
        flag = !flag;
    }
}
