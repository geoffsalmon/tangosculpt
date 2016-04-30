using UnityEngine;
using System.Collections;

public class RealVoxelsSculptControl : MonoBehaviour {

	//public Vector3 numPoints = new Vector3(8, 10, 8);
	public int numPointsX = 8;
	public int numPointsY = 10;
	public int numPointsZ = 8;
	public float pointGap = 0.02f;


	bool[,,] voxels;
	GameObject[,,] voxelObjs;

	private GameObject m_selectObject;

	private bool m_voxelsDirty = true;

	private GameObject m_container;

	// Use this for initialization
	void Start () {
		voxels = new bool[numPointsX, numPointsY, numPointsZ];
		int i, j, k;
		for (i = 0; i < numPointsX; i++) {
			for (j = 0; j < numPointsY; j++) {
				for (k = 0; k < numPointsZ; k++) {
					if (Random.Range(0, 10) < 6) {
						voxels[i, j, k] = true;
					}
				}
			}
		}

		Debug.Log("Located at " + transform.position);
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
			if (obj != m_selectObject) {
				if (m_selectObject != null) {
					setObjColor(m_selectObject, Color.white);
				}
				m_selectObject = obj;
				setObjColor(m_selectObject, Color.red);
			}

			// remove selected
			if (m_selectObject != null) {
				Destroy(m_selectObject);
				m_selectObject = null; 
			}
			//Debug.Log("Hit " + hit.point);
			//transform.position = ray.GetPoint(100.0F);
			//m_lastHit = hit.point;
			//m_select.SetActive(true);
			//m_select.transform.position = m_lastHit;
			//} else {
			//Debug.Log("Miss");
		} else {
			if (m_selectObject != null) {
				setObjColor(m_selectObject, Color.white);
			}
			m_selectObject = null;
		}
		//}
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
		Debug.Log("I am at " + transform.position + " local " + transform.localPosition);
		Debug.Log("New container at position " + m_container.transform.position + " local " + m_container.transform.localPosition);
		Debug.Log("Parent is correct? " + (transform == ct.parent));
		Vector3 cubeScale = Vector3.one * 0.9f * pointGap;
		int i, j, k;

		// build voxels out of individual cubes
		float x, y, z;
		x = (numPointsX - 1) * pointGap / -2.0f;
		for (i = 0; i < numPointsX; i++) {
			y = 0; //(numPointsY - 1) * pointGap / -2.0f;
			for (j = 0; j < numPointsY; j++) {
				z = (numPointsZ - 1) * pointGap / -2.0f;
				for (k = 0; k < numPointsZ; k++) {
					if (voxels[i,j,k]) {
						GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
						voxelObjs[i, j, k] = cube;
						Transform t = cube.transform;
						t.SetParent(ct, false);
						t.localScale = cubeScale;
						t.localPosition = new Vector3(x, y, z);
					}
					z += pointGap;
				}
				y += pointGap;
			}
			x += pointGap;
		}
	}
}
