using UnityEngine;

/// <summary>Displays a serialized property when another property meets a condition.</summary>
public class ShowIfAttribute : PropertyAttribute
{
    /// <summary>Name of the property used to determine visibility.</summary>
    public string condition;

    /// <summary>Creates an attribute with the controlling property name.</summary>
    public ShowIfAttribute(string condition)
    {
        this.condition = condition;
    }
}
