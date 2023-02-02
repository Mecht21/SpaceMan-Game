using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

//Enumeramos todos los estados del juego
public enum GameState {
    menu,
    inGame,
    gameOver
}

public class GameManager : MonoBehaviour
{
    //Indicamos que inicie en menú
    public GameState currentGameState = GameState.menu;

    //Variables para singleton
    public static GameManager sharedInstance;
    private PlayerController controller;

    public int collectedObject = 0;

    void Awake()
    {
        if (sharedInstance == null)
        {
            sharedInstance = this;
        }        
    }

    // Start is called before the first frame update
    void Start()
    {
        //Buscamos la tag Player para obtener el playerController
        controller = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        //La partida inica cuando se presione el boton enter y no este dentro de la partida 
        if (Input.GetButtonDown("Submit") && currentGameState != GameState.inGame)
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        //Le indicamos que el estado de juego es inGame
        SetGameState(GameState.inGame);
    }

    public void GameOver()
    {
        //Le indicamos que el estado de juego es gameOver
        SetGameState(GameState.gameOver);
    }

    public void BackToMenu()
    {
        //Le indicamos que el estado de juego es en el menu
        SetGameState(GameState.menu);
    }

    private void SetGameState(GameState newGameState)
    {
        //Vamos a establecer el qué hacer según el estado del juego
        if (newGameState == GameState.menu)
        {
            //TODO: Colocar la lógica del menú
            MenuManager.sharedInstance.ShowMainMenu();
            MenuManager.sharedInstance.HideGameMenu();
            MenuManager.sharedInstance.HideGameOverMenu();
        } else if(newGameState == GameState.inGame)
        {
            //TODO: Hay que preparar la escena para jugar
            LevelManager.sharedInstance.RemoveAllLevelBlocks();
            LevelManager.sharedInstance.GenerateInitialBlocks();
            controller.StartGame();
            MenuManager.sharedInstance.HideMainMenu();
            MenuManager.sharedInstance.ShowGameMenu();
            MenuManager.sharedInstance.HideGameOverMenu();

        } else if(newGameState == GameState.gameOver)
        {
            //TODO: preparar el juego para el GameOver
            MenuManager.sharedInstance.ShowMainMenu();
            MenuManager.sharedInstance.HideMainMenu();
            MenuManager.sharedInstance.HideGameMenu();
            MenuManager.sharedInstance.ShowGameOverMenu();
        }

        this.currentGameState = newGameState;
    }

    public void CollectObject (Collectable collectable)
    {
        collectedObject += collectable.value;
    }
}
