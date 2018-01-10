﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingMovement : MonoBehaviour {

    private GameObject player;
    private SteamVR_TrackedObject trackedObj;
    private static Config.Speed speed = Config.speed;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    private static Config.Speed CalculateSpeedBasedOnModelSize()
    {
        GameObject g;

        if (Config.photogrammetryModelName.Equals("undefined"))
        {
            g = GameObject.FindWithTag("Photogrammetry Model");
        } else
        {
            g = GameObject.Find(Config.photogrammetryModelName);
        }

        Vector3 modelSize = g.GetComponent<PhotogrammetryModelProperties>().GetRange();

        float modelHypot = Mathf.Sqrt(modelSize.x * modelSize.x + modelSize.y * modelSize.y + modelSize.z * modelSize.z);
        double smallModelHypot = 1e5;

        if (modelHypot < smallModelHypot)
        {
            return Config.Speed.Slow;
        } else if (modelHypot < smallModelHypot * 5)
        {
            return Config.Speed.Medium;
        } else if (modelHypot < smallModelHypot * 25)
        {
            return Config.Speed.Fast;
        } else
        {
            return Config.Speed.Lightning;
        }
    }

    public static void SpeedUp()
    {
        if (speed == Config.Speed.Slow)
        {
            speed = Config.Speed.Medium;
        } else if (speed == Config.Speed.Medium)
        {
            speed = Config.Speed.Fast;
        }
        else if (speed == Config.Speed.Fast)
        {
            speed = Config.Speed.Lightning;
        }
    }

    public static void SpeedDown()
    {
        if (speed == Config.Speed.Lightning)
        {
            speed = Config.Speed.Fast;
        }
        else if (speed == Config.Speed.Fast)
        {
            speed = Config.Speed.Medium;
        }
        else if (speed == Config.Speed.Medium)
        {
            speed = Config.Speed.Slow;
        }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }
    

    void Start()
    {
        player = GameObject.Find("Player");

        if (!Config.useConfigSpeed)
        {
            speed = CalculateSpeedBasedOnModelSize();
            Debug.Log(speed);
        }
    }

    // Update is called once per frame
    void Update()
    {

        float step = (int)speed * Time.deltaTime;

        if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {

            if (Controller.GetAxis().y > 0)
            {
                player.transform.position = Vector3.MoveTowards(player.transform.position, player.transform.position + trackedObj.transform.forward, step);
            } else
            {
                player.transform.position = Vector3.MoveTowards(player.transform.position, player.transform.position - trackedObj.transform.forward, step);
            }     
        } 
    }
}
