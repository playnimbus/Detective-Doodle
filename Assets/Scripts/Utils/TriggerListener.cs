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
        if(collider == null)
        {
            foreach (TriggerListener child in GetComponentsInChildren<TriggerListener>())
            {
                if (child != this) // GetComponentsInChildren returns the component in this GO too!
                {
                    child.onTriggerEntered += OnChildTriggerEnter;
                    child.onTriggerExited += OnChildTriggerExit;
                }
            }

            colliderContactCount = new Dictionary<Collider, int>();
        }
        else collider.isTrigger = true;
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