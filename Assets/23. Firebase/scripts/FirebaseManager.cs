using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance { get; private set; }

    public FirebaseApp App { get; private set; }
    public FirebaseAuth Auth { get; private set; }
    public FirebaseDatabase DB { get; private set; }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    private async void Start()
    {
        DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync(); // 파이어베이스 초기화 상태 체크, 비동기 함수이므로 완료시까지 대기

        if(status == DependencyStatus.Available)
        {
            App = FirebaseApp.DefaultInstance;
            Auth = FirebaseAuth.DefaultInstance;
            DB = FirebaseDatabase.DefaultInstance;
        }
        else
        {
            Debug.LogWarning($"파이어베이스 초기화 실패 : {status}");
        }

    }

    public async void Create(string email, string password, Action<FirebaseUser> callback = null)
    {
        try
        {
            var result = await Auth.CreateUserWithEmailAndPasswordAsync(email, password);

            callback?.Invoke(result.User);
        }
        catch (FirebaseException e)
        { 
            Debug.LogWarning(e.Message);
        }
    }

    public async void SignIn(string email, string password, Action<FirebaseUser> callback = null)
    {
        try
        {
            var result = await Auth.SignInWithEmailAndPasswordAsync(email, password);

            callback?.Invoke(result.User);
        }
        catch (FirebaseException e)
        {
            Debug.LogWarning(e.Message);
        }
    }

}
