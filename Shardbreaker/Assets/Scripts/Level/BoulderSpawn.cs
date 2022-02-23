using UnityEngine;

public class BoulderSpawn : MonoBehaviour
{
    [SerializeField] protected float timeBetweenSpawns = 1f;
    private float lastSpawnTime = 0f;
    [SerializeField] protected Transform projectileSpawn;
    [SerializeField] protected GameObject projectilePrefab;
    public Transform[] targets;
    public bool isEnabled;
    private int index = 0;

    [Range(0f, 5f)] [SerializeField] private float posMin = 1.2f;
    [Range(0f, 5f)] [SerializeField] private float posMax = 1.8f;
    [Range(0f, 5f)] [SerializeField] private float scaleMin = 1f;
    [Range(0f, 5f)] [SerializeField] private float scaleMax = 3f;

    private System.Random rnd = new System.Random();

    private void Awake()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            var meshRenderer = targets[i].GetComponent<MeshRenderer>();
            if (meshRenderer)
            {
                meshRenderer.enabled = false;
            }         
        }
    }

    void Update()
    {
        if (isEnabled)
        {
            ThrowBoulder();
        }
    }

    public void ThrowBoulder()
    {
        if (lastSpawnTime + timeBetweenSpawns <= Time.time)
        {         
            lastSpawnTime = Time.time;
            index = rnd.Next(0, targets.Length);
            transform.LookAt(targets[index].position);
            CreateInstance();
        }
    }

    private void CreateInstance()
    {
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawn.position, projectileSpawn.transform.rotation);
        projectile.transform.localScale = RandomScale();
        projectile.GetComponent<Projectile>().Speed = SetSpeed() / RandomSpeedOffset();
    }

    private float SetSpeed()
    {
        float speedValue = Vector3.Distance(new Vector3(0, transform.position.y, 0), new Vector3(0, targets[index].position.y, 0));
        return speedValue;
    }


    private float RandomSpeedOffset()
    {
        float random = Random.Range(posMin, posMax);
        return random;
    }

    private Vector3 RandomScale()
    {
        float randomScale = Random.Range(scaleMin, scaleMax);
        return new Vector3(randomScale, randomScale, randomScale);
    }

    public void ToggleState()
    {
        isEnabled = !isEnabled;
    }
}
