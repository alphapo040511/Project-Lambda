using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveObject
{
    public string UniqueId { get; }

    public ObjectState State { get; }

    public void SetObjectState(string id, ObjectState state);
}
