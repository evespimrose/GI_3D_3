using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISignUp : MonoBehaviour
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
        if (password.text.Length < 6)
        {
            UIManager.Instance.PageOpen<UIDialog>().SetDialog("�˸�", "��й�ȣ�� 6���� �̻�.", DialogCallback);
        }
        else
        {
            FirebaseManager.Instance.Create(email.text, password.text, CreateCallback);
        }
    }

    private void SignInButtonClick()
    {
        UIManager.Instance.PageOpen("UISignIn");
    }

    private void DialogCallback()
    {
        UIManager.Instance.PageOpen(GetType().Name);
    }

    private void CreateCallback(FirebaseUser user)
    {
        UIManager.Instance.PageOpen<UIDialog>().SetDialog("ȸ������ �Ϸ�", "ȸ�� ������ �Ϸ�Ǿ����ϴ�.\n�α��� ���ּ���.", DialogCallback);
    }
}
