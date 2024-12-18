using Firebase;
using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseTest : MonoBehaviour
{
    // Start is called before the first frame update
    private async void Start()
    {
        DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();
        print(status);

        try
        {
            AuthResult result = await FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync("1@1.1", "111111");

            print(result.User.UserId);
        }
        catch
        {
            print("FireBase 로그인 실패!");
        }
    }

}
