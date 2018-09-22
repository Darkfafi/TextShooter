using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyViewFactory : MonoBehaviour
{
    [SerializeField]
    private EnemyView _enemyPrefab;

    public EnemyView CreateEnemyView(EnemyModel enemyModel)
    {
        EnemyView enemyView = Object.Instantiate(_enemyPrefab);
        EntityManager.Instance.LinkAndRegisterEntity(enemyModel, enemyView);
        return enemyView;
    }
}
