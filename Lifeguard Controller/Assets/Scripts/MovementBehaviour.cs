using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class MovementBehaviour : MonoBehaviour {
    public enum Health { Good, Bad, Dead, Null }
    public Health health;

    public Vector2 distanceSwim;
    Global global;
    Level level;
    NavMeshAgent agent;
    public enum State { Idle, Walk, GoOut, Drown }
    public State state;
    public float distance;
    public int timer;

    [Space(10)]

    // De 0 to 1.
    public float skill;
    public float stamina;
    public float maxStamina;
    public float staminaSwimLoss;
    public float staminaWalkGain;
    public float staminaIdleGain;
    public float staminaDrawnValue;
    public float staminaSwimValue;

    bool inWater;
    public GameObject followButton;
    public GameObject stopFollowButton;
    public GameObject helpIcon;
    public GameObject medicalAssistanceIcon;
    public GameObject deadIcon;
    public GameObject drownParticles;
    public Slider drownBar;
    public float timeToDrown;
    public float timeDrowning;
    public bool getInInfirmary;

    private void Start() {
        agent = GetComponent<NavMeshAgent>();
        skill = Random.Range(0.0f, 0.5f);
        maxStamina = Random.Range(100, 400);
        stamina = Random.Range(0.0f, maxStamina);
        staminaSwimValue = maxStamina / 2;
        global = GameObject.Find("Level").GetComponent<Global>();
        level = GameObject.Find("Level").GetComponent<Level>();
    }

    private void Update() {
        // Se actualiza la estamina.
        switch (state) {
            case State.Idle:
                Idle();
                break;
            case State.Walk:
                Walk();
                break;
            case State.Drown:
                Drowning();
                break;
            case State.GoOut:
                GoOut();
                break;
            default:
                break;
        }
        if(level.time +15 >= level.minutesPerDay * 60 && state != State.Drown) {
            state = State.GoOut;
        }
    }

    void Idle() {
        // Se actualiza la estamina.
        if (!inWater) {
            stamina += staminaIdleGain;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
        }
        // Se actualiza el tiempo que queda de estar parado.
        timer--;

        // Si se ha acabado el tiempo de idle se le asigna un nuevo
        // destino. 
        if (timer <= 0) {
            // Si tiene suficiente estamina se le asignara una posicion en el agua.
            // Sino seguira en tierra.
            Vector3 result;
            if (stamina < staminaSwimValue) {
                result = global.GetPosition(false);
                state = State.GoOut;
            } else if (!inWater && stamina != maxStamina) {
                result = global.GetPosition(false);
                state = State.Walk;
            } else {
                result = global.GetPosition(true);
                state = State.Walk;
            }
            agent.SetDestination(result);
        }
    }
    bool reset;
    void Walk() {
        // Se actualiza la estamina.
        if (!inWater) {
            stamina += staminaWalkGain;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
        } else {
            stamina -= staminaSwimLoss * ( 1 - skill );
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
        }

        // Si no tiene estamina se ahoga.
        if (stamina <= staminaDrawnValue && inWater) {
            state = State.Drown;
            agent.SetDestination(transform.position);
            return;
        }
        if (stamina < staminaSwimValue) {
            state = State.Idle;
            agent.SetDestination(transform.position);
        }
        // Si se ha llegado a la localizacion se pone un
        // temporizador para que se quede un rato en idle
        // proporcional a la distancia recorrida.
        if (Vector3.SqrMagnitude(agent.destination - transform.position) < 2.0f) {
            state = State.Idle;
            timer = Random.Range(0, 200);
            reset = false;
        }
    }
    void GoOut() {
        // Se actualiza la estamina.
        if (!inWater) {
            stamina += staminaWalkGain;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
        } else {
            stamina -= staminaSwimLoss * ( 1 - skill );
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
        }

        // Si no tiene estamina se ahoga.
        if (stamina <= staminaDrawnValue && inWater) {
            state = State.Drown;
            agent.SetDestination(transform.position);
            return;
        }
        // Si se ha llegado a la localizacion se pone un
        // temporizador para que se quede un rato en idle
        // proporcional a la distancia recorrida.
        if (Vector3.SqrMagnitude(agent.destination - transform.position) < 2.0f) {
            state = State.Idle;
            timer = Random.Range(0, 200);
            reset = false;
        }
    }

    bool follow;
    Vector3 diference;
    Transform target;
    bool inInfirmary;
    bool deathadded;
    void Drowning() {
        bool near = follow;


        if (health != Health.Dead) {
            drownParticles.SetActive(true);
            drownBar.gameObject.SetActive(true);
            drownBar.value = 1 - Mathf.InverseLerp(0, timeToDrown, timeDrowning);
        }

        // Verificamos si ya esta siguiendo a alguien.
        if (!follow && health != Health.Dead) {
            // Lista de "Players" en un radio de 10.
            Collider[] colliders = Physics.OverlapSphere(transform.position, 3, LayerMask.GetMask("Player"));
            near = near == false ? colliders.Length != 0 : true;
            followButton.SetActive(near);

            // Hacemos un loop por el array para buscar el mas cercano.
            foreach (var item in colliders) {
                if (target == null) {
                    target = item.transform;
                    diference = target.position - transform.position;
                    continue;
                }

                // Comparamos el elemento actual con el mas cercano encontrado
                // hasta ahora.
                Vector3 nd = item.transform.position - transform.position;
                if (Vector3.SqrMagnitude(diference) > Vector3.SqrMagnitude(nd)) {
                    target = item.transform;
                    diference = nd;
                }
            }
        }

        health = timeDrowning < timeToDrown / 2 ? Health.Good : Health.Bad;
        health = timeDrowning < timeToDrown ? health : Health.Dead;
        timeDrowning += Time.deltaTime;
        switch (health) {
            case Health.Good:
                swimAudio.Stop();
                helpIcon.SetActive(near);
                // Si ya tiene un "Player" que le esta salvando
                if (follow) {
                    if (inWater) {
                        // Hacemos que le siga.
                        transform.position = target.position - diference;
                        agent.enabled = false;

                    } else {
                        follow = false;
                        target = null;
                        agent.enabled = true;
                        state = State.Idle;
                        stamina = staminaSwimValue - 5;
                        agent.SetDestination(transform.position);
                        stopFollowButton.SetActive(false);
                        helpIcon.SetActive(false);
                        drownParticles.SetActive(false);
                        drownBar.gameObject.SetActive(false);
                        timeDrowning = 0;
                        skill += 0.1f;
                    }
                }
                break;
            case Health.Bad:
                swimAudio.Stop();
                helpIcon.SetActive(false);
                medicalAssistanceIcon.SetActive(near);
                // Añadir el evento.
                // Si ya tiene un "Player" que le esta salvando
                if (follow) {
                    if (inWater) {
                        // Hacemos que le siga.
                        transform.position = target.position - diference;
                        agent.enabled = false;
                    } else {
                        transform.position = target.position - diference;
                        agent.enabled = false;
                        drownParticles.SetActive(false);
                        if (inInfirmary) {
                            GameObject.Find("Level").GetComponent<Level>().enemies.Remove(this.gameObject);
                            GameObject.Destroy(this.gameObject, 1);
                        }
                    }
                }
                break;
            case Health.Dead:
                follow = false;
                target = null;
                helpIcon.SetActive(false);
                medicalAssistanceIcon.SetActive(false);
                followButton.SetActive(false);
                stopFollowButton.SetActive(false);
                drownParticles.SetActive(false);
                drownBar.gameObject.SetActive(false);
                deadIcon.SetActive(true);

                if (!deathadded) {
                    GameObject.Find("Level").GetComponent<Level>().death++;
                    deathadded = true;
                }
                health = Health.Null;
                break;
            default:
                break;
        }
    }

    public void SelectFollowActive(bool active) {
        follow = active;
        followButton.SetActive(!followButton.activeSelf);
        stopFollowButton.SetActive(!stopFollowButton.activeSelf);
        if (active == false) {
            target = null;
        }
    }
    public AudioSource swimAudio;
    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Water") {
            inWater = true;
            swimAudio.Play();
        }
    }
    private void OnCollisionExit(Collision collision) {
        if (collision.gameObject.tag == "Water") {
            inWater = false;
            swimAudio.Stop();
        }
    }
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Infirmary")
            inInfirmary = true;
    }
    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Infirmary")
            inInfirmary = false;
    }
}
