using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOutcome : MonoBehaviour
{
    public static GameOutcome Instance { get; private set; }

    [SerializeField] GameObject teleportsRemainingScreens;
    [SerializeField] GameObject tilesRemainingCornerScreens;

    [Space(10)]
    [SerializeField] GameObject forfeitScreens;
    [SerializeField] GameObject gameOverResultsScreens;

    Coroutine gameOver;
            
    int minimumTilesRemainingToWin;
    bool gameHasEnded;


    void Start() {
        Instance = this;
        minimumTilesRemainingToWin = GameManager.Instance.minimumTilesRemainingToWin;        
    }


    void Update() =>
        G_KeyOpensForfeitMenu();


    void G_KeyOpensForfeitMenu()
    {
        if (!gameHasEnded) {
            if (Input.GetKeyDown(KeyCode.G))
            {
                if (!forfeitScreens.activeInHierarchy) {
                    Time.timeScale = 0;
                    forfeitScreens.SetActive(true);
                }
                else {
                    Time.timeScale = 1;
                    forfeitScreens.SetActive(false);
                }
            }
        }
    }

    // called by buttons
    public void PlayerCanceledEndingGame() {
        Time.timeScale = 1;
        forfeitScreens.SetActive(false);
    }


    // called by buttons
    public void PlayerEndedGame()
    {
        Time.timeScale = 1; // keep for now

        if (GameManager.Instance.minimumTilesRemainingToWin >= GameManager.Instance.remainingTiles)
            PrepForGameOver("You won!");
        else
            PrepForGameOver("You lost :(");
    }


    void PrepForGameOver(string gameOutcome)
    {
        teleportsRemainingScreens.SetActive(false);
        tilesRemainingCornerScreens.SetActive(false);
        forfeitScreens.SetActive(false);

        if (!gameHasEnded)
            gameOver = StartCoroutine(GameOver(gameOutcome));
    }

    // Sequentially turn on ending screens.
    // Stupid but it works. Refactor later.
    IEnumerator GameOver (string outcome)
    {
        gameHasEnded = true;
        // gameOverResultsScreens.SetActive(true);

        for (int i = 0; i < gameOverResultsScreens.transform.childCount; i++)
        {
            yield return new WaitForSeconds(1f);
            var child = gameOverResultsScreens.transform.GetChild(i).gameObject;
            child.SetActive(true);

            // Screen for Tiles Remained number
            if (i == 3)
                child.GetComponent<TMPro.TextMeshProUGUI>().text = GameManager.Instance.remainingTiles.ToString();
            // Screen for "game won" / "game lost"
            if (i == 4)
                child.GetComponent<TMPro.TextMeshProUGUI>().text = outcome;            
        }

        GameManager.Instance.canReloadScene = true;

        yield return null;
    }
}
