using UnityEngine;
using UnityEngine.UI;

public class HyperlinkMainMenu : MonoBehaviour
{
    [SerializeField] private string webLink;
    [SerializeField] private Button openWebLinkButton;

    private void OnEnable()
    {
        openWebLinkButton.onClick.AddListener(() => OpenURL(webLink));
        Debug.Log("HyperlinkMainMenu OnEnable");
    }

    private void OnDisable()
    {
        openWebLinkButton.onClick.RemoveAllListeners();
    }

    public void OpenYouTube()
    {
        Application.OpenURL("https://www.youtube.com/@ibelonghere");
        Debug.Log("HyperlinkMainMenu OpenYouTube - https://www.youtube.com/@ibelonghere");
    }

    public void OpenURL(string link)
    {
        Application.OpenURL(link);
        Debug.Log("HyperLinkMainMenu OpenURL - " + link);
    }
}
