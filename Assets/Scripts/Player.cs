using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CnControls;
using DG.Tweening;
using TMPro;
using UnityEngine.AI;

public class Player : MonoBehaviour
{


    public bool active;
    public bool isAi;
    public Transform[] bouns;
    public bool bounsStraight;

    public float speed;
    public float turnSpeed;
    public bool isInZone;
    public int score;
    public Transform scoreFx;
    public Transform zone;
    public Transform target;
    public float outOfZoneRange;
    private Animator anim;
    public ParticleSystem trailFx;


    private Rigidbody rb;
    private gamemanager thegameManager;
    private NavMeshAgent nav;
    private Zone zoneScript;
    private float defualtSpeed;


    public bool canPunch = true;
    public float PunchForce;
    public GameObject PunchFx;
    public bool stunned;
    public Transform Puncher;
    private CameraFollow CameraFollowScipts;


    public bool Bomber;
    public bool unstable;
    public Transform explostionFx;

    public Transform ScoreCard;
    public TextMeshPro PlayerName;
    public GameObject crown;



    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        InvokeRepeating("AddScore", 1, 1);
        thegameManager = FindObjectOfType<gamemanager>();
        anim = GetComponent<Animator>();
        CameraFollowScipts = FindObjectOfType<CameraFollow>();
        if (isAi)
        {
            nav = GetComponent<NavMeshAgent>();
            zoneScript = zone.GetComponent<Zone>();
        }
        else
        {
            defualtSpeed = speed;
        }
    }



    private void OnEnable()
    {
        canPunch = true;
        stunned = false;

        ParticleSystem.EmissionModule em = trailFx.emission;
        em.enabled = true;
        rb.velocity = Vector3.zero;

        anim.SetBool("Run", true);

        if (Bomber)
            unstable = false;

        if (isAi)
        {
            nav.enabled = true;
        }
        else
        {
            speed = defualtSpeed;
            CameraFollowScipts.enabled = true;
        }

        Vector3 direction = new Vector3(transform.position.x, 0f, transform.position.z) - new Vector3(zone.transform.position.x, 0f, zone.transform.position.z);
        transform.rotation = Quaternion.LookRotation(-direction);
    }


    private void Update()
    {
        if (thegameManager.gameStarted && !active)
        {
            active = true;
            anim.SetBool("Run", true);

            ParticleSystem.EmissionModule em = trailFx.emission;
            em.enabled = true;


            if (isAi)
            {
                nav.enabled = true;
            }
        }


        if (active)
        {

            if (isAi)
            {
                if (!stunned)
                {
                    if (thegameManager.PlayerInZone.Contains(transform))
                    {
                        target = getColsetEnemyInZone(thegameManager.PlayerInZone);

                        if (target == null)
                        {
                            target = zoneScript.wayPoint[Random.Range(0, zoneScript.wayPoint.Length)];
                        }

                    }
                    else
                    {
                        target = getColsetEnemy(thegameManager.Players);

                        if (Vector3.Distance(transform.position, target.position) > outOfZoneRange)
                        {
                            target = zone;
                        }
                    }

                    nav.SetDestination(target.position);


                }
            }
            else
            {

                if (Input.GetMouseButton(0))
                {
                    Vector3 touchMagnutied = new Vector3(CnInputManager.GetAxis("Horizontal"), CnInputManager.GetAxis("Vertical"), 0f);
                    Vector3 touchPos = transform.position + touchMagnutied.normalized;
                    Vector3 touchDir = touchPos - transform.position;
                    float Angle = Mathf.Atan2(touchDir.y, touchDir.x) * Mathf.Rad2Deg;
                    Quaternion quo = Quaternion.AngleAxis(Angle - 90, Vector3.down);

                    transform.rotation = Quaternion.Lerp(transform.rotation, quo, turnSpeed * Mathf.Min(Time.deltaTime, 0.04f));
                }
            }

            if(transform.position.y < -1f && !stunned)
            {
                ParticleSystem.EmissionModule em = trailFx.emission;
                em.enabled = false;
                stunned = true;
                speed = 0;
                StartCoroutine(Death());
            }



        }

    }


    private void FixedUpdate()
    {
        if (active && !isAi)
            rb.MovePosition(transform.position + transform.forward * speed * Time.fixedDeltaTime);
    }

    private void LateUpdate()
    {
        if (bounsStraight)
        {
            foreach (Transform n in bouns)
            {
                n.eulerAngles = new Vector3(0, n.eulerAngles.y, n.eulerAngles.z);
            }
        }
    }

    private void AddScore()
    {
        if (isInZone && !thegameManager.gameover)
        {
            if (isAi)
            {
                score++;

            }
            else
            {
                score++;
                Transform t = Instantiate(scoreFx, new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), Quaternion.identity);
                TextMeshPro txt = t.GetComponent<TextMeshPro>();
                txt.DOFade(0, 0.5f).SetDelay(0.5f);
                t.DOMoveY(t.position.y + 2, 1);
                Destroy(t.gameObject, 2);
            }


        }
    }

    public void addknockOutScore()
    {
        if (isAi && !thegameManager.gameover)
        {
            score += 10;
        }
        else if(!thegameManager.gameover)
        {
            score += 10;
            Transform t = Instantiate(scoreFx, new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), Quaternion.identity);
            TextMeshPro txt = t.GetComponent<TextMeshPro>();
            txt.text = "+10";
            txt.color = Color.yellow;
            txt.DOFade(0, 0.5f).SetDelay(0.5f);
            t.DOMoveY(t.position.y + 2, 1);
            t.DOPunchScale(new Vector3(.5f, .5f, .5f), .8f);
            Destroy(t.gameObject, 2);
        }
    }


    private Transform getColsetEnemyInZone(List<Transform> Enemies)
    {
        Transform BestTarget = null;
        float smallestDistance = Mathf.Infinity;
        Vector3 CurrentPosition = transform.position;

        foreach (Transform PotionalTarget in Enemies)
        {
            if (PotionalTarget != transform)
            {
                Vector3 directionToTarget = PotionalTarget.position - CurrentPosition;
                float distance = directionToTarget.sqrMagnitude;

                if (distance < smallestDistance)
                {
                    smallestDistance = distance;
                    BestTarget = PotionalTarget;
                }
            }
        }

        return BestTarget;
    }

    private Transform getColsetEnemy(Player[] Enemies)
    {
        Transform BestTarget = null;
        float smallestDistance = Mathf.Infinity;
        Vector3 CurrentPosition = transform.position;

        foreach (Player PotionalTarget in Enemies)
        {
            if (PotionalTarget.transform != transform)
            {
                Vector3 directionToTarget = PotionalTarget.transform.position - CurrentPosition;
                float distance = directionToTarget.sqrMagnitude;

                if (distance < smallestDistance)
                {
                    smallestDistance = distance;
                    BestTarget = PotionalTarget.transform;
                }
            }
        }

        return BestTarget;
    }

    public void Punch(Transform other)
    {
        if (canPunch)
        {

            if (Bomber)
            {
                if (!unstable)
                {
                    unstable = true;

                    Material mat = transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material;

                    Sequence unstableAnimation = DOTween.Sequence();
                    unstableAnimation.Append(mat.DOColor(Color.red, 0.25f));
                    unstableAnimation.Join(transform.GetChild(0).DOScale(1.5f, 0.25f));
                    unstableAnimation.Append(mat.DOColor(Color.white, 0.25f));
                    unstableAnimation.Join(transform.GetChild(0).DOScale(1, 0.25f));
                    unstableAnimation.SetLoops(5);
                    unstableAnimation.OnComplete(() =>
                    {
                        Transform t = Instantiate(explostionFx, new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), Quaternion.identity);
                        Destroy(t.gameObject, 3);

                        StartCoroutine(Death());

                        Collider[] coll = Physics.OverlapSphere(transform.position, 5f);

                        foreach(Collider c in coll)
                        {
                            if (c.CompareTag("Player"))
                            {
                                Rigidbody _rb = c.GetComponent<Rigidbody>();
                                _rb.velocity = Vector3.zero;
                                rb.velocity = (c.transform.position - transform.position).normalized * PunchForce;

                                Player tempPlayerScript = c.GetComponent<Player>();
                                tempPlayerScript.StartCoroutine(tempPlayerScript.stun());
                            }
                        }

                    });

                    

                }
            }
            else
            {
                canPunch = false;
                anim.SetBool("Attack", true);
                StartCoroutine(ResetPunch());

                Player tempPlayerScript = other.GetComponent<Player>();
                tempPlayerScript.Puncher = transform;
                tempPlayerScript.StartCoroutine(tempPlayerScript.stun());


                Rigidbody _rb = other.GetComponent<Rigidbody>();
                _rb.velocity = transform.forward * PunchForce;
            }
        }
    }


    private IEnumerator ResetPunch()
    {
        yield return new WaitForSeconds(0.1f);
        bounsStraight = false;
        PunchFx.SetActive(true);

        yield return new WaitForSeconds(0.25f);
        bounsStraight = true;
        PunchFx.SetActive(false);
        canPunch = true;
        anim.SetBool("Attack", false);
    }

    public IEnumerator stun()
    {
        stunned = true;
        ParticleSystem.EmissionModule em = trailFx.emission;
        em.enabled = false;

        if (isAi)
        {
            canPunch = false;
            nav.enabled = false;
        }
        else
        {
            speed = 0;
        }

        yield return new WaitForSeconds(0.25f);

        if (Vector3.Distance(Vector3.zero, new Vector3(transform.position.x, 0f, transform.position.z)) < 12.7f)
        {
            rb.velocity = Vector3.zero;
            stunned = false;
            em.enabled = true;

            if (isAi)
            {
                canPunch = true;
                nav.enabled = true;
            }
            else
            {
                speed = defualtSpeed;
            }

        }
        else
        {
            if (Puncher)
            {
                Puncher.GetComponent<Player>().addknockOutScore();
                Puncher = null;
            }

            StartCoroutine(Death());
        }
    }

    private IEnumerator Death()
    {
        if (!isAi)
            CameraFollowScipts.enabled = false;

        yield return new WaitForSeconds(0.5f);

        gameObject.SetActive(false);
        transform.position = new Vector3(0, -100, 0f);
        thegameManager.StartCoroutine(thegameManager.Respawn(transform, 3f));
    }


    public void Stop(bool won)
    {
        active = false;

        ParticleSystem.EmissionModule em = trailFx.emission;
        em.enabled = false;

        anim.SetBool("Run", false);

        if (isAi)
            nav.enabled = false;
        else
            speed = 0;

        if (won)
        {
            anim.SetBool("Won", true);
        }
        else
        {
            anim.SetBool("Lost", true);
        }

    }


}
