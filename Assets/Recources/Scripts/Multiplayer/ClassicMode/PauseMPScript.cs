using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PauseMPScript : MonoBehaviour
{
    [SerializeField] private TMP_Text PingText;
    [SerializeField] private TMP_Text RegionText;
    [SerializeField] private Image LowPingImage;
    [SerializeField] private Image MediumPingImage;
    [SerializeField] private Image HighPingImage;
    [SerializeField] private TMP_Text WaitingText;

    private void FixedUpdate()
    {
        if (PhotonNetwork.IsConnected)
        {
            PingText.text = PhotonNetwork.GetPing().ToString() + "мс";
            RegionText.text = "Сервер: " + PhotonNetwork.CloudRegion;
            if (PhotonNetwork.GetPing() <= 50)
            {
                LowPingImage.color = new Color(0, 1, 0, 1);
                MediumPingImage.color = new Color(0, 1, 0, 1);
                HighPingImage.color = new Color(0, 1, 0, 1);
            }
            else if (PhotonNetwork.GetPing() > 50 && PhotonNetwork.GetPing() <= 100)
            {
                LowPingImage.color = new Color(1, 1, 0, 1);
                MediumPingImage.color = new Color(1, 1, 0, 1);
                HighPingImage.color = new Color(1, 1, 1, 0);
            }
            else
            {
                LowPingImage.color = new Color(1, 0, 0, 1);
                MediumPingImage.color = new Color(1, 0, 0, 0);
                HighPingImage.color = new Color(1, 0, 0, 0);
            }
        }
    }
    public void wait()
    {
        StartCoroutine(AddDots());
    }
    IEnumerator AddDots()
    {
        while (true)
        {
            WaitingText.text = "очікування";
            yield return new WaitForSeconds(1f);
            WaitingText.text = "очікування.";
            yield return new WaitForSeconds(1f);
            WaitingText.text = "очікування..";
            yield return new WaitForSeconds(1f);
            WaitingText.text = "очікування...";
            yield return new WaitForSeconds(1f);
        }
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
}


