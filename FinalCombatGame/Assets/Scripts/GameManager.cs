using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public PlayerController player1;
    public PlayerController player2;

    public static GameManager instance;
    public GameObject restartButton;
    private void Awake()
    {
        instance = this;
        restartButton.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerDefeated(PlayerController player)
    {
        restartButton.SetActive(true);
        player1.BlockInputs();
        player2.BlockInputs();
        if (player == player1)
        {
            player2.TriggerWin();
        }
        else
        {
            player1.TriggerWin();
        }
    }

    public void restartgame()
    {
        SceneManager.LoadScene("Example");

    }
}
