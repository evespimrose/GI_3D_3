using Firebase.Auth;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHome : MonoBehaviour
{
    public TextMeshProUGUI info;

    public Button signInButton;

    private void Awake()
    {
        signInButton.onClick.AddListener(SignInButtonClick);
    }

    private void SignUpButtonClick()
    {
        UIManager.Instance.PageOpen("UISignUp");
    }

    private void SignInButtonClick()
    {
        UIManager.Instance.PageOpen("UISignIn");
    }


    public void SetInfo(FirebaseUser user)
    {
        info.text = $"�̸��� : {user.Email}\n�̸� : ";
    }

}
