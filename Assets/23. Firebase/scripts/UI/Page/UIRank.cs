using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIRank : UIPage
{
    public List<UserData> userDatas;
    public GameObject rank1st;
    public GameObject rank2nd;
    public GameObject rank3rd;
    public GameObject rank4th;

    public Transform scrollViewPoint;

    public Button closeButton;

    private void Start()
    {
        FirebaseManager.Instance.OrderDataBase((userDatas) => UIRankPopUpOpen(userDatas));
    }

    public void UIRankPopUpOpen(List<UserData> userDatas)
    {
        foreach (var (userData, index) in userDatas.Select((userData, index) => (userData, index)))
        {
            AddItem(index, userData.userName, userData.userClass.ToString() ,userData.level);
        }
    }

    public void AddItem(int index, string userName, string ClassName, int level)
    {
        GameObject newItem = null;
        switch (index)
        {
            case 0:
                newItem = Instantiate(rank1st, scrollViewPoint);
                break;
            case 1:
                newItem = Instantiate(rank2nd, scrollViewPoint);
                break;
            case 2:
                newItem = Instantiate(rank3rd, scrollViewPoint);
                break;
            default:
                newItem = Instantiate(rank4th, scrollViewPoint);
                break;
        }

        if (newItem != null && newItem.TryGetComponent(out RankContainer rc))
        {
            rc.userName.text = userName;
            rc.level.text = level.ToString();
            if(index >= 3)
            {
                rc.rank.text = (index + 1).ToString();
            }
        }
    }
}
