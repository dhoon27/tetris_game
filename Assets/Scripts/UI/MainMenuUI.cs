using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public Button playButton;

    private void Start()
    {
        playButton.onClick.AddListener(OnPlayButton);
    }

    private void OnPlayButton()
    {
        SceneManager.LoadScene("Game");
    }
}
