// using UnityEngine;
// using UnityEngine.AI;

// public class FlyingMobsAi : MonoBehaviour
// {
//     private NavMeshAgent m_agent;
//     private Animator m_animator;
//     private Transform m_target;
//     public float MoveSpeed;
//     public float FLyingHeight;

//     //Enemy script
//     [Header("Movement")]
//     [SerializeField] private float moveSpeed = 2f;

//     [Header("Attack")]
//     [SerializeField] private float attackDamage = 10f;
//     [SerializeField] private float attackRate = 1f;

//     [Header("Targeting")]
//     [SerializeField] private float attackRange = 1.2f;

//     private Transform player;
//     private FencePart currentFenceTarget;
//     private float nextAttackTime;
    
//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//     void Start()
//     {
//         m_agent= GetComponent<NavMeshAgent>();
//         m_animator= GetComponent<Animator>();
//         m_target = GameObject.FindGameObjectWithTag("Player").transform;
//         float randomAnimSpeed = Random.Range(0.1f, 0.5f);
//         float newAnimSpeed = Mathf.Round(randomAnimSpeed*10)/10;
//         m_animator.speed = newAnimSpeed;
//         m_agent.speed = MoveSpeed;
//         m_agent.height = FLyingHeight;
//         player = GameObject.FindGameObjectWithTag("Player").transform;
//         FindNewFenceTarget();
    
//     }

//     // Update is called once per frame
//     void Update()
//     {
        
//         if (currentFenceTarget != null)
//         {
//             HandleFenceBehavior();
//         }
//         else
//         {
//             MoveTowards(player);
//         }


//     }

//      private void HandleFenceBehavior()
//     {
//         if (currentFenceTarget == null)
//         {
//             FindNewFenceTarget();
//             return;
//         }

//         float distance = Vector3.Distance(transform.position, currentFenceTarget.transform.position);

//         if (distance > attackRange)
//         {
//             MoveTowards(currentFenceTarget.transform);
//         }
//         else if (Time.time >= nextAttackTime)
//         {
//             currentFenceTarget.TakeDamage(attackDamage);
//             nextAttackTime = Time.time + attackRate;
//         }
//     }

//     private void MoveTowards(Transform target)
//     {
//         Vector3 direction = (target.position - transform.position).normalized;
//         transform.position += direction * moveSpeed * Time.deltaTime;
//     }

//     private void FindNewFenceTarget()
//     {
//         FencePart[] fences = FindObjectsOfType<FencePart>();

//         if (fences.Length == 0)
//         {
//             currentFenceTarget = null;
//             return;
//         }

//         currentFenceTarget = fences[0];
//         float shortestDistance = Vector3.Distance(transform.position, fences[0].transform.position);

//         foreach (FencePart fence in fences)
//         {
//             float distance = Vector3.Distance(transform.position, fence.transform.position);
//             if (distance < shortestDistance)
//             {
//                 shortestDistance = distance;
//                 currentFenceTarget = fence;
//             }
//         }
//     }


// }
//Gwen Proposal

using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class FlyingMobsAi : MonoBehaviour
{
    // === Movement & Floating ===
    [Header("Floating Movement")]
    public float moveSpeed = 2f;
    public float flyingHeight = 2f;              // Height above ground (world Y)
    public float floatingAmplitude = 0.25f;      // Bobbing intensity (e.g., 0.25 = ±25cm)
    public float floatingFrequency = 1.8f;       // Bobbing speed (radians per second)

    [Header("Idle Hover (Optional)")]
    public bool enableIdleHover = true;
    public float hoverRadius = 0.4f;
    public float hoverSpeed = 0.7f;

    // === Attack ===
    [Header("Attack")]
    public float attackDamage = 10f;
    public float attackRate = 1f;
    public float attackRange = 1.5f;

    // === References ===
    private NavMeshAgent m_agent;
    private Animator m_animator;
    private Transform player;
    private FencePart currentFenceTarget;
    private float nextAttackTime;

    // === Lifecycle ===
    void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_animator = GetComponent<Animator>();

        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogError("Player not found! Tag it as 'Player'.");
            enabled = false;
            return;
        }
        player = playerObj.transform;

        // Configure NavMeshAgent
        m_agent.speed = moveSpeed;
        m_agent.updateRotation = false;   // We handle rotation manually (optional)
        m_agent.updateUpAxis = false;     // Prevents tilting on slopes — critical for flyers

        // Optional: randomize animation speed
        if (m_animator != null)
        {
            float randomSpeed = Random.Range(0.1f, 0.5f);
            m_animator.speed = Mathf.Round(randomSpeed * 10) / 10;
        }

        FindNewFenceTarget();
    }

    void Update()
    {
        ApplyFloatingEffect();

        if (currentFenceTarget != null)
        {
            HandleFenceBehavior();
        }
        else
        {
            MoveTowards(player);
        }
    }

    // === Movement Logic ===
    private void MoveTowards(Transform target)
    {
        if (target == null) return;
        m_agent.SetDestination(target.position);
    }

    private void HandleFenceBehavior()
    {
        if (currentFenceTarget == null)
        {
            FindNewFenceTarget();
            return;
        }

        float distance = Vector3.Distance(transform.position, currentFenceTarget.transform.position);

        if (distance > attackRange)
        {
            MoveTowards(currentFenceTarget.transform);
        }
        else if (Time.time >= nextAttackTime)
        {
            currentFenceTarget.TakeDamage(attackDamage);
            nextAttackTime = Time.time + attackRate;
        }
    }

    // === Floating & Visual Polish ===
    private void ApplyFloatingEffect()
    {
        // Base position from NavMeshAgent (XZ only)
        Vector3 desiredPosition = m_agent.nextPosition;

        // Optional: Idle hover circle when near destination
        if (enableIdleHover && Vector3.Distance(transform.position, m_agent.destination) < 0.6f)
        {
            float angle = Time.time * hoverSpeed;
            desiredPosition.x += Mathf.Cos(angle) * hoverRadius;
            desiredPosition.z += Mathf.Sin(angle) * hoverRadius;
        }

        // Vertical bobbing using sine wave
        float yOffset = Mathf.Sin(Time.time * floatingFrequency) * floatingAmplitude;
        desiredPosition.y = flyingHeight + yOffset;

        // Apply final position
        transform.position = desiredPosition;

        // Optional: subtle tilt for organic feel
        float tiltAngle = Mathf.Sin(Time.time * floatingFrequency * 0.6f) * 4f;
        transform.rotation = Quaternion.Euler(tiltAngle, transform.eulerAngles.y, 0f);

        // Optional: sync animation (if you have a "FloatPhase" parameter in Animator)
        if (m_animator != null)
        {
            m_animator.SetFloat("FloatPhase", Mathf.Sin(Time.time * floatingFrequency));
        }
    }

    // === Targeting Logic ===
    private void FindNewFenceTarget()
    {
        FencePart[] fences = FindObjectsOfType<FencePart>();
        if (fences.Length == 0)
        {
            currentFenceTarget = null;
            return;
        }

        // Find closest fence by distance
        currentFenceTarget = fences.OrderBy(f => Vector3.Distance(transform.position, f.transform.position)).First();
    }
}