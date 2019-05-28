using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public delegate void CurrencyChanged();

public class GameManager : Singleton<GameManager>
{
    public event CurrencyChanged Changed;

    public TowerBtn ClickedBtn { get; set; }

    private int currency;

    private int wave = 0;

    private int increaseHealth = 0;

    private int lives;

    private bool gameOver = false;

    private int health;

    [SerializeField]
    private Text livesTxt;

    private int maxMonster =  1;

    [SerializeField]
    private Text waveTxt;

    [SerializeField]
    private Text currencyTxt;

    [SerializeField]
    private GameObject waveBtn;

    [SerializeField]
    private GameObject gameOverMenu;

    [SerializeField]
    private GameObject upgradePanel;

    [SerializeField]
    private Text sellTxt;

    [SerializeField]
    private GameObject inGameMenu;

    [SerializeField]
    private GameObject optionMenu;

    private Tower selectedTower;


    public List<int> score = new List<int>();

    private List<Monster> activeMonsters = new List<Monster>();

    public ObjectPool Pool { get; set; }

    public bool WaveActive
    {
        get { return activeMonsters.Count > 0; }
    }

    public int Currency
    {
        get
        {
            return currency;
        }

        set
        {
            this.currency = value;
            this.currencyTxt.text = value.ToString() + "<color=yellow>$</color>";

            OnCurrencyChanged();
        }
    }

    public int Lives
    {
        get
        {
            return lives;
        }

        set
        {
            this.lives = value;

            if (lives <= 0)
            {
                this.lives = 0;
                GameOver();
            }

            this.livesTxt.text = lives.ToString();
        }
    }

    private void Awake()
    {
        Pool = GetComponent<ObjectPool>();
    }



    // Start is called before the first frame update
    void Start()
    {
        Lives = 10;
        Currency = 300;
    }

    // Update is called once per frame
    void Update()
    {
        HandleEscape();
    }

    public void PickTower(TowerBtn towerBtn)
    {
        if (Currency >= towerBtn.Price)
        {   
            this.ClickedBtn = towerBtn;
            Hover.Instance.Activate(towerBtn.Sprite);
        }
         
    }

    public void BuyTower()
    {
        if (Currency >= ClickedBtn.Price)
        {
            Currency -= ClickedBtn.Price;

            Hover.Instance.Deactivate();
        }
    }

    public  void OnCurrencyChanged()
    {
        
        if (Changed != null)
        {
            Changed();
            Debug.Log("change");

        }
    }

    public void SelectTower(Tower tower)
    {
        if (selectedTower != null)
        {
            selectedTower.Select();
        }
        selectedTower = tower;
        selectedTower.Select();

        sellTxt.text = "+ " + (selectedTower.Price / 2) + "<color=yellow>$</color>";

        upgradePanel.SetActive(true);
    }

    public void DeselectTower()
    {
        if (selectedTower != null)
        {
            selectedTower.Select();
        }

        upgradePanel.SetActive(false);

        selectedTower = null;
    }

   

        

    private void HandleEscape()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {   
            if (selectedTower == null && !Hover.Instance.IsVisible)
            {
                ShowInGameMenu();
            }
            else if (Hover.Instance.IsVisible)
            {
                DropTower();
            }else if(selectedTower != null)
            {
                DeselectTower();
            }
        }
    }

    public void StartWave()
    {
        wave++;

        waveTxt.text = string.Format("Wave: <color=yellow>{0}</color>", wave);


        StartCoroutine(SpawnWave());

        waveBtn.SetActive(false);
    }

    private IEnumerator SpawnWave()
    {
        LevelManager.Instance.GeneratePath();

        for (int i = 0; i < maxMonster; i++)
        {
            LevelManager.Instance.GeneratePath();

            int monsterIndex = Random.Range(0, 4);


            string type = string.Empty;
            switch (monsterIndex)
            {
                case 0:
                    type = "NormalOrc";
                    health = 40;
                    break;
                case 1:
                    type = "BossOrc";
                    health = 50;
                    break;
                case 2:
                    type = "TankOrc";
                    health = 60;
                    break;
                case 3:
                    type = "MageOrc";
                    health = 20;
                    break;
            }
            if (wave % 2 == 0)
            {
                increaseHealth += 10;
                health += increaseHealth;

            }
            Monster monster = Pool.GetObject(type).GetComponent<Monster>();
            monster.Spawn(health);

            

            activeMonsters.Add(monster);

            yield return new WaitForSeconds(1f);

            
        }

        maxMonster += 2;



    }

    public void RemoveMonster(Monster monster)
    {
        activeMonsters.Remove(monster);
        if (!WaveActive && !gameOver)
        {
            waveBtn.SetActive(true);
        }
    }

    public void GameOver()
    {
        if (!gameOver)
        {
            gameOver = true;
            gameOverMenu.SetActive(true);
            //score.Add(wave);
            //score.Sort();
            //score.Reverse();
            //for (int i = 0; i < score.Count; i++)
            //{
            //    Debug.Log((i+1).ToString() + ": " + score[i].ToString()+ " Waves");
            //}
        }
    }

    public void Restart()
    {
        Time.timeScale = 1;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void SellTower()
    {
        if (selectedTower != null)
        {
            Currency += selectedTower.Price / 2;

            selectedTower.GetComponentInParent<TileScript>().IsEmpty = true;

            Destroy(selectedTower.transform.parent.gameObject);

            DeselectTower();
        }
    }

    public void ShowInGameMenu()
    {
        if (optionMenu.activeSelf)
        {
            ShowMain();
        }
        else
        {
            inGameMenu.SetActive(!inGameMenu.activeSelf);
            if (!inGameMenu.activeSelf)
            {
                Time.timeScale = 1;
            }
            else
            {
                Time.timeScale = 0;
            }
        }
        
    }

    private void DropTower()
    {
        ClickedBtn = null;
        Hover.Instance.Deactivate();
    }

    public void ShowOption()
    {
        inGameMenu.SetActive(false);
        optionMenu.SetActive(true);
    }
    public void ShowMain()
    {
        inGameMenu.SetActive(true);
        optionMenu.SetActive(false);
    }
}
