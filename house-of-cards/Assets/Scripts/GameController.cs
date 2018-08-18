using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public PointCloudManager manager;

    public float initalWaitTime = 14.7f;
    public bool initialLoad = true;

    private int index = 1; // starting frame
    private int endFrame = 1450; // 2101

    private float frameRate = 30.0f;
    private float waiting = 3.0f;

    private GameObject mesh;

    // 0 = culdesac (loading)
    // 1 = face
    // 2 = city
    private int state = 0;

    void Start()
    {
        manager.dataPath = "/Data/culdesac";

        if (initialLoad)
        {
            waiting = 0.0f;
            state = 1;
        }

        manager.load();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case 0:
                waiting += Time.deltaTime;
                print(waiting);

                if (waiting > initalWaitTime)
                {
                    state++;
                    waiting = 0.0f;

                    manager.dataPath = "/Data/" + index;
                    manager.load();
                }

                break;
            case 1:
                waiting += Time.deltaTime;

                if (manager.loaded)
                {
                    if (initialLoad)
                    {
                        index++;
                    }
                    else
                    {
                        index = ((int)Math.Floor(waiting * frameRate)) + 1;
                    }

                    if (index > endFrame)
                    {
                        index = 1;
                        state++;

                        manager.dataPath = "/Data/city";
                        manager.load();
                        break;
                    }

                    manager.dataPath = "/Data/" + index;
                    manager.load();
                }
                break;
            case 2:
                if (manager.loaded && manager.pointCloud != null)
                {
                    if (mesh == null)
                    {
                        mesh = manager.pointCloud;
                    }

                    var position = mesh.transform.position.x;

                    if (position > 300.0f)
                    {
                        state = 1;

                        manager.dataPath = "/Data/" + index;
                        manager.load();
                    }

                    mesh.transform.position = new Vector3(position + 0.1f, 0.0f, 0.0f);
                }
                break;
        }

    }
}
