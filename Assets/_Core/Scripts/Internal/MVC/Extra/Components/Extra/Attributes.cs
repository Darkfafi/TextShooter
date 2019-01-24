using System;

// Tags

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class TagsHolderAttribute : Attribute
{

}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class TagFieldAttribute : Attribute
{

}

// Editor Debug Tools

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class ModelEditorFieldAttribute : Attribute
{

}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class ModelEditorMethodAttribute : Attribute
{

}