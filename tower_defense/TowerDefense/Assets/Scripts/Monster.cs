using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField]
    private float speed;

    private SpriteRenderer mySpriteRenderer;

    [SerializeField]
    private Stat health;

    public bool Alive
    {
        get
        {
            return health.CurrentVal > 0;
        }
    }

    private Stack<Node> path;

    private List<Debuff> debuffs = new List<Debuff>();

    [SerializeField]
    private Element elementType;

    private int invulnerability = 2;

    

    public Point Gridposition { get; set; }

    private Vector3 destination;

    private void Awake()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        health.Initialize();    
    }

    public bool IsActive { get; set; }

    private void Update()
    {
        //HandleDebuffs();
        Move();
    }

    public void Spawn(int health)
    {
        transform.position = LevelManager.Instance.BluePortal.transform.position;
        this.health.Bar.Reset();

        this.health.MaxVal = health;
        this.health.CurrentVal = this.health.MaxVal;

        StartCoroutine(Scale(new Vector3(0.1f, 0.1f), new Vector3(1, 1), false));

        SetPath(LevelManager.Instance.Path);
    }

    public IEnumerator Scale(Vector3 from, Vector3 to, bool remove)
    {


        float progress = 0;
        while (progress <= 1)
        {
            transform.localScale = Vector3.Lerp(from, to, progress);

            progress += Time.deltaTime;

            yield return null;

        }

        transform.localScale = to;

        IsActive = true;


        if (remove)
        {
            Release();
        }
    }

    private void Move()
    {
        if (IsActive)
        {
            transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.deltaTime);

            if (transform.position == destination)
            {
                if (path != null && path.Count > 0)
                {
                    Gridposition = path.Peek().GridPosition;
                    destination = path.Pop().WorldPosition;
                }
            }
        }

        
    }

    private void SetPath(Stack<Node> newPath)
    {
        if (newPath != null)
        {
            this.path = newPath;
            Gridposition = path.Peek().GridPosition;
            destination = path.Pop().WorldPosition;
        }
    }

    //animation

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "RedPortal")
        {
            StartCoroutine(Scale(new Vector3(1, 1), new Vector3(0.1f, 0.1f), true));

            GameManager.Instance.Lives--;
        }

        if (other.tag == "Tile")
        {
            //mySpriteRenderer.sortingOrder = other.GetComponents<TileScript>().GridPosition.Y;
        }
    }

    public void Release()
    {
        IsActive = false;
        Gridposition = LevelManager.Instance.BlueSpawn;
        GameManager.Instance.Pool.ReleaseObject(gameObject);
        GameManager.Instance.RemoveMonster(this);
    }

    public void TakeDamage(int damage, Element dmgSource)
    {
        if (IsActive)
        {
            if (dmgSource == elementType)
            {
                damage = damage * invulnerability;
                //invulnerability++;
            }
            
            health.CurrentVal -= damage;

            if (health.CurrentVal <= 0)
            {
                SoundManager.Instance.PlaySFX("ou");

                GameManager.Instance.Currency += 50;

                GetComponent<Monster>().Release();


                IsActive = false;

                //GetComponent<SpriteRenderer>().sortingOrder--;
            }
        }
        
    }

    //public void AddDebuff(Debuff debuff)
    //{
    //    if(!debuffs.Exists(x=> x.GetType()  == debuff.GetType()))
    //    {
    //       debuffs.Add(debuff);
    //    }
    //}

    //private void HandleDebuffs()
    //{
    //    foreach (Debuff debuff in debuffs)
    //    {
    //        //debuff.Update();
    //    }
    //}

}
