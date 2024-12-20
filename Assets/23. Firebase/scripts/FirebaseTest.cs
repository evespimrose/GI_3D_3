using Firebase;
using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseTest : MonoBehaviour
{
    private async void Start()
    {
        DependencyStatus status =  await FirebaseApp.CheckAndFixDependenciesAsync();
        print(status);
        var result = await FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync("asd@asd.asd","a123456");
        print(result.User.UserId);
    }
}
