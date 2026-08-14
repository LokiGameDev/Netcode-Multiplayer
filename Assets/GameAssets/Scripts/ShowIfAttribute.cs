using UnityEngine;

public class ShowIfAttribute : PropertyAttribute
{
    public string condition;
    public ShowIfAttribute(string condition)
    {
        this.condition = condition;
    }
}
