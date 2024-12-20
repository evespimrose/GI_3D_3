using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDialogPopup : UIPopup
{

    public void SetDialog(string title, string message, Action callback = null)
    {
        this.title.text = title;
        this.message.text = message;
        this.callback = callback;
    }

}
