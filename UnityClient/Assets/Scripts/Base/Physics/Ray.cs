/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com
way 1.use unity's physicsX to process ray
way 2. 
 */
using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// wrapper of ray
/// </summary>
public class RayImpl
{
    public Ray impl;

    public const float DEFAULT_Z = 1.0f;

    private RayImpl(Vector3 origin, Vector3 direction)
    {
        impl = new Ray(origin, direction);
    }
    private RayImpl(Vector2 origin, Vector2 direction)
    {
        impl = new Ray(new Vector3(origin.x, origin.y, 0f), new Vector3(direction.x, direction.y, 0f));
    }
    private RayImpl()
    {
        impl = new Ray();
    }
    public static RayImpl Create()
    {
        RayImpl ret = new RayImpl();
        return ret;
    }
    public static RayImpl Create(Vector3 center, Vector3 size)
    {
        RayImpl ret = new RayImpl(center, size);
        return ret;
    }
    public static RayImpl Create(Vector2 center, Vector2 size)
    {
        RayImpl ret = new RayImpl(center, size);
        return ret;
    }

    public Vector3 origin
    {
        get
        {
            return impl.origin;
        }
        set
        {
            impl.origin = value;
        }
    }
    public Vector3 direction
    {
        get
        {
            return impl.direction;
        }
        set
        {
            impl.direction = value;
        }
    }
    public Vector3 GetPoint(float distance)
    {
        return impl.GetPoint(distance);
    }

}