
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Player Components")]
    public Image image;

    [Header("Child Text Objects")]
    public TextMeshProUGUI playerNumberText;
    public TextMeshProUGUI playerPointsText;
    // This value can change as clients leave and join
    public void OnPlayerNumberChanged(byte newPlayerNumber)
    {
        playerNumberText.text = string.Format("Player {0:00}", newPlayerNumber);
    }
    // Random color set by Player::OnStartServer
    public void OnPlayerColorChanged(Color32 newPlayerColor)
    {
        playerNumberText.color = newPlayerColor;
        image.color = new Color32(newPlayerColor.r,newPlayerColor.g,newPlayerColor.b,100);
    }
    // This updates from Player::UpdateData via InvokeRepeating on server
    public void OnPlayerPointsChanged(byte newPoints)
    {
        // Show the data in the UI
        playerPointsText.text = string.Format("Points: {0:00}", newPoints);
    }
}
