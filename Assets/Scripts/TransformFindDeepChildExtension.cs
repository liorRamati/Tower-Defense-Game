using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class TransformFindDeepChildExtension
{
    /// <summary>
    /// Breadth-first search for a child object of the given parent wit the given name
    /// </summary>
    public static Transform FindDeepChild(this Transform parent, string name)
    {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(parent);
        while (queue.Count > 0)
        {
            var c = queue.Dequeue();
            if (c.name == name)
                return c;
            foreach (Transform t in c)
                queue.Enqueue(t);
        }
        return null;
    }
}
