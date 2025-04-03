using UnityEngine;
using UnityEngine.SceneManagement;

public class Win : MonoBehaviour
{
    public GameObject NewGameButton;
    public GameObject VictoryText;

    private void Start()
    {
        // VictoryText.SetActive(false);
        // NewGameButton.SetActive(false);
        VictoryText.SetActive(true);
        NewGameButton.SetActive(true);
        NewGameButton.GetComponent<UnityEngine.UI.Button>().Select();
        // Invoke("ShowVictory", 3f);
    }

    private void ShowVictory()
    {
        VictoryText.SetActive(true);
        NewGameButton.SetActive(true);
        NewGameButton.GetComponent<UnityEngine.UI.Button>().Select();
    }

    public void NewGame()
    {
        SceneManager.LoadScene("StartMenu");
    }
}
