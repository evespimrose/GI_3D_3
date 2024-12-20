using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserData
{
    public enum UserClass
    {
        Warrior,
        Wizard,
        Rogue,
        Archer
    }

    public string userId {  get; set; }
    public string userName;
    public int level;
    public int exp;
    public int gold;
    public int jem;
    public UserClass userClass;

    public UserData() { }
    public UserData(string userId)
    {
        this.userId = userId;
        userName = "무명의 전사";
        level = 1;
        exp = 0;
        gold = 0;
        jem = 0;
        userClass = UserClass.Warrior;
    }
    public UserData(string userId, string userName, int level, int exp, int gold, int jem, UserClass userClass)
    {
        this.userId = userId;
        this.userName = userName;
        this.level = level;
        this.exp = exp;
        this.gold = gold;
        this.jem = jem;
        this.userClass = userClass;
    }
}
