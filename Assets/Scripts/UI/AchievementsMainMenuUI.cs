using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementsMainMenuUI : MonoBehaviour
{
    private SFXManager sFXManager;

    [Header("UI References")]
    [SerializeField] private Button openAchievementsButton;
    [SerializeField] private Button closeAchievementsButton;
    [SerializeField] private GameObject achievementsHolder;
    [SerializeField] private GameObject achievementsButtonHolder;
    [SerializeField] private AudioClip[] achievementsOpenCloseSFX;
    [SerializeField] private float uIOpenCloseLerpTime = 2;

    [Header("Level Achievements")]
    [SerializeField] private GameObject level1CompleteAchievement;
    [SerializeField] private GameObject level2CompleteAchievement;
    [SerializeField] private GameObject level3CompleteAchievement;
    [SerializeField] private GameObject level4CompleteAchievement;
    [SerializeField] private GameObject level5CompleteAchievement;
    [SerializeField] private GameObject level6CompleteAchievement;
    [SerializeField] private GameObject level7CompleteAchievement;
    [SerializeField] private GameObject level8CompleteAchievement;
    [SerializeField] private GameObject level9CompleteAchievement;
    [SerializeField] private GameObject level10CompleteAchievement;
    
    [Header("Level Achievement Blockers")]
    [SerializeField] private GameObject level1AchievementBlocker;
    [SerializeField] private GameObject level2AchievementBlocker;
    [SerializeField] private GameObject level3AchievementBlocker;
    [SerializeField] private GameObject level4AchievementBlocker;
    [SerializeField] private GameObject level5AchievementBlocker;
    [SerializeField] private GameObject level6AchievementBlocker;
    [SerializeField] private GameObject level7AchievementBlocker;
    [SerializeField] private GameObject level8AchievementBlocker;
    [SerializeField] private GameObject level9AchievementBlocker;
    [SerializeField] private GameObject level10AchievementBlocker;


    /*
    [Header("Achievements")]
    [SerializeField] private GameObject VisitComicsAchievement;
    [SerializeField] private GameObject VisitShipUpgradesAchievement;
    [SerializeField] private GameObject VisitMissileSellerAchievement;
    [SerializeField] private GameObject VisitLauncherUpgradesAchievement;
    */

    private void OnEnable()
    {
        DataPersister.InitializationComplete += OnInitializationComplete;
        openAchievementsButton.onClick.AddListener(OpenStore);
        closeAchievementsButton.onClick.AddListener(CloseStore);
    }

    private void OnDisable()
    {
        DataPersister.InitializationComplete -= OnInitializationComplete;
        openAchievementsButton.onClick.RemoveListener(OpenStore);
        closeAchievementsButton.onClick.RemoveListener(CloseStore);
    }

    private void OnInitializationComplete()
    {
        achievementsHolder.SetActive(false);
        sFXManager = SFXManager.Instance;

    }
    private void OpenStore()
    {
        achievementsHolder.SetActive(true);
        UpdateAchievements();
        PlayOpenCloseSFX();
        StartCoroutine(OpenStoreLerp());
    }

    private void CloseStore()
    {
        achievementsHolder.SetActive(false);
        PlayOpenCloseSFX();
        StartCoroutine(CloseStoreLerp());
    }

    IEnumerator OpenStoreLerp()
    {
        // without delay, move storeButtonHolder up 2000 on the y axis.
        RectTransform rectTransform = achievementsButtonHolder.GetComponent<RectTransform>();
        Vector3 originalPosition = rectTransform.localPosition;
        Vector3 startPosition = originalPosition + new Vector3(0, 2000, 0);
        rectTransform.localPosition = startPosition;

        // lerp storeButtonHolder's position from its +2000 y axis position to its original position over UIOpenCloseLerpTime seconds.
        float elapsedTime = 0f;
        while (elapsedTime < uIOpenCloseLerpTime)
        {
            rectTransform.localPosition = Vector3.Lerp(startPosition, originalPosition, elapsedTime / uIOpenCloseLerpTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rectTransform.localPosition = originalPosition;
    }
    IEnumerator CloseStoreLerp()
    {
        // lerp storeButtonHolder's position from its original position to +2000 y over UIOpenCloseLerpTime seconds.
        RectTransform rectTransform = achievementsButtonHolder.GetComponent<RectTransform>();
        Vector3 originalPosition = rectTransform.localPosition;
        Vector3 endPosition = originalPosition + new Vector3(0, 2000, 0);

        float elapsedTime = 0f;
        while (elapsedTime < uIOpenCloseLerpTime)
        {
            rectTransform.localPosition = Vector3.Lerp(originalPosition, endPosition, elapsedTime / uIOpenCloseLerpTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rectTransform.localPosition = endPosition;

        // without delay, move comicHolder down 2000 on the y axis back to its original position.
        rectTransform.localPosition = originalPosition;
    }

    private void PlayOpenCloseSFX()
    {
        AudioClip sFX = achievementsOpenCloseSFX[Random.Range(0, achievementsOpenCloseSFX.Length)];

        if (sFXManager != null)
        {
            sFXManager.PlaySFX(sFX);
        }
    }

    private void UpdateAchievements()
    {
        var gameData = DataPersister.Instance.CurrentGameData;

        level1CompleteAchievement.SetActive(gameData.GetMissionComplete(1));
        level2CompleteAchievement.SetActive(gameData.GetMissionComplete(2));
        level3CompleteAchievement.SetActive(gameData.GetMissionComplete(3));
        level4CompleteAchievement.SetActive(gameData.GetMissionComplete(4));
        level5CompleteAchievement.SetActive(gameData.GetMissionComplete(5));
        level6CompleteAchievement.SetActive(gameData.GetMissionComplete(6));
        level7CompleteAchievement.SetActive(gameData.GetMissionComplete(7));
        level8CompleteAchievement.SetActive(gameData.GetMissionComplete(8));
        level9CompleteAchievement.SetActive(gameData.GetMissionComplete(9));
        level10CompleteAchievement.SetActive(gameData.GetMissionComplete(10));

        level1AchievementBlocker.SetActive(!gameData.GetMissionComplete(1));
        level2AchievementBlocker.SetActive(!gameData.GetMissionComplete(2));
        level3AchievementBlocker.SetActive(!gameData.GetMissionComplete(3));
        level4AchievementBlocker.SetActive(!gameData.GetMissionComplete(4));
        level5AchievementBlocker.SetActive(!gameData.GetMissionComplete(5));
        level6AchievementBlocker.SetActive(!gameData.GetMissionComplete(6));
        level7AchievementBlocker.SetActive(!gameData.GetMissionComplete(7));
        level8AchievementBlocker.SetActive(!gameData.GetMissionComplete(8));
        level9AchievementBlocker.SetActive(!gameData.GetMissionComplete(9));
        level10AchievementBlocker.SetActive(!gameData.GetMissionComplete(10));


        /*
        VisitComicsAchievement.SetActive(gameData.GetVisitComics());
        VisitShipUpgradesAchievement.SetActive(gameData.GetVisitShipUpgrades());
        VisitMissileSellerAchievement.SetActive(gameData.GetVisitMissileSeller());
        VisitLauncherUpgradesAchievement.SetActive(gameData.GetVisitLauncherUpgrades());
        */

    }

}


