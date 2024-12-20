using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;


public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance { get; private set; }

    public FirebaseApp App { get; private set; }
    public FirebaseAuth Auth { get; private set; }
    public FirebaseDatabase DB { get; private set; }

    private DatabaseReference usersRef;
    private DatabaseReference messageRef;

    public event Action onLogIn;

    public UserData currentUserData {  get; private set; }


    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        onLogIn += OnLogIn;
    }

    private void OnLogIn()
    {
        messageRef = DB.GetReference($"msg/{Auth.CurrentUser.UserId}");

        messageRef.ChildAdded += OnMessageReceive;
    }

    private void OnMessageReceive(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError == null) // ���� ����
        {
            string rawJson = args.Snapshot.GetRawJsonValue();

            Message message = JsonConvert.DeserializeObject<Message>(rawJson);

            var popup = UIManager.Instance.PopupOpen<UIDialogPopup>();
            popup.SetPopup($"From.{message.sender}", $"{message.message}\n{message.GetSendTime()}");

        }
        else                            // ���� �߻�
        {
            print("���� �߻�!");
        }
    }

    public void MessageToTarget(string target, Message message)
    {
        DatabaseReference targetRef = DB.GetReference($"msg/{target}");
        string messageJson = JsonConvert.SerializeObject(message);
        targetRef.Child(message.sender + message.sendTime).SetRawJsonValueAsync(messageJson);

    }

    private async void Start()
    {
        //���̾�̽� �ʱ�ȭ ���� üũ. �񵿱�(Async)�Լ��̹Ƿ� �Ϸ�� �� ���� ���
        DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();
        //�ʱ�ȭ ����
        if (status == DependencyStatus.Available)
        {
            App = FirebaseApp.DefaultInstance;
            Auth = FirebaseAuth.DefaultInstance;
            DB = FirebaseDatabase.DefaultInstance;

            DataSnapshot dummyData = await DB.GetReference("users").Child("dummy").GetValueAsync();
            
            if(dummyData.Exists)
            {
                print(dummyData.GetRawJsonValue());
            }
        }
        //�ʱ�ȭ ����
        else
        {
            Debug.LogWarning($"���̾�̽� �ʱ�ȭ ���� : {status}");
        }
    
    }

    //ȸ������ �Լ�
    public async void Create(string email, string passwd, Action<FirebaseUser> callback = null)
    {
        try
        {
            var result = await Auth.CreateUserWithEmailAndPasswordAsync(email, passwd);

            // ������ ȸ���� Database Reference�� ����
            usersRef = DB.GetReference($"users/{result.User.UserId}");

            // ȸ���� �����͸� Database�� ����
            UserData userData = new UserData(result.User.UserId);

            string userDataJson = JsonConvert.SerializeObject(userData);

            await usersRef.SetRawJsonValueAsync(userDataJson);

            callback?.Invoke(result.User);
        }
        catch(FirebaseException e)
        {
            Debug.LogError(e.Message);
        }
    }
    //�α���
    public async void SignIn(string email, string passwd, Action<FirebaseUser, UserData> callback = null)
    {
        try
        {
            var result = await Auth.SignInWithEmailAndPasswordAsync(email, passwd);

            usersRef = DB.GetReference($"users/{result.User.UserId}");

            DataSnapshot userDataValues = await usersRef.GetValueAsync();
            UserData userData = null;
            if(userDataValues.Exists)
            {
                string json = userDataValues.GetRawJsonValue();
                userData = JsonConvert.DeserializeObject<UserData>(json);
            }
            currentUserData = userData;

            callback?.Invoke(result.User, userData);

            onLogIn?.Invoke();
        }
        catch (FirebaseException e)
        {
            UIManager.Instance.PopupOpen<UIDialogPopup>().SetPopup("�α��� ����", "�̸��� �Ǵ� ��й�ȣ�� Ʋ�Ƚ��ϴ�.");
            Debug.LogError(e.Message);
        }
    }
    //���� ���� ����
    public async void UpdateUserProfile(string displayName, Action<FirebaseUser> callback = null)
    {
        //userProfile����
        UserProfile profile = new UserProfile() { DisplayName = displayName, PhotoUrl = new Uri("https://picsum.photos/120"), };
        await Auth.CurrentUser.UpdateUserProfileAsync(profile);

        UpdateUserData("userName", displayName);

        DataSnapshot userDataValues = await usersRef.GetValueAsync();
        UserData userData = null;
        if (userDataValues.Exists)
        {
            string json = userDataValues.GetRawJsonValue();
            userData = JsonConvert.DeserializeObject<UserData>(json);
        }
        currentUserData = userData;

        callback?.Invoke(Auth.CurrentUser);
    }
    // ���� ������ ����
    public async void UpdateUserData(string childName, object value, Action<object> callback = null)
    {
        DatabaseReference targetRef = usersRef.Child(childName);

        await targetRef.SetValueAsync(value);
        DataSnapshot snapshot = await usersRef.GetValueAsync();
        

        callback?.Invoke(value);
    }

    public void SignOut()
    {
        Auth.SignOut();
    }

    public async void OrderDataBase(Action<List<UserData>> callback = null)
    {
        try
        {
            // �����͸� level�� �������� �����Ͽ� ��������
            Query query = DB.GetReference("users").OrderByChild("level");
            DataSnapshot snapshot = await query.GetValueAsync();

            if (snapshot.Exists)
            {
                List<UserData> userList = new List<UserData>();

                foreach (DataSnapshot child in snapshot.Children)
                {
                    try
                    {
                        string json = child.GetRawJsonValue();
                        if (!string.IsNullOrEmpty(json))
                        {
                            UserData userData = JsonConvert.DeserializeObject<UserData>(json);
                            userList.Add(userData);
                        }
                        else
                        {
                            Debug.LogWarning("Empty or null JSON detected for a user.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Failed to parse user data: {ex.Message}");
                    }
                }

                userList.Sort((a, b) => b.level.CompareTo(a.level));

                callback?.Invoke(userList);
            }
            else
            {
                Debug.Log("No data found in the database.");
            }
        }
        catch (FirebaseException e)
        {
            Debug.LogError($"Failed to retrieve and sort data: {e.Message}");
        }
    }
}
