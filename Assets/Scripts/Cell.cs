using System.Collections;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] private Rigidbody2D _rb;
    public SpriteRenderer sr;
    [SerializeField] private GameObject _deathParticle;
    [SerializeField] private GameObject _damageParticle;
    [SerializeField] private GameObject _sickDeathParticle;

    [Header("Main Stats")]
    public int maxHP;
    public int currentHP;
    public int energy;
    public int needsEnergy;
    public int currentMoves;
    public int maxMoves;
    public float deathSpeed;
    public bool canReproduce = true;

    [Header("Force")]
    public float minForce;
    public float maxForce;

    [Header("Rotation")]
    public float maxZRotationAngle;
    public float minRotationTime;
    public float maxRotationTime;
    public float rotationSpeed;

    [Header("Scale")]
    public Vector3 growthOnEat;
    public float minScale;
    public float maxScale;
    [SerializeField] private float _newAxisScale;

    private WaitForSeconds _waitTime = new WaitForSeconds(2);

    private void Awake()
    {
        // Scale
        _newAxisScale = Random.Range(minScale, maxScale);
        transform.localScale = new Vector2(_newAxisScale, _newAxisScale);

        // Stats
        currentHP = maxHP;
        CellSpawner.CellsCount++;
    }

    private void Start()
    {
        StartCoroutine(WanderRoutine());
    }

    IEnumerator WanderRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1f, 3f));

            // Getting new Angle
            float targetZ = (Random.value > 0.5f) ? maxZRotationAngle : -maxZRotationAngle;
            Quaternion targetAngle = Quaternion.Euler(0, 0, transform.eulerAngles.z + targetZ);

            // Setting time
            float startTime = Time.time;
            float targetTime = startTime + Random.Range(minRotationTime, maxRotationTime);

            // Rotation
            while(Time.time < targetTime)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetAngle, rotationSpeed * Time.deltaTime);
                yield return null;
            }
            // The cell can be pushed, so the rotation will never be precise

            // Move
            _rb.linearVelocity = transform.up * Random.Range(minForce, maxForce);
            
            // Checking if we are too old
            currentMoves++;
            if (currentMoves > maxMoves)
            {
                _rb.linearVelocity = Vector2.zero;
                Die();
                yield break;
            }

            if (CellSpawner.CellsCount > 100)
            {
                if (Random.value > 0.5f)
                {
                    Instantiate(_sickDeathParticle, transform.position, Quaternion.identity);
                    Die();
                    yield break;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Cell"))
        {
            Cell cell = collision.gameObject.GetComponent<Cell>();

            if (collision.gameObject.transform.localScale.x < transform.localScale.x && cell.sr.color != sr.color)
            {
                if (canReproduce)
                {
                    cell.currentHP--;
                    energy++;
                    transform.localScale += growthOnEat;
                    

                    if (energy >= needsEnergy)
                    {
                        StartCoroutine(WaitAndSpawnCell());
                        energy = 0;
                    }

                    if (cell.currentHP <= 0)
                    {
                        cell.Die();
                        Instantiate(_deathParticle, cell.transform.position, Quaternion.identity);
                    }
                    else
                    {
                        Instantiate(_damageParticle, cell.transform.position, Quaternion.identity);
                    }
                }
            }
        }
    }

    public void Die()
    {
        StopAllCoroutines();
        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        while (transform.localScale.x > 0)
        {
            transform.localScale = Vector2.MoveTowards(transform.localScale, Vector2.zero, deathSpeed * Time.deltaTime);
            yield return null;
        }
        CellSpawner.CellsCount--;
        Destroy(gameObject);
    }

    IEnumerator WaitAndSpawnCell()
    {
        canReproduce = false;
        yield return _waitTime;
        transform.localScale = new Vector2(_newAxisScale, _newAxisScale);
        CellSpawner.Instanse.SpawnNewCell(transform.position, sr.color);
        canReproduce = true;
    }
}
