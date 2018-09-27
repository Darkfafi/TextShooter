public class EntityModel : BaseModel
{
    public EntityTransform Transform { get; protected set; }

    public EntityModel()
    {
        Transform = new EntityTransform();
        EntityTracker.Instance.Register(this);
    }
}