using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraModel : EntityModel
{
    public float OrthographicSize
    {
        get; private set;
    }

    public CameraModel(float ortographicSize)
    {
        OrthographicSize = ortographicSize;
    }
}
