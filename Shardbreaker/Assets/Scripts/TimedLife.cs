using UnityEngine;

public class TimedLife : MonoBehaviour
{
    [Range(0.10f, 10f)][SerializeField] private float lifeTime = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
