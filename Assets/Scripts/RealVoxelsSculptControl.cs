using UnityEngine;
using System.Collections;

public class RealVoxelsSculptControl : MonoBehaviour, IGetSculptController {

	//public Vector3 numPoints = new Vector3(8, 10, 8);
	public int numPointsX = 8;
	public int numPointsY = 10;
	public int numPointsZ = 8;
	public float pointGap = 0.02f;
	private Vector3 cubeScale;

	private Vector3 numPointsCenter;
	private float minPointRadius;



	bool[,,] voxels;
	GameObject[,,] voxelObjs;

	private GameObject m_selectObject;

	private bool m_voxelsDirty = true;

	private GameObject m_container;

	private SculptGUIController m_sculptController;

	// Use this for initialization
	void Start () {
		cubeScale = Vector3.one * 0.9f * pointGap;
		numPointsCenter = new Vector3(numPointsX, numPointsY, numPointsZ);
		numPointsCenter = numPointsCenter / 2.0f;
		voxels = new bool[numPointsX, numPointsY, numPointsZ];
		int i, j, k;

		minPointRadius = Mathf.Min(numPointsCenter.x, numPointsCenter.y);
		minPointRadius = Mathf.Min(minPointRadius, numPointsCenter.y);
		for (i = 0; i < numPointsX; i++) {
			for (j = 0; j < numPointsY; j++) {
				for (k = 0; k < numPointsZ; k++) {
					//if (Random.Range(0, 10) < 6) {
					//	voxels[i, j, k] = true;
					//}

					voxels[i, j, k] = (new Vector3(i, j, k) - numPointsCenter).magnitude <= minPointRadius;

				}
			}
		}
	}

	private void setObjColor(GameObject obj, Color col) {
		MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
		renderer.material.color = col;
	}
	
	// Update is called once per frame
	void Update () {
		if (m_voxelsDirty) {
			updateVoxels();
			m_voxelsDirty = false;
		}

		if (Input.touchCount != 1) {
			return;
		}

		Touch t = Input.GetTouch(0);
		//if (t.phase == TouchPhase.Began || t.phase == TouchPhase.Moved || t.phase == TouchPhase.) {
		if (t.phase == TouchPhase.Canceled || t.phase == TouchPhase.Ended) {
			if (m_selectObject != null) {
				setObjColor(m_selectObject, Color.white);
			}
			m_selectObject = null;
			return;
		}

		Ray ray = Camera.main.ScreenPointToRay(t.position);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 100.0F)) {
			GameObject obj = hit.collider.gameObject;
			if (m_sculptController.IsAdding()) {
				if (obj != m_selectObject) {
					if (m_selectObject != null) {
						setObjColor(m_selectObject, Color.white);
					}
					//m_selectObject = obj;
					//setObjColor(m_selectObject, Color.red);
				}

				Vector3 localNormal = transform.InverseTransformDirection(hit.normal);

				localNormal.Normalize();
				Vector3 addObjPos = obj.transform.localPosition + localNormal * pointGap;
				int x, y, z;
				if (voxelFromPos(addObjPos, out x, out y, out z)) {
					if (!voxels[x, y, z]) {
						voxels[x, y, z] = true;
						GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
						setObjColor(cube, voxelColor(x, y, z));
						voxelObjs[x, y, z] = cube;
						Transform tr = cube.transform;
						tr.SetParent(m_container.transform, false);
						tr.localScale = cubeScale;
						tr.localPosition = posFromVoxel(x, y, z);
					}

				} else {
					//Debug.Log("Not voxel! " + x + " " + y + " " + z);
				}

				// identify cube from position
				/*Vector3 pos = obj.transform.localPosition;
				int x, y, z;
				if (voxelFromPos(pos, out x, out y, out z)) {
					Debug.Log("Select voxel " + x + " " + y + " " + z);
				} else {
					//Debug.Log("Not voxel! " + x + " " + y + " " + z);
				}*/
				// based on face of object hit, add a new cube

			} else {
				// remove selected
				if (obj == m_selectObject) {
					m_selectObject = null; 
				}

				int x, y, z;
				if (voxelFromPos(obj.transform.localPosition, out x, out y, out z)) {
					voxels[x, y, z] = false;
					voxelObjs[x, y, z] = null;
				}
				Destroy(obj);
				//Debug.Log("Hit " + hit.point);
				//transform.position = ray.GetPoint(100.0F);
				//m_lastHit = hit.point;
				//m_select.SetActive(true);
				//m_select.transform.position = m_lastHit;
				//} else {
				//Debug.Log("Miss");
			}
		} else {
			if (m_selectObject != null) {
				setObjColor(m_selectObject, Color.white);
			}
			m_selectObject = null;
		}
		//}
	}

	private bool voxelFromPos(Vector3 pos, out int x, out int y, out int z) {
		//Debug.Log("Check pos " + pos.x + " " + pos.y + " " + pos.z);
		pos.x += (numPointsX - 1) * pointGap / 2.0f;
		pos.z += (numPointsZ - 1) * pointGap / 2.0f;

		// round up to ensure divide and floor picks the right index
		x = Mathf.FloorToInt(pos.x / pointGap + 0.5f);
		y = Mathf.FloorToInt(pos.y / pointGap + 0.5f);
		z = Mathf.FloorToInt(pos.z / pointGap + 0.5f);
		return x >= 0 && x < numPointsX && y >= 0 && y < numPointsY && z >= 0 && z < numPointsZ;
	}

	private Vector3 posFromVoxel(int x, int y, int z) {
		return new Vector3(
			(numPointsX - 1) * pointGap / -2.0f + pointGap * x,
			pointGap * y,
			(numPointsZ - 1) * pointGap / -2.0f + pointGap * z
		);
	}

	private Color voxelColor(int x, int y, int z) {
		float dist = (new Vector3(x, y, z) - numPointsCenter).magnitude;
		float r = Mathf.Clamp01(dist / minPointRadius);
		return new Color(1-r, 0, r);
	}

	private void updateVoxels() {
		Debug.Log("updateVoxels");
		if (m_container != null) {
			Destroy(m_container);
			m_container = null;
		}


		m_container = new GameObject();
		voxelObjs = new GameObject[numPointsX, numPointsY, numPointsZ];
		Transform ct = m_container.transform;
		ct.SetParent(transform, false);
		ct.localPosition = Vector3.zero;
		ct.localScale = Vector3.one;
		int i, j, k;

		// build voxels out of individual cubes
		//float x, y, z;
		//x = (numPointsX - 1) * pointGap / -2.0f;
		for (i = 0; i < numPointsX; i++) {
			//y = 0; //(numPointsY - 1) * pointGap / -2.0f;
			for (j = 0; j < numPointsY; j++) {
				//z = (numPointsZ - 1) * pointGap / -2.0f;
				for (k = 0; k < numPointsZ; k++) {
					if (voxels[i,j,k]) {
						GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
						setObjColor(cube, voxelColor(i, j, k));
						voxelObjs[i, j, k] = cube;
						Transform t = cube.transform;
						t.SetParent(ct, false);
						t.localScale = cubeScale;
						t.localPosition = posFromVoxel(i, j, k); //new Vector3(x, y, z);
					}
					//z += pointGap;
				}
				//y += pointGap;
			}
			//x += pointGap;
		}
	}

	public void RegisterSculptController(SculptGUIController c) {
		m_sculptController = c;
	}
}
