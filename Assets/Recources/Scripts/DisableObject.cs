using UnityEngine;

public class DisableObject : MonoBehaviour
{
    // Start is called before the first frame update
    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
