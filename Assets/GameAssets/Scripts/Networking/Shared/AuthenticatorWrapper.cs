using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Friends;
using UnityEngine;

public static class AuthenticatorWrapper
{
    public static AuthState AuthState { get; private set; } = AuthState.NotAuthenticated;

    public static async Task<AuthState> DoAuthorize(int maxRetries = 5)
    {
        if(AuthState == AuthState.Authenticated) return AuthState;

        if(AuthState == AuthState.Authenticating)
        {
            await Authenticating();
            return AuthState;
        }

        await SignInAnonymouslyAsync(maxRetries);

        await GetFriendsListAsync();

        return AuthState;
    }

    private static async Task Authenticating()
    {
        while(AuthState==AuthState.Authenticating || AuthState==AuthState.NotAuthenticated)
        {
            await Task.Delay(100);
        }
        return;
    }

    private static async Task SignInAnonymouslyAsync(int maxRetries)
    {
        AuthState = AuthState.Authenticating;

        int retries = 0;

        while(AuthState == AuthState.Authenticating && retries < maxRetries)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if(AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    if(PlayerPrefs.GetInt("PlayerAuthenticated")==0)
                    {
                        await AuthenticationService.Instance.UpdatePlayerNameAsync(PlayerPrefs.GetString("PlayerName", "Default"));
                        PlayerPrefs.SetInt("PlayerAuthenticated", 1);
                    }

                    AuthState = AuthState.Authenticated;
                    break;
                }
            }
            catch(Exception e)
            {
                Debug.LogError(e);
                AuthState = AuthState.Error;
            }

            retries++;

            await Task.Delay(1000);
        }

        if(AuthState == AuthState.Authenticating)
        {
            AuthState = AuthState.TimeOut;
        }

        return;
    }

    private static async Task GetFriendsListAsync()
    {
        try
        {
            await UnityServices.InitializeAsync();

            await FriendsService.Instance.InitializeAsync();
        }
        catch(Exception e)
        {
            Debug.Log(e);
            return;
        }
        return;
    }
}

public enum AuthState
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    TimeOut
}
