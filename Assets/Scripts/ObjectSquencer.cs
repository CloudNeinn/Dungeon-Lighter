using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSequencer : MonoBehaviour
{
    [SerializeField] private GameObject[] _objects;
    [SerializeField] private float _interval = 0.5f;
    [SerializeField] private float _startDelay;
    [SerializeField] private bool _loop = true;
    [SerializeField] private float _loopDelay;

    private readonly List<ISequenceable> _sequenceables = new();

    void Awake()
    {
        foreach (var obj in _objects)
        {
            if (obj && obj.TryGetComponent<ISequenceable>(out var s))
                _sequenceables.Add(s);
        }
    }

    void Start() => StartCoroutine(RunSequence());

    private IEnumerator RunSequence()
    {
        if (_sequenceables.Count == 0)
        {
            Debug.LogWarning("No valid objects.", this);
            yield break;
        }

        if (_startDelay > 0f) yield return new WaitForSeconds(_startDelay);

        do
        {
            foreach (var s in _sequenceables)
            {
                s.Activate();
                yield return new WaitForSeconds(_interval);
            }
            if (_loopDelay > 0f) yield return new WaitForSeconds(_loopDelay);
            else yield return null;
        } while (_loop);
    }
}