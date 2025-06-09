using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ComicsUI : MonoBehaviour
{
    [SerializeField] private Image lockedImage;
    [SerializeField] private Image comicTestPlanet1;
    [SerializeField] private Image comicTestPlanet2;
    [SerializeField] private Image comicTestPlanet3;
    [SerializeField] private Image comicTestPlanet4;
    [SerializeField] private Image comicThrowPillows1;
    [SerializeField] private Image comicThrowPillows2;
    [SerializeField] private Image comicThrowPillows3;
    [SerializeField] private Image comicThrowPillows4;
    [SerializeField] private Image comicTrafficButton1;
    [SerializeField] private Image comicTrafficButton2;
    [SerializeField] private Image comicTrafficButton3;
    [SerializeField] private Image comicTrafficButton4;
    [SerializeField] private Image comicImpatient1;
    [SerializeField] private Image comicImpatient2;
    [SerializeField] private Image comicImpatient3;
    [SerializeField] private Image comicImpatient4;
    [SerializeField] private Image comicNatureLover1;
    [SerializeField] private Image comicNatureLover2;
    [SerializeField] private Image comicNatureLover3;
    [SerializeField] private Image comicNatureLover4;
    [SerializeField] private Image comicDysonSphere1;
    [SerializeField] private Image comicDysonSphere2;
    [SerializeField] private Image comicDysonSphere3;
    [SerializeField] private Image comicDysonSphere4;
    [SerializeField] private Image comicGoingLive1;
    [SerializeField] private Image comicGoingLive2;
    [SerializeField] private Image comicGoingLive3;
    [SerializeField] private Image comicGoingLive4;
    [SerializeField] private Image comicClippingCoupons1;
    [SerializeField] private Image comicClippingCoupons2;
    [SerializeField] private Image comicClippingCoupons3;
    [SerializeField] private Image comicClippingCoupons4;
    [SerializeField] private Image comicExactChange1;
    [SerializeField] private Image comicExactChange2;
    [SerializeField] private Image comicExactChange3;
    [SerializeField] private Image comicExactChange4;
    [SerializeField] private Image comicHeyNeighbour1;
    [SerializeField] private Image comicHeyNeighbour2;
    [SerializeField] private Image comicHeyNeighbour3;
    [SerializeField] private Image comicHeyNeighbour4;
    [SerializeField] private Image comicRewind1;
    [SerializeField] private Image comicRewind2;
    [SerializeField] private Image comicRewind3;
    [SerializeField] private Image comicRewind4;
    [SerializeField] private Image comicInTheBlack1;
    [SerializeField] private Image comicInTheBlack2;
    [SerializeField] private Image comicInTheBlack3;
    [SerializeField] private Image comicInTheBlack4;

    private void Start()
    {
        LoadComicData(levelData comicNumber, unlocked);
    }

    private void LoadComicData(float comicNumber)
    {
        if (DataPersister.Instance == null || DataPersister.Instance.CurrentGameData == null)
        {
            Debug.LogWarning("DataPersister or CurrentGameData not initialized yet.");
            return;
        }

        GameData gameData = DataPersister.Instance.CurrentGameData;

        if (!gameData.comicData.TryGetValue(comicNumber, out ComicData comicData))
        {
            Debug.LogWarning($"No comic data found for comic number {comicNumber}");
            return;
        }

        if (gameData.comicData[comicNumber].unlocked)
        {
            lockedImage.gameObject.SetActive(false);
            ShowComic(comicNumber);
        }
        else
        {
            lockedImage.gameObject.SetActive(true);
        }


    }

}
