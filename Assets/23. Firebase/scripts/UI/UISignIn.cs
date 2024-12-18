using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISignIn : MonoBehaviour
{
    public TMP_InputField email;
    public TMP_InputField password;

    public Button signUpButton;
    public Button signInButton;

    private void Awake()
    {
        signInButton.onClick.AddListener(SignInButtonClick);
        signUpButton.onClick.AddListener(SignUpButtonClick);
    }

    private void SignUpButtonClick()
    {
        UIManager.Instance.PageOpen("UISignUp");
    }

    private void SignInButtonClick()
    {
        FirebaseManager.Instance.SignIn(email.text, password.text, (user) => { UIManager.Instance.PageOpen<UIHome>().SetInfo(user); });
    }
}
