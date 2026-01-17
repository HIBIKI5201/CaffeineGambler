using DG.Tweening;
using System;
using UnityEngine;

public class EnterEffect : MonoBehaviour
{
    [SerializeField] private GameObject _pokerObject;
    [SerializeField] private Vector3 _targetPosition;
    [SerializeField] private Ease _easeType = Ease.OutBounce;
    [SerializeField] private Transform _startPosition;

    private void Start()
    {
        _pokerObject.transform.position = _startPosition.position;
        //Entry();
    }

    public void Entry()
    {
        _pokerObject.transform.DOMove(_targetPosition, 1f)
                                .SetEase(_easeType);
    }
}
