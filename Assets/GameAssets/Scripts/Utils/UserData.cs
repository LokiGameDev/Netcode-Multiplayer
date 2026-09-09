using System;

/// <summary>Stores profile data sent during network connection approval.</summary>
[Serializable]
public class UserData
{
    /// <summary>Player-facing display name.</summary>
    public string userName;
    /// <summary>Unity Authentication player identifier.</summary>
    public string userAuthId;
    /// <summary>Selected player skin identifier.</summary>
    public string skinID;
}
