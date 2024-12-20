using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISignIn : UIPage
{
    public TMP_InputField email;
    public TMP_InputField passwd;

    public Button signUpButton;
    public Button signInButton;

    private void Awake()
    {
        signUpButton.onClick.AddListener(SignUpButtonClick);
        signInButton.onClick.AddListener(SignInButtonClick);

    }
    private void SignUpButtonClick()
    {
        UIManager.Instance.PageOpen<UISignUP>();
    }
    private void SignInButtonClick()
    {
        FirebaseManager.Instance.SignIn(email.text, passwd.text, (user, userData) =>
        {
            UIManager.Instance.PageOpen<UIHome>().SetInfo(user);
            UIManager.Instance.PageOpen<UIHome>().SetUserData(userData);
        });
    }
}
