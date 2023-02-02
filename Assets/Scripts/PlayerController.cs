using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Variables del movimiento del personaje
    public float jumpForce = 6f;
    public float runningSpeed = 2f;
    public float maxVelocidad;

    private Rigidbody2D rigidBody;
    public LayerMask groundMask;
    Animator animator;
    Vector3 startPosition;

    //Variables para referenciar las variables creadas en el Animator de Unity
    const string STATE_ALIVE = "isAlive";
    const string STATE_ON_THE_GROUND = "isOnTheGround";

    private int healthPoints, manaPoints;

    public const int INITIAL_HEALTH = 100, INITIAL_MANA = 100,
        MAX_HEALTH = 200, MAX_MANA = 200,
        MIN_HEALTH = 10, MIN_MANA = 0;

    //Variables para el mana
    public const int SUPERJUMP_COST = 100;
    public const float SUPERJUMP_FORCE = 2f;

    public float jumpRaycastDistance = 1.5f;

    bool voltearPersonaje = true;

    SpriteRenderer personajeRender;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        personajeRender = GetComponent<SpriteRenderer>();   
    }

    // Start is called before the first frame update
    void Start()
    {
        //Obtenemos la posicion inicial del jugador
        startPosition = transform.position;
    }

    public void StartGame()
    {
        animator.SetBool(STATE_ALIVE, true);
        animator.SetBool(STATE_ON_THE_GROUND, true);

        healthPoints = MAX_HEALTH;
        manaPoints = MAX_MANA;

        //Vamos a utilizar el metodo Invoke para atrasar la animación y que sea vea mas natural
        Invoke("RestartPosition", 0.2f);
    }

    private void RestartPosition()
    {
        //Cuando se inice el juego estará en la posicion inicial ya obtenida anteriormente
        this.transform.position = startPosition;
        this.rigidBody.velocity = Vector2.zero;

        GameObject mainCamera = GameObject.Find("Main Camera");
        mainCamera.GetComponent<CameraFollow>().ResetCameraPosition();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Jump(false);
        }

        if (Input.GetButtonDown("SuperJump"))
        {
            Jump(true);
        }

        animator.SetBool(STATE_ON_THE_GROUND, IsTouchingTheGround());
        Debug.DrawRay(this.transform.position, Vector2.down * jumpRaycastDistance, Color.red);
    }

    void FixedUpdate()
    {
        Walk();
    }

    void Walk()
    {
        float mover = Input.GetAxis("Horizontal");

        //Validamos el movimiento del jugador solo si se encuentra en el estado "inGame"
        if (GameManager.sharedInstance.currentGameState == GameState.inGame)
        {
            if (rigidBody.velocity.x < runningSpeed && Input.GetButton("Horizontal"))
            {
                if (mover > 0 && !voltearPersonaje)

                {
                    Voltear();
                }
                else if (mover < 0 && voltearPersonaje)
                {
                    Voltear();
                }
                rigidBody.velocity = new Vector2(mover * runningSpeed, rigidBody.velocity.y);
            }
        }
        else
        {//Detenemos el personaje sino esta en la partida
            rigidBody.velocity = new Vector2(mover, rigidBody.velocity.y);
        }
    }
    void Voltear()

    {

        voltearPersonaje = !voltearPersonaje;

        personajeRender.flipX = !personajeRender.flipX;

    }

    /*
     Método para anadir una fuerza de salto, utilizamos la variable rigidBody para añadirle una fuerza,
    la cual toma por parametro la subida en un vector 2D al cual se le multiplica los Newtons del JumpForce creado arriba
    , por último le pasamos el tipo de fuerza a utilizar, dado que es un salto, la acción será impulsiva
     */
    void Jump(bool superJump)
    {
        float jumpForceFactor = jumpForce;
        Walk();

        if (superJump && manaPoints >= SUPERJUMP_COST)
        {
            manaPoints -= SUPERJUMP_COST;
            jumpForceFactor *= SUPERJUMP_FORCE;
        }

        //Validamos el salto del jugador solo si se encuentra en el estado "inGame"
        if (GameManager.sharedInstance.currentGameState == GameState.inGame)
        {
            if (IsTouchingTheGround())
            {
                GetComponent<AudioSource>().Play();
                rigidBody.AddForce(Vector2.up * jumpForceFactor, ForceMode2D.Impulse);
            }
        }
    }


    /*Nos indica si el personaje está o no tocando el suelo
     
    Physics2D el método RayCast tiene los siguientes parámetros:

    Origen : el punto de donde se origina el rayo 2d (ejemplo this.transform.position)
    Dirección: Un vector que representa la dirección
    Distancia : la distancia máxima en la que se lanza el rayo
    LayerMask: es un filtro que detectan los colliders en ciertas capas


    this.transform.position: empezar desde el centro del personaje
    Vector2.down: Hacia donde quiero el rayo, hacia abajo(down)
    Para establecer el limite del rayo: 1.5f
     */
    bool IsTouchingTheGround()
    {
        if (Physics2D.Raycast(this.transform.position, Vector2.down, jumpRaycastDistance, groundMask))
        {
            //TODO: programar lógica de contacto con el suelo
            return true;
        }else
        {
            //TODO: programar lógica de no contacto
            return false;
        }

    }

    public void Die()
    {
        float traveledDistance = GetTravelledDistance();
        float previousMaxDistance = PlayerPrefs.GetFloat("maxscore");

        if (traveledDistance > previousMaxDistance)
        {
            PlayerPrefs.SetFloat("maxscore", traveledDistance);
        }

        this.animator.SetBool(STATE_ALIVE, false);
        GameManager.sharedInstance.GameOver();
    }

    public void CollectHealth(int points)
    {
        this.healthPoints += points;

        if (this.healthPoints >= MAX_HEALTH)
        {
            this.healthPoints = MAX_HEALTH;
        }

        if (this.healthPoints <= 0)
        {
            Die();
        }
    }

    public void CollectMana(int points)
    {
        this.manaPoints += points;

        if (this.manaPoints >= MAX_MANA)
        {
            this.manaPoints = MAX_MANA;
        }
    }

    public void CollectDamagePotion(int points)
    {
        this.healthPoints -= points;

        if (this.healthPoints <= 0)
        {
            this.healthPoints = 0;
            Die();
        }
    }

    //Despues de modificarse las estadisticas del jugador las retornamos
    public int GetHealth()
    {
        return this.healthPoints;
    }

    public int GetMana()
    {
        return this.manaPoints;
    }

    public float GetTravelledDistance()
    {
        return this.transform.position.x - startPosition.x;
    }
}
