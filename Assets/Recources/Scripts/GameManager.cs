using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TMP_Text versionText;
    static public int score = 0;
    private void Awake() {
        Application.targetFrameRate = 3000;
        versionText.text = "v." + Application.version;
    }

}
