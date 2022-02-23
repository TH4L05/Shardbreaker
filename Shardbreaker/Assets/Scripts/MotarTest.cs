using UnityEngine;

public class MotarTest : MonoBehaviour
{
    [SerializeField] protected float timeBetweenShots = 0.25f;
    [SerializeField] protected float waitTime = 2f;
    private float lastShotTime = 0f;
    [SerializeField] protected Transform projectileSpawn;
    [SerializeField] protected Transform pivot;
    [SerializeField] protected GameObject bullet;
    [Range(1,100)] [SerializeField] protected int bulletAmount = 1;
    public Timer timer;

    private System.Random rnd = new System.Random();
    private int amountIndex = 0;
    private bool run = false;
    public bool isEnabled;

    private void Start()
    {
        run = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnabled)
        {
            if (!timer.Timer_isRunning && !run)
            {
                run = true;
            }


            if (run)
            {
                for (int i = 0; i < bulletAmount; i++)
                {
                    CreateBulletInstance();
                }
            }



            if (amountIndex == bulletAmount)
            {
                amountIndex = 0;
                run = false;
                timer.StartTimer(waitTime, true);
            }
        }
        
    }

    public void CreateBulletInstance()
    {
            if (lastShotTime + timeBetweenShots <= Time.time)
            {
                lastShotTime = Time.time;
                int random_y = rnd.Next(0, 360);    
                pivot.Rotate(new Vector3(0, random_y, 0));  
                GameObject projectile = Instantiate(bullet, projectileSpawn.position, projectileSpawn.transform.rotation);
                projectile.GetComponent<Projectile>().Speed = 8f;
                amountIndex++;
            }  
        
    }
}
