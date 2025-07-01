using UnityEngine;
using System.Collections;
using TMPro;

public class FishMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float turnSpeed = 3f;
    public float changeDirectionInterval = 3f;
    public float mate = 100f;

    private Vector3 targetDirection;
    private float timer;
    private bool isTouchingSurface = false;
    private bool foundFood = false;

    private float matingCooldown = 0f; // segundos restantes hasta poder volver a aparearse
    private const float MATING_COOLDOWN_DURATION = 20f; // duración fija

    private bool starving;
    public float foodCount = 100.0f;
    public bool gender = true; //True male or false female
    private bool mating = false;

    private bool infertil = false;

    public GameObject fishes;


    void Start()
    {
        starving = false;
        //mate = 100f;
        ChooseNewDirection();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (!foundFood && timer >= changeDirectionInterval)
        {
            Debug.Log("Buscanding");
            ChooseNewDirection();
            timer = 0f;
        }



    
        if (isTouchingSurface)
        {

            targetDirection.y = -1f;
            targetDirection.x = 0f;
            targetDirection.z = 0f;

        }

        if (foundFood || mating)
        {


            Debug.Log("Funca");
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = targetRotation; // ¡Rotación inmediata!
        }
        else
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            
        }

        // Dibuja una flecha roja desde la posición del pez hacia adelante
        Debug.DrawRay(transform.position, transform.forward * 2f, Color.red);
        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        // Suavemente rotar hacia target

        foodCount -= 1.2f * Time.deltaTime;
        mate -= 5f * Time.deltaTime;
        // evita que sea menor que 0


        if (foodCount < 32f)
        {
            starving = true;
        }
        else
        {
            starving = false;
        }

        //Debug.Log(mate);
        

        if(foodCount < -20f)
        {
            Destroy(gameObject);
        }

        if (matingCooldown > 0f)
        {
            matingCooldown -= Time.deltaTime;
        }



        // Mover hacia adelante

    }

    void reproduce()
    {
        for(int i = 0; i < 2; i++)
        {
            GameObject newFish =  Instantiate(fishes, transform.position, Quaternion.identity);
            newFish.GetComponent<FishMovement>().gender = Random.value > 0.5f;
            newFish.GetComponent<FishMovement>().infertil = Random.value > 0.3f;
            newFish.GetComponent<FishMovement>().mate = 100f;
            newFish.GetComponent<FishMovement>().mating = false;

        }
    }

    void ChooseNewDirection()
    {
        targetDirection = Random.onUnitSphere;

        // Si está tocando la superficie, forzar la dirección hacia abajo
        if (isTouchingSurface)
        {
            Debug.Log("Aqui");
            targetDirection.y *= -1f; // Invertir componente Y para que no suba
            targetDirection.x *= 0f;
            targetDirection.z *= 0f;
        }


        targetDirection.Normalize(); // Asegurar magnitud 1
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("WaterSurface"))
        { 
            Debug.Log("Entre");
            isTouchingSurface = true;
        }

        if (other.CompareTag("Food") && starving && !mating)
        {
            Debug.Log("EntreComidaaa!!!");
            // Calcular la dirección hacia la comida
            foundFood = true;

            Vector3 directionToFood = other.transform.position - transform.position;
            directionToFood.Normalize();

            // Cancelar dirección anterior y usar la nueva
            targetDirection = Vector3.zero; // "Reset"
            targetDirection = directionToFood.normalized;

        }

        if(other.CompareTag("Fish") && mate <= 20 && !infertil)
        {
            Debug.Log("Mating");
            if (other.gameObject.GetComponent<FishMovement>().gender == !gender && other.gameObject.GetComponent<FishMovement>().infertil == false)
            {
                if(matingCooldown <= 0f && other.gameObject.GetComponent<FishMovement>().matingCooldown <= 0f)
                {
                    Vector3 directionMate = other.transform.position - transform.position;
                    targetDirection = directionMate.normalized;
                    mating = true;

                }

            }

        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (foundFood || mating)
        {
            Vector3 directionToFood = other.transform.position - transform.position;
            directionToFood.Normalize();

            // Cancelar dirección anterior y usar la nueva
            targetDirection = Vector3.zero; // "Reset"
            targetDirection = directionToFood.normalized;
        }

 

        
    }


    private void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.CompareTag("Food"))
        {
            Debug.Log("Me lo comi");
            foundFood = false;
            Destroy(other.gameObject);
            foodCount += 20f;
            foodCount = Mathf.Min(foodCount, 100f);

        }

        if (other.gameObject.CompareTag("Fish"))
        {
            Debug.Log("Sila");
            mating = false;
            mate = 100f;
            other.gameObject.GetComponent<FishMovement>().mate = 100f;
            other.gameObject.GetComponent<FishMovement>().mating = false;

            if (!gender)
            {
                StartCoroutine(DelayedReproduction());
                //reproduzirse
            }
            

        }


    }

    // Corrutina que espera 5 segundos antes de reproducirse
    private IEnumerator DelayedReproduction()
    {
        yield return new WaitForSeconds(5f);
        reproduce();
        mating = false;
        mate = 100f;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("WaterSurface"))
        {
            Debug.Log("Sali");
            isTouchingSurface = false;
        }
    }
}
