using UnityEngine;
using System.Collections.Generic;
using System;

// Either manages children collision to make them act as one collider
// or is a leaf collider
public class TriggerListener : MonoBehaviour
{
    public Action<Collider> onTriggerEntered;
    public Action<Collider> onTriggerExited;
    Dictionary<Collider, int> colliderContactCount;
    
    void Start()
    {
        // If we have no collider it means this is set up to be an internal node that listens to its direct children
        if(GetComponent<Collider>() == null)
        {
            foreach (Transform t in transform)
            {
                TriggerListener child;
                if( (child = t.GetComponent<TriggerListener>()) != null)
                {
                    child.onTriggerEntered += OnChildTriggerEnter;
                    child.onTriggerExited += OnChildTriggerExit;
                }
            }

            colliderContactCount = new Dictionary<Collider, int>();
        }
        else GetComponent<Collider>().isTrigger = true;
    }

    void OnChildTriggerEnter(Collider other)
    {
        if (!colliderContactCount.ContainsKey(other))
        {
            if (onTriggerEntered != null) onTriggerEntered(other);
            colliderContactCount.Add(other, 1);
        }
        else
        {
            int count = colliderContactCount[other];
            colliderContactCount[other] = count + 1;
        }
    }

    void OnChildTriggerExit(Collider other)
    {
        int count = colliderContactCount[other];
        count--;
        colliderContactCount[other] = count;

        if(count == 0)
        {
            if (onTriggerExited != null) onTriggerExited(other);
            colliderContactCount.Remove(other);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (onTriggerEntered != null) onTriggerEntered(other);
    }

    void OnTriggerExit(Collider other)
    {
        if (onTriggerExited != null) onTriggerExited(other);
    }
}