using UnityEngine;

public class EndGameScript : MonoBehaviour
{
    private GameManager gm;
    public GameObject endGameInformation;

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        gm.endGameEvent.AddListener(EndGame);
    }

    private void EndGame()
    {
        endGameInformation.SetActive(true);
        gameObject.SetActive(false);
    }
}