using UnityEngine;

public class ProceduralTree : MonoBehaviour
{
    public Taper tree;

    public Material leaves;

    public int MaxGenerations;

    public float yMaxRotation = 45f;

    public float branchProbability = 30f;

    public float randomBendAngle = 50f;

    private bool lastGeneration = false;

    void Start()
    {
        var root = Instantiate(tree, transform.position, Quaternion.identity);
        root.currentOffset = TaperEnding(root);
        root.transform.parent = transform;
        root.CreateBranch();
        CreateBranches(root, 0, lastGeneration);
    }

    private Taper CreateBranch(Taper root, Vector3 rotation, bool lastGeneration)
    {
        var branch = Instantiate(tree, root.currentOffset, Quaternion.identity);
        var randomBend = Random.Range(0f, randomBendAngle);

        branch.currentOffset = TaperEnding(root);
        branch.transform.parent = root.transform;
        branch.transform.localPosition = root.currentOffset;

        branch.transform.Rotate(rotation, Space.World);
        branch.m_RadiusStart = root.m_RadiusStart / 2;
        branch.m_Height = root.m_Height * 0.67f;
        branch.m_BendAngle = randomBend;

        if (lastGeneration)
        {
            branch.GetComponent<MeshRenderer> ().material = leaves;
        }

        branch.CreateBranch();

        return branch;
    }

    private void CreateBranches(Taper root, int currentGeneration, bool lastGeneration)
    {
        currentGeneration++;
        lastGeneration = false;
        if(currentGeneration <= MaxGenerations)
        {
            if (currentGeneration == MaxGenerations)
            {
                lastGeneration = true;
            }

            var random = Random.Range(0f, 100f);

            if (random > branchProbability)
            {
                var rightBranch = CreateBranch(root, new Vector3(0, Random.Range(0f, yMaxRotation), -45), lastGeneration);
                CreateBranches(rightBranch, currentGeneration, lastGeneration);
            }

            random = Random.Range(0f, 100f);
            if (random > branchProbability)
            {
                var leftBranch = CreateBranch(root, new Vector3(0, Random.Range(0f, yMaxRotation), 45), lastGeneration);
                CreateBranches(leftBranch, currentGeneration, lastGeneration);
            }

            random = Random.Range(0f, 100f);
            if (random > branchProbability)
            {
                var backBranch = CreateBranch(root, new Vector3(45, Random.Range(0f, yMaxRotation), 0), lastGeneration);
                CreateBranches(backBranch, currentGeneration, lastGeneration);
            }

            random = Random.Range(0f, 100f);
            if (random > branchProbability)
            {
                var frontBranch = CreateBranch(root, new Vector3(-45, Random.Range(0f, yMaxRotation), 0), lastGeneration);
                CreateBranches(frontBranch, currentGeneration, lastGeneration);
            }
        }
        else 
        {
            return;
        }
    }

    private Vector3 TaperEnding(Taper taper)
    {
        float bendAngleRadians = taper.m_BendAngle * Mathf.Deg2Rad;
        float bendRadius = taper.m_Height / bendAngleRadians;

        Vector3 currentOffset = Vector3.zero;
        currentOffset.x = Mathf.Cos(bendAngleRadians);
        currentOffset.y = Mathf.Sin(bendAngleRadians);

        currentOffset *= bendRadius;

        Vector3 startOffset = new Vector3(bendRadius, 0.0f, 0.0f);

        currentOffset -= startOffset;

        return currentOffset;
    }
}
