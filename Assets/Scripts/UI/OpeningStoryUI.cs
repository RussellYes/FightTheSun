using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class OpeningStoryUI : MonoBehaviour
{
    [SerializeField] private GameObject openingStoryHolder;
    [SerializeField] private TextMeshProUGUI storyText1;
    [SerializeField] private TextMeshProUGUI storyText2;
    private void Start()
    {
        openingStoryHolder.SetActive(true);
        storyText1.enabled = false;
        storyText2.enabled = false;

        StartCoroutine(PlayOpeningStory());
    }

    IEnumerator PlayOpeningStory()
    {
        openingStoryHolder.SetActive(true);
        yield return new WaitForSeconds(1f);

        storyText1.enabled = true;
        yield return new WaitForSeconds(1f);

        storyText2.enabled = true;
        yield return new WaitForSeconds(1f);

        openingStoryHolder.SetActive(false);
    }

}
