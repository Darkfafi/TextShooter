using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField]
    private TurretView turretViewInstance;

    [SerializeField]
    private SoldierView targetViewInstance;

    protected void Awake()
    {
        var turretController = EntityManager.Instance.CreateEntity(new TurretModel(), turretViewInstance);
        var targetController = EntityManager.Instance.CreateEntity(new SoldierModel(), targetViewInstance);


        SoldierModel soldier = EntityManager.Instance.GetAnEntity<SoldierModel>();

        turretController.Model.FocusOnTarget(soldier);

        soldier.DestroyEvent += OnDestroyEvent;
    }

    private void OnDestroyEvent(EntityModel e)
    {
        e.DestroyEvent -= OnDestroyEvent;
        MVCUtil.GetModel<TurretModel>(turretViewInstance).FocusOnTarget(null);
    }
}
