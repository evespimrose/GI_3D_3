using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHome : UIPage
{
    public Image profileImage;
    public TextMeshProUGUI displayName;
    public TextMeshProUGUI gold;
    public TextMeshProUGUI exp;
    public TextMeshProUGUI level;
    public TextMeshProUGUI jem;

    public Button profileChangeButton;
    public Button goldAddButton;
    public Button signOutButton;
    public Button huntButton;
    public Button jemAddButton;
    public Button rankButton;
    public Button messageButton;
    public Button inviteButton;

    public Board gameBoard;


    public Image expFillImage;

    private string messageTarget;

    private Room currentRoom;

    private void Awake()
    {
        profileChangeButton.onClick.AddListener(ProfileChangeButtonClick);
        goldAddButton.onClick.AddListener(AddGoldButtonClick);
        jemAddButton.onClick.AddListener(AddJemButtonClick);
        signOutButton.onClick.AddListener(SignOutButtonClick);
        huntButton.onClick.AddListener(HuntButtonClick);
        rankButton.onClick.AddListener(RankButtonClick);
        messageButton.onClick.AddListener(MessageButtonClick);
        inviteButton.onClick.AddListener(InviteButtonClick);

        gameBoard.gameObject.SetActive(false);
    }

    private void Start()
    {
        FirebaseManager.Instance.onGameStart += GameStart;
        FirebaseManager.Instance.onTurnProcceed += ProcessTurn;
    }

    public void GameStart(Room room, bool isHost)
    {
        currentRoom = room;
        gameBoard.isHost = isHost;
        gameBoard.gameObject.SetActive(true);
    }

    public void ProcessTurn(Turn turn)
    {
        gameBoard.turnCount++;
        //새로운 턴 입력이 추가 될 때마다 호출.


        gameBoard.PlaceMark(turn.isHostTurn, turn.coodinate);

        gameBoard.isMyTurn = turn.isHostTurn != gameBoard.isHost;

        bool result = gameBoard.isGameOver(gameBoard.isMyTurn);
        if (result)
        {
            //오목 한 개 이상 있음
        }
        else
        {
            // 오목 없음
        }
    }

    private void InviteButtonClick()
    {
        var popup = UIManager.Instance.PopupOpen<UIInputFieldPopup>();
        popup.SetPopup("초대하기", "누구를 초대하시겠습니까?", InviteTarget);
    }

    private async void InviteTarget(string target)
    {
        Room room = new Room()
        {
            host = FirebaseManager.Instance.Auth.CurrentUser.UserId,
            guest = target,
            state = RoomState.Waiting,
        };

        await FirebaseManager.Instance.CreateRoom(room);

        Message message = new Message()
        {
            type = MessageType.Invite,
            sender = FirebaseManager.Instance.Auth.CurrentUser.UserId,
            message = "",
            sendTime = DateTime.Now.Ticks
        };

        await FirebaseManager.Instance.MessageToTarget(target, message);
    }

    private void MessageButtonClick()
    {
        var popup = UIManager.Instance.PopupOpen<UIInputFieldPopup>();
        popup.SetPopup("메시지 보내기", "누구에게 메시지를 보내시겠습니까?", SetMessageTarget);
    }

    private void SetMessageTarget(string target)
    {
        messageTarget = target;
        var popup = UIManager.Instance.PopupOpen<UIInputFieldPopup>();
        popup.SetPopup($"To.{messageTarget}", "뭐라고 메시지를 보내시겠습니까?", MessageToTarget);
    }

    private void MessageToTarget(string messageText)
    {
        Message message = new Message()
        {
            type = MessageType.Message,
            sender = FirebaseManager.Instance.Auth.CurrentUser.UserId,
            message = messageText,
            sendTime = DateTime.Now.Ticks

        }; 

        FirebaseManager.Instance.MessageToTarget(messageTarget, message);
    }

    public void ProfileChangeButtonClick()
    {
        UIManager.Instance.PopupOpen<UIInputFieldPopup>().
            SetPopup("닉네임 변경", "변경할 닉네임 입력.", ProfileChangeCallback);
    }
    private void ProfileChangeCallback(string newName)
    {
        FirebaseManager.Instance.UpdateUserProfile(newName, SetInfo);
    }

    public void SetInfo(FirebaseUser user)
    {
        displayName.text = user.DisplayName;
        
        if (user.PhotoUrl != null)
        {
            SetPhoto(user.PhotoUrl.AbsoluteUri);
        }
        else
            SetPhoto("");
    }

    public async void SetPhoto(string url)
    {
        if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
        {
            using (HttpClient client = new HttpClient())
            {
                byte[] response = await client.GetByteArrayAsync(url);
                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(response);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f));
                profileImage.sprite = sprite;
            }
        }
        else
        {
            profileImage.sprite = null;
        }
    }

    public void SetUserData(UserData userData)
    {
        gold.text = userData.gold.ToString();
        jem.text = userData.jem.ToString();
        level.text = userData.level.ToString();
        exp.text = (userData.exp % 100).ToString();

        if (expFillImage.type == Image.Type.Filled)
        {
            expFillImage.fillAmount = (userData.exp % 100) / 100f;
        }
    }

    private void AddGoldButtonClick()
    {
        UserData data = FirebaseManager.Instance.currentUserData;
        data.gold += 10;

        FirebaseManager.Instance.UpdateUserData("gold", data.gold, (x) => { SetUserData(data); });
    }


    private void AddJemButtonClick()
    {
        UserData data = FirebaseManager.Instance.currentUserData;
        data.jem += 1;

        FirebaseManager.Instance.UpdateUserData("jem", data.jem, (x) => { SetUserData(data); });
    }

    private void SignOutButtonClick()
    {
        FirebaseManager.Instance.SignOut();
        UIManager.Instance.PageOpen<UIMain>();
    }


    private void HuntButtonClick()
    {
        UserData data = FirebaseManager.Instance.currentUserData;
        data.exp += 1;

        if ((data.exp / 100) >= data.level)
        {
            data.level += 1;
            FirebaseManager.Instance.UpdateUserData("level", data.level, (x) => { SetUserData(data); });
        }

        FirebaseManager.Instance.UpdateUserData("exp", data.exp, (x) => { SetUserData(data); });
    }


    private void RankButtonClick()
    {
        UIManager.Instance.PageOpen<UIRank>();
    }

}
