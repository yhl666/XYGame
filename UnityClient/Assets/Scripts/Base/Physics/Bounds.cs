/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com
way 1.use unity's physicsX to process collider
way 2. 
 */
using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// wrapper of bounds
/// </summary>
public class BoundsImpl
{
    Bounds impl;

    public const float DEFAULT_Z = 0.1f;
    private BoundsImpl(Vector3 center, Vector3 size)
    {
        impl = new Bounds(center, size);
    }
    private BoundsImpl(Vector2 center, Vector2 size)
    {
        impl = new Bounds(new Vector3(center.x, center.y, DEFAULT_Z), new Vector3(size.x, size.y, DEFAULT_Z));
    }
    private BoundsImpl()
    {
        impl = new Bounds();
    }
    public static BoundsImpl Create()
    {
        BoundsImpl ret = new BoundsImpl();
        return ret;
    }
    public static BoundsImpl Create(Vector3 center, Vector3 size)
    {
        BoundsImpl ret = new BoundsImpl(center, size);
        return ret;
    }
    public static BoundsImpl Create(Vector2 center, Vector2 size)
    {
        BoundsImpl ret = new BoundsImpl(center, size);
        return ret;
    }
    public static bool operator !=(BoundsImpl lhs, BoundsImpl rhs)
    {
        return lhs.impl != rhs.impl;
    }
    public static bool operator ==(BoundsImpl lhs, BoundsImpl rhs)
    {
        return lhs.impl == rhs.impl;
    }

    // 摘要: 
    //     The center of the bounding box.
    public Vector3 center
    {
        get
        {
            return impl.center;
        }
        set
        {
            impl.center = value;
        }
    }
    //
    // 摘要: 
    //     The extents of the box. This is always half of the size.
    public Vector3 extents
    {
        get
        {
            return impl.extents;
        }
        set
        {
            impl.extents = value;
        }
    }
    //
    // 摘要: 
    //     The maximal point of the box. This is always equal to center+extents.
    public Vector3 max
    {
        get
        {
            return impl.max;
        }
        set
        {
            impl.max = value;
        }
    }
    //
    // 摘要: 
    //     The minimal point of the box. This is always equal to center-extents.
    public Vector3 min
    {
        get
        {
            return impl.min;
        }
        set
        {
            impl.min = value;
        }
    }
    //
    // 摘要: 
    //     The total size of the box. This is always twice as large as the extents.
    public Vector3 size
    {
        get
        {
            return impl.size;
        }
        set
        {
            impl.size = value;
        }
    }
    public override int GetHashCode()
    {
        return impl.GetHashCode();
    }
    public bool Contains(Vector3 point)
    {
        return impl.Contains(point);
    }
    public bool Contains(Vector2 point)
    {
        return impl.Contains(new Vector3(point.x, point.y, 0f));
    }
    public bool IntersectRayImpl(RayImpl ray)
    {
        return impl.IntersectRay(ray.impl);
    }
    public bool IntersectRayImpl(RayImpl ray, out float distance)
    {
        return impl.IntersectRay(ray.impl, out distance);
    }
    public bool Intersects(BoundsImpl bounds)
    {
        return impl.Intersects(bounds.impl);
    }
    public float SqrDistance(Vector3 point)
    {
        return impl.SqrDistance(point);
    }
}