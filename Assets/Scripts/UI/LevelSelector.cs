using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    public LevelsOrder levelsOrder;
    public GameObject LevelSelectButtonPrefab;
    public Transform LevelField;
    public GameObject LevelPanel;

    private int maxLevel;

    // Start is called before the first frame update
    void Start()
    {
        maxLevel = Resources.Load<LevelProgress>("LevelProgress").LevelsCompleted;
        LevelsOrder levels = Resources.Load<LevelsOrder>("LevelOrder");
        Rect fieldRect = LevelField.gameObject.GetComponent<RectTransform>().rect;
        for (int i = 0; i < levels.NumLevels; ++i)
        {
            GameObject levelButton = Instantiate(LevelSelectButtonPrefab, LevelField);
            int currentLevel = i;
            levelButton.GetComponent<Button>().onClick.AddListener(delegate { LevelButtonClicked(currentLevel); });
            levelButton.GetComponentInChildren<Text>().text = "" + (i + 1);
            if(i <= maxLevel)
            {
                levelButton.transform.GetChild(1).gameObject.SetActive(false);
            }
            levelButton.transform.localPosition = new Vector3(5 + (40 * (i % ((Mathf.CeilToInt(fieldRect.width) - 10) / 40))), -5 - 40 * (Mathf.FloorToInt(i / ((Mathf.CeilToInt(fieldRect.width) - 10) / 40))), 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LevelButtonClicked(int levelIndex)
    {
        // TODO: Panel with level info
        //Debug.Log("Clicked button " + levelIndex);
        //if (levelIndex <= maxLevel)
        //{
        //    LevelPanel.SetActive(true);
        //}
        levelsOrder.LoadLevel(levelIndex);
    }
}
