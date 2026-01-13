using System;
using Unity.Services.Authentication;
using Unity.Services.Multiplayer;
using UnityEngine;
using Unity.Services.Core;


public class SessionManager : MonoBehaviour
{
    [SerializeField] int m_MaxPlayers = 10;
    [SerializeField] string sessionName = "Session1";
    ISession m_Session;

    private bool hasJoined = false;


    async void Awake()
    {
        await UnityServices.InitializeAsync();
    }

    void Start()
    {
    }

    public async void Join()
    {
        if (!hasJoined) return;
        hasJoined = true;

        try
        {
            // Only sign in if not already signed in.
            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

            // Set the session options.
            var options = new SessionOptions()
            {
                Name = sessionName,
                MaxPlayers = m_MaxPlayers
            }.WithDistributedAuthorityNetwork();

            // Join a session if it already exists, or create a new one.
            m_Session = await MultiplayerService.Instance.CreateOrJoinSessionAsync(sessionName, options);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public async void Leave()
    {
        if (m_Session != null)
            await m_Session.LeaveAsync();
    }
}
