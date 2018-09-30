public class EntityModel : BaseModel, IModelTransformHolder, IModelTagsHolder
{
    public EntityModel()
    {
        ModelTransform = new ModelTransform();
        ModelTags = new ModelTags(this);
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
        ModelTags.Clean();
        ModelTags = null;
        ModelTransform = null;
    }
}