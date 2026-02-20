// using UnityEngine;

// public class EnemyAi : MonoBehaviour
// {
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

//     private void Start()
//     {
//         player = GameObject.FindGameObjectWithTag("Player").transform;
//         FindNewFenceTarget();
//     }

//     private void Update()
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

//     private void HandleFenceBehavior()
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

// With navMesh agent:

using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAi : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float repathInterval = 0.2f;

    [Header("Attack")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackRate = 1f;
    [SerializeField] private float attackRange = 1.2f;

    [Header("Blocker Detection")]
    [Tooltip("Used to detect a fence segment between enemy and player.")]
    [SerializeField] private float blockerRayHeight = 0.6f;

    [Tooltip("If straight ray misses (because of height/colliders), we try a thicker cast.")]
    [SerializeField] private float blockerSphereRadius = 0.35f;

    [Tooltip("Only fence parts should be on this layer for best results.")]
    [SerializeField] private LayerMask fenceLayerMask;

    [Header("Player NavMesh")]
    [Tooltip("How far to search around player to find a NavMesh point.")]
    [SerializeField] private float playerNavSampleDistance = 3f;

    private NavMeshAgent agent;
    private Transform player;

    private FencePart currentFenceTarget;

    private float nextAttackTime;
    private float nextRepathTime;

    // Re-used path object (no GC allocations)
    private NavMeshPath path;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
    }

    private void Start()
    {
        agent.speed = moveSpeed;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        player = p != null ? p.transform : null;
    }

    private void Update()
    {
        if (player == null)
        {
            StopAgent();
            return;
        }

        // 1) If we are currently breaking a fence, do that first.
        if (currentFenceTarget != null)
        {
            HandleFenceAttack();
            return;
        }

        // 2) Always try to chase player.
        if (!TryGetPlayerNavPoint(out Vector3 playerNavPoint))
        {
            // Player not near NavMesh: can't path properly.
            StopAgent();
            return;
        }

        // 3) Check if path to player is complete.
        bool pathComplete = CalculatePathTo(playerNavPoint);

        if (pathComplete)
        {
            // No blocker (NavMesh-wise): chase player.
            Chase(playerNavPoint);
            return;
        }

        // 4) Path is NOT complete -> find what's blocking (fence part) and break it.
        FencePart blocker = FindBlockingFence();

        if (blocker != null)
        {
            currentFenceTarget = blocker;
            // Force immediate movement toward it
            nextRepathTime = 0f;
            agent.isStopped = false;
            Chase(currentFenceTarget.transform.position);
        }
        else
        {
            // No fence found in front, but path is still partial.
            // Move toward the last reachable corner to "press" the blockade.
            if (path.corners != null && path.corners.Length > 0)
            {
                Vector3 lastCorner = path.corners[path.corners.Length - 1];
                Chase(lastCorner);
            }
            else
            {
                StopAgent();
            }
        }
    }

    private void HandleFenceAttack()
    {
        // If it got destroyed by someone else, clear and continue to player.
        if (currentFenceTarget == null)
        {
            ResumeChasePlayerImmediate();
            return;
        }

        float dist = Vector3.Distance(transform.position, currentFenceTarget.transform.position);

        if (dist > attackRange)
        {
            Chase(currentFenceTarget.transform.position);
            return;
        }

        // In range: attack fence
        StopAgent();
        FaceTarget(currentFenceTarget.transform);

        if (Time.time < nextAttackTime) return;
        nextAttackTime = Time.time + attackRate;

        currentFenceTarget.TakeDamage(attackDamage);

        // After TakeDamage, if fence was destroyed (Destroy(gameObject)),
        // Unity will turn this reference into null by next frame (often immediately).
        if (currentFenceTarget == null)
        {
            // Carving updates may take a frame, but we immediately resume chasing.
            ResumeChasePlayerImmediate();
        }
    }

    private void ResumeChasePlayerImmediate()
    {
        currentFenceTarget = null;
        agent.isStopped = false;
        nextRepathTime = 0f;

        // We don’t assume path is instantly updated; we just try again next Update,
        // but this prevents the “stand still after breaking” issue.
    }

    private bool TryGetPlayerNavPoint(out Vector3 playerNavPoint)
    {
        playerNavPoint = default;

        if (!agent.enabled || !agent.isOnNavMesh)
            return false;

        // Sample player's position onto NavMesh so CalculatePath works reliably.
        if (NavMesh.SamplePosition(player.position, out NavMeshHit hit, playerNavSampleDistance, NavMesh.AllAreas))
        {
            playerNavPoint = hit.position;
            return true;
        }

        return false;
    }

    private bool CalculatePathTo(Vector3 destination)
    {
        bool ok = agent.CalculatePath(destination, path);
        if (!ok) return false;

        return path.status == NavMeshPathStatus.PathComplete;
    }

    private FencePart FindBlockingFence()
    {
        // We detect the fence that literally blocks the line between enemy and player.
        // This is stable and “feels right” (they hit what’s in front of them).

        Vector3 from = transform.position + Vector3.up * blockerRayHeight;
        Vector3 to = player.position + Vector3.up * blockerRayHeight;
        Vector3 dir = (to - from);
        float dist = dir.magnitude;

        if (dist < 0.01f) return null;
        dir /= dist;

        // 1) Try a raycast
        if (Physics.Raycast(from, dir, out RaycastHit hit, dist, fenceLayerMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.TryGetComponent<FencePart>(out var fence))
                return fence;

            // If FencePart is on parent:
            return hit.collider.GetComponentInParent<FencePart>();
        }

        // 2) Fallback: spherecast (helps if ray goes between colliders)
        if (Physics.SphereCast(from, blockerSphereRadius, dir, out hit, dist, fenceLayerMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.TryGetComponent<FencePart>(out var fence))
                return fence;

            return hit.collider.GetComponentInParent<FencePart>();
        }

        return null;
    }

    private void Chase(Vector3 destination)
    {
        if (Time.time < nextRepathTime) return;
        nextRepathTime = Time.time + repathInterval;

        if (!agent.enabled || !agent.isOnNavMesh) return;

        agent.isStopped = false;
        agent.SetDestination(destination);
    }

    private void StopAgent()
    {
        if (agent.enabled)
            agent.isStopped = true;
    }

    private void FaceTarget(Transform target)
    {
        Vector3 dir = target.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;

        transform.rotation = Quaternion.LookRotation(dir);
    }
}
