using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class gamemanager : MonoBehaviour
{
    public bool gameStarted;
    public List<Transform> PlayerInZone = new List<Transform>();
    public Player[] Players;
    public Transform RespawnT;

    private int RespawnTires;

    public float Timer;
    public TextMeshProUGUI Timerlabel;
    public Color[] PlayerColors;
    public nameManager namesM;

    public List<Transform> sortedList = new List<Transform>();
    private Camera cam;
    private CameraFollow CamFollow;
    public TMP_InputField nameField;
    public Image LoadingImage;
    public TextMeshProUGUI PlayersCountLabel;
    public GameObject MenuPanel;
    public GameObject matchMakingPanel;
    public GameObject gamePanel;
    public GameObject GameOverPanel;
    public Transform[] bots;
    public Transform zone;
    public Vector3 InGameCamPos;
    public Vector3 MatchMakingPos;
    public Transform[] LeaderbordsCard;
    public bool gameover;
    public TextMeshProUGUI timerSpawn;


    private string username;

    private void Awake()
    {
        cam = Camera.main;

        CamFollow = cam.GetComponent<CameraFollow>();
        CamFollow.enabled = false;
    }

    private void Start()
    {
        namesM = FindObjectOfType<nameManager>();

        if (PlayerPrefs.HasKey("username"))
            username = PlayerPrefs.GetString("username");
        else
            username = "Player";


        nameField.text = username;

        SetupPlayer();

        LoadingImage.transform.DORotate(new Vector3(0, 0, -1), 0.005f).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
    }


    private void Update()
    {
        foreach(Player p in Players)
        {
            if (PlayerInZone.Contains(p.transform))
            {
                if (!p.isInZone)
                    p.isInZone = true;
            }
            else
            {
                if (p.isInZone)
                    p.isInZone = false;
            }
        }


        if(Timer > 0 && gameStarted)
        {
            if (Timer < 10 && Timerlabel.color != Color.red)
                Timerlabel.color = Color.red;

            Timer -= Time.deltaTime;
            Timerlabel.text = (int)Timer / 60 + ":" + ((int)Timer % 60).ToString("00");

            UpdateScores();

        }
        else if(Timer < 0)
        {
            StartCoroutine(gameOver());
        }


    }

    public IEnumerator Respawn(Transform t, float delay)
    {

        if (!t.GetComponent<Player>().isAi)
        {
            timerSpawn.gameObject.SetActive(true);

            timerSpawn.text = "3";
            yield return new WaitForSeconds(1f);
            timerSpawn.text = "2";
            yield return new WaitForSeconds(1f);
            timerSpawn.text = "1";
            yield return new WaitForSeconds(1f);
            timerSpawn.gameObject.SetActive(false);
            if (gameStarted)
                Spawn(t);
        }
        else
        {
            yield return new WaitForSeconds(delay);
            if (gameStarted)
                Spawn(t);
        }


    }

    private void Spawn(Transform t)
    {
        RespawnT.eulerAngles = new Vector3(0, Random.Range(0, 359), 0f);
        Collider[] cols = Physics.OverlapSphere(RespawnT.GetChild(0).position, 5f);

        bool Playernear = false;

        foreach(Collider c in cols)
        {
            if (c.CompareTag("Player"))
                Playernear = true;
        }

        if (!Playernear)
        {
            t.position = RespawnT.GetChild(0).position;
            t.gameObject.SetActive(true);
            RespawnTires = 0;
        }
        else
        {
            if(RespawnTires < 10)
            {
                RespawnTires++;
                Spawn(t);
            }
            else
            {
                t.position = RespawnT.GetChild(0).position;
                t.gameObject.SetActive(true);
                RespawnTires = 0;
            }
        }

    }

    private IEnumerator gameOver()
    {
        gameStarted = false;
        GameOverPanel.SetActive(true);
        gamePanel.SetActive(false);
        gameover = true;

        for (int i = 0; i < sortedList.Count; i++)
        {
            if (i == 0)
            {
                CamFollow.focusOnWinner(sortedList[i]);
                sortedList[i].GetComponent<Player>().Stop(true);
            }
            else
                sortedList[i].GetComponent<Player>().Stop(false);
        }

        for(int i = 0; i < Players.Length; i++)
        {
            Player tempPlayerScript = sortedList[i].GetComponent<Player>();
            LeaderbordsCard[i].GetChild(0).GetComponent<TextMeshProUGUI>().text = tempPlayerScript.ScoreCard.GetSiblingIndex() + 1 + ".";
            LeaderbordsCard[i].GetChild(1).GetComponent<TextMeshProUGUI>().text = tempPlayerScript.PlayerName.text;
            LeaderbordsCard[i].GetChild(2).GetComponent<TextMeshProUGUI>().text = $"{tempPlayerScript.score}";
            LeaderbordsCard[i].GetComponent<Image>().color = tempPlayerScript.ScoreCard.GetComponent<Image>().color;


        }
        yield return null;
    }


    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    private void UpdateScores()
    {
        sortedList.Clear();

        Dictionary<Transform, int> unsortedDic = new Dictionary<Transform, int>();

        foreach(Player p in Players)
        {
            unsortedDic.Add(p.transform, p.score);
        }

        foreach(var item in unsortedDic.OrderByDescending(i => i.Value))
        {
            sortedList.Add(item.Key);
        }

        for(int i = 0; i < Players.Length; i++)
        {

            if (i == 0)
                Players[i].crown.SetActive(true);
            else
                Players[i].crown.SetActive(false);

            Player tempPlayerSctipt = sortedList[i].GetComponent<Player>();

            tempPlayerSctipt.ScoreCard.SetSiblingIndex(i);
            tempPlayerSctipt.ScoreCard.GetChild(0).GetComponent<TextMeshProUGUI>().text = (i + 1) + ".";
            tempPlayerSctipt.ScoreCard.GetChild(2).GetComponent<TextMeshProUGUI>().text = tempPlayerSctipt.score.ToString();

        }


    }

    private void SetupPlayer()
    {
        int[] index = { 0, 1, 2, 3, 4 };
        System.Random rnd = new System.Random();
        int[] RandomIndex = index.OrderBy(x => rnd.Next()).ToArray();


        for(int i = 0; i < Players.Length; i++)
        {
            string name;
            if (i == 0)
                name = username;
            else
                name = namesM.names[Random.Range(0, namesM.names.Length)];

            Players[i].ScoreCard.GetComponent<Image>().color = PlayerColors[i];
            Players[i].ScoreCard.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (i + 1) + ".";
            Players[i].ScoreCard.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = name;
            Players[i].ScoreCard.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "0";

            Material Playermat = Players[i].transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material;
            Playermat.color = PlayerColors[i];

            Players[i].PlayerName.text = name;
            Players[i].PlayerName.color = PlayerColors[i];


        }


    }

    public void SaveUserName()
    {
        username = nameField.text;
        PlayerPrefs.SetString("username", username);
        Players[0].PlayerName.text = username;
        Players[0].ScoreCard.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = username;

    }

    private IEnumerator MatchMacking()
    {
        cam.transform.DOMove(MatchMakingPos, 0.5f);

        PlayersCountLabel.text = "1/5";
        yield return new WaitForSeconds(Random.Range(0.3f, 0.7f));
        PlayersCountLabel.text = "2/5";
        bots[0].gameObject.SetActive(true);
        yield return new WaitForSeconds(Random.Range(0.3f, 0.7f));
        PlayersCountLabel.text = "3/5";
        bots[1].gameObject.SetActive(true);

        yield return new WaitForSeconds(Random.Range(0.3f, 0.7f));
        PlayersCountLabel.text = "4/5";
        bots[2].gameObject.SetActive(true);

        yield return new WaitForSeconds(Random.Range(0.3f, 0.7f));
        PlayersCountLabel.text = "5/5";
        bots[3].gameObject.SetActive(true);

        yield return new WaitForSeconds(Random.Range(0.3f, 0.7f));
        PlayersCountLabel.transform.parent.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Starting Game";
        yield return new WaitForSeconds(Random.Range(0.4f, 1f));
        zone.gameObject.SetActive(true);
        bots[4].gameObject.SetActive(true);
        matchMakingPanel.SetActive(false);
        gamePanel.SetActive(true);

        cam.transform.DOMove(InGameCamPos, 0.5f).OnComplete(() => {
        CamFollow.enabled = true;
        });

    }

    public void searchGame()
    {
        MenuPanel.gameObject.SetActive(false);
        matchMakingPanel.gameObject.SetActive(true);
        StartCoroutine(MatchMacking());
    }

    public void StartGame()
    {
        gameStarted = true;
    }

}
