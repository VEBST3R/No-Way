using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class SyncSprite : MonoBehaviourPunCallbacks
{
    [PunRPC]
    void SyncSpriteRPC(string spritePath, int moveCount)
    {
        Debug.Log("SyncSpriteRPC called with path: " + spritePath);
        Image imageComponent = GetComponent<Image>();
        imageComponent.sprite = Resources.Load<Sprite>(spritePath);
        imageComponent.color = new Color(1f, 1f, 1f, 1f);
        // If the move count is 2, remove the sprite
        if (moveCount == 2)
        {
            imageComponent.sprite = null;
            imageComponent.color = new Color(1f, 1f, 1f, 0f);
        }
    }
}