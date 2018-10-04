public class EntityModel : BaseModel
{
    public EntityModel()
    {
        ModelTransform = AddComponent<ModelTransform>();
        ModelTags = AddComponent<ModelTags>();
        EntityTracker.Instance.Register(this);
    }

    public ModelTags ModelTags
    {
        get; private set;
    }

    public ModelTransform ModelTransform
    {
        get; private set;
    }

    protected override void OnModelDestroy()
    {
        base.OnModelDestroy();
        ModelTags = null;
        ModelTransform = null;
    }
}