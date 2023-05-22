using UnityEngine;

/// <summary>
/// A cylinder with both bend and taper deformations.
/// </summary>
public class Taper : Base
{
    //public GameObject testLine;

	//the radii at the start and end of the cylinder:
	public float m_RadiusStart = 0.5f;
	public float m_RadiusEnd = 0.0f;

	//the height of the cylinder:
	public float m_Height = 2.0f;

	//the angle to bend the cylinder:
	public float m_BendAngle = 90.0f;

	//the number of radial segments:
	public int m_RadialSegmentCount = 10;

	//the number of height segments:
	public int m_HeightSegmentCount = 4;

	public MeshBuilder meshBuilder;

    public Quaternion currentRotation;

    public Vector3 currentOffset;

	public string tapperName;

    public void CreateBranch()
    {
        //Build the mesh:
		Mesh mesh = BuildMesh();

		//Look for a MeshFilter component attached to this GameObject:
		MeshFilter filter = GetComponent<MeshFilter>();

		//If the MeshFilter exists, attach the new mesh to it.
		//Assuming the GameObject also has a renderer attached, our new mesh will now be visible in the scene.
		if (filter != null)
		{
			filter.sharedMesh = mesh;
		}
    }

	public override Mesh BuildMesh()
	{
		meshBuilder = new MeshBuilder();

        //store the current position and rotaion of the stem:
		currentRotation = Quaternion.identity;
		currentOffset = Vector3.zero;

		//our bend code breaks if m_BendAngle is zero:
		if (m_BendAngle == 0.0f)
		{
			//taper only:
			float heightInc = m_Height / m_HeightSegmentCount;

			//calculate the slope of the cylinder based on the height and difference between radii:
			Vector2 slope = new Vector2(m_RadiusEnd - m_RadiusStart, m_Height);
			slope.Normalize();

			//build the rings:
			for (int i = 0; i <= m_HeightSegmentCount; i++)
			{
				//centre position of this ring:
				Vector3 centrePos = Vector3.up * heightInc * i;

				//V coordinate is based on height:
				float v = (float)i / m_HeightSegmentCount;

				//interpolate between the radii:
				float radius = Mathf.Lerp(m_RadiusStart, m_RadiusEnd, (float)i / m_HeightSegmentCount);

                currentOffset = Vector3.up * heightInc * i;

				//build the ring:
				BuildRing(meshBuilder, m_RadialSegmentCount, centrePos, radius, v, i > 0, Quaternion.identity, slope);
			}
		}
		else
		{
			//bend and taper:

			//get the angle in radians:
			float bendAngleRadians = m_BendAngle * Mathf.Deg2Rad;

			//the radius of our bend (vertical) circle:
			float bendRadius = m_Height / bendAngleRadians;

			//the angle increment per height segment (based on arc length):
			float angleInc = bendAngleRadians / m_HeightSegmentCount;

			//calculate a start offset that will place the centre of the first ring (angle 0.0f) on the mesh origin:
			//(x = cos(0.0f) * bendRadius, y = sin(0.0f) * bendRadius)
			Vector3 startOffset = new Vector3(bendRadius, 0.0f, 0.0f);

			//calculate the slope of the cylinder based on the height and difference between radii:
			Vector2 slope = new Vector2(m_RadiusEnd - m_RadiusStart, m_Height);
			slope.Normalize();

			//build the rings:
			for (int i = 0; i <= m_HeightSegmentCount; i++)
			{
				//unit position along the edge of the vertical circle:
				currentOffset = Vector3.zero;
				currentOffset.x = Mathf.Cos(angleInc * i);
				currentOffset.y = Mathf.Sin(angleInc * i);

				//rotation at that position on the circle:
				float zAngleDegrees = angleInc * i * Mathf.Rad2Deg;
				currentRotation = Quaternion.Euler(0.0f, 0.0f, zAngleDegrees);

				//multiply the unit postion by the radius:
				currentOffset *= bendRadius;

				//offset the position so that the base ring (at angle zero) centres around zero:
				currentOffset -= startOffset;

				//interpolate between the radii:
				float radius = Mathf.Lerp(m_RadiusStart, m_RadiusEnd, (float)i / m_HeightSegmentCount);

				//V coordinate is based on height:
				float v = (float)i / m_HeightSegmentCount;

				//build the ring:
				BuildRing(meshBuilder, m_RadialSegmentCount, currentOffset, radius, v, i > 0, currentRotation, slope);
			}
		}

        // testLine.transform.position = currentOffset + gameObject.transform.position;
        // testLine.transform.rotation = currentRotation;
        // Instantiate(testLine);

		return meshBuilder.CreateMesh();
	}
}