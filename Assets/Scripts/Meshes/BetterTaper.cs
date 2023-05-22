using UnityEngine;
using System.Collections;

/// <summary>
/// A mushroom mesh.
/// </summary>
public class BetterTaper : ProcBase
{
	//stem data:

	//height and radius of the stem:
	public float m_StemHeight = 1.0f;
	public float m_StemRadius = 0.3f;

	//the angle to bend the stem:
	public float m_StemBendAngle = 45.0f;

	//number of radial segments in the stem:
	public int m_StemRadialSegmentCount = 10;

	//number of height segments in the stem:
	public int m_StemHeightSegmentCount = 10;

    Quaternion currentRotation;
    Vector3 currentOffset;

	public override Mesh BuildMesh()
	{
		MeshBuilder meshBuilder = new MeshBuilder();

		//store the current position and rotaion of the stem:
		currentRotation = Quaternion.identity;
		currentOffset = Vector3.zero;

		//build the stem:

		//build a straight stem if m_StemBendAngle is zero:
		if (m_StemBendAngle == 0.0f)
		{
			//straight cylinder:
			float heightInc = m_StemHeight / m_StemHeightSegmentCount;

			for (int i = 0; i <= m_StemHeightSegmentCount; i++)
			{
				currentOffset = Vector3.up * heightInc * i;

				BuildRing(meshBuilder, m_StemRadialSegmentCount, currentOffset, m_StemRadius, (float)i / m_StemHeightSegmentCount, i > 0);
			}
		}
		else
		{
			//get the angle in radians:
			float stemBendRadians = m_StemBendAngle * Mathf.Deg2Rad;

			//the radius of our bend (vertical) circle:
			float stemBendRadius = m_StemHeight / stemBendRadians;

			//the angle increment per height segment (based on arc length):
			float angleInc = stemBendRadians / m_StemHeightSegmentCount;

			//calculate a start offset that will place the centre of the first ring (angle 0.0f) on the mesh origin:
			//(x = cos(0.0f) * bendRadius, y = sin(0.0f) * bendRadius)
			Vector3 startOffset = new Vector3(stemBendRadius, 0.0f, 0.0f);

			//build the rings:
			for (int i = 0; i <= m_StemHeightSegmentCount; i++)
			{
				//current normalised height value:
				float heightNormalised = (float)i / m_StemHeightSegmentCount;

				//unit position along the edge of the vertical circle:
				currentOffset = Vector3.zero;
				currentOffset.x = Mathf.Cos(angleInc * i);
				currentOffset.y = Mathf.Sin(angleInc * i);

				//rotation at that position on the circle:
				float zAngleDegrees = angleInc * i * Mathf.Rad2Deg;
				currentRotation = Quaternion.Euler(0.0f, 0.0f, zAngleDegrees);

				//multiply the unit postion by the radius:
				currentOffset *= stemBendRadius;

				//offset the position so that the base ring (at angle zero) centres around zero:
				currentOffset -= startOffset;

				//build the ring:
				BuildRing(meshBuilder, m_StemRadialSegmentCount, currentOffset, m_StemRadius, heightNormalised, i > 0, currentRotation);
			}
		}

		return meshBuilder.CreateMesh();
	}
}
