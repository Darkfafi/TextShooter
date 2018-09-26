using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSystemView : MonoBaseView
{
    [Header("Options")]
    [SerializeField]
    private float _orthographicSpawnMargin = 1f;

    [Header("Requirements")]
    [SerializeField]
    private Camera _gameCamera;

    [SerializeField]
    private EnemyViewFactory _enemyViewFactory;

    private WaveSystemModel _waveSystemModel;

    #region LifeCycle

    protected override void OnViewReady()
    {
        _waveSystemModel = MVCUtil.GetModel<WaveSystemModel>(this);
        SetSpawnDistanceToCamBoundry();
        _waveSystemModel.SpawnEnemyEvent += OnSpawnEnemyEvent;
    }

    protected override void OnViewDestroy()
    {
        _waveSystemModel.SpawnEnemyEvent -= OnSpawnEnemyEvent;
    }

    protected void Update()
    {
        SetSpawnDistanceToCamBoundry();
    }

    #endregion

    private void SetSpawnDistanceToCamBoundry()
    {
        if (_waveSystemModel == null)
            return;

        float spawnDistY = _gameCamera.orthographicSize + _orthographicSpawnMargin;
        float spawnDistX = spawnDistY * Screen.width / Screen.height;

        _waveSystemModel.SetSpawnDistance(spawnDistX, spawnDistY);
    }

    private void OnSpawnEnemyEvent(EnemyModel enemy)
    {
        _enemyViewFactory.CreateEnemyView(enemy);
    }

}
