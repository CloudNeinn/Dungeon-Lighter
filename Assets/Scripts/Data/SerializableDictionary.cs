using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerializableDictionary<TKey, Tvalue> : Dictionary<TKey, Tvalue>, ISerializationCallbackReceiver
{
    public List<TKey> keys = new List<TKey>();

    public List<Tvalue> values = new List<Tvalue>();

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (KeyValuePair<TKey, Tvalue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear();

        if(keys.Count != values.Count)
        {
            Debug.LogError("Amount of keys: " + keys.Count + " not equal to amout of values:" + values.Count);
            Debug.LogError("Key: " + keys[0]);
            Debug.LogError("Key: " + keys[1]);
            Debug.LogError("Key: " + keys[2]);
            Debug.LogError("Key: " + keys[3]);
            Debug.LogError("Key: " + keys[4]);
        }

        for(int i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);
        }
    }


}