using UnityEngine;
using System.Collections;

public class VoxelSculptControl : MonoBehaviour {
	public const float UI_LABEL_START_X = 15.0f;
	public const float UI_LABEL_START_Y = 600.0f;
	public const float UI_LABEL_SIZE_X = 1920.0f;
	public const float UI_LABEL_SIZE_Y = 35.0f;
	public const float UI_LABEL_GAP_Y = 3.0f;
	public const string UI_FONT_SIZE = "<size=25>";

	private BoxCollider m_collider;
	// Use this for initialization

	/*public int numXPoints = 4;
	public int numYPoints = 10;
	public int numZPoints = 4;*/

	public Vector3 numPoints = new Vector3(8, 10, 8);

	public float pointGap = 0.02f;

	private Vector3 m_lastHit = new Vector3(0, 0, 0);

	private GameObject m_select;

	void Start () {
		m_collider = gameObject.AddComponent<BoxCollider>();
		m_collider.size = (numPoints - new Vector3(1, 1, 1)) * pointGap;
		m_collider.center = new Vector3(0, m_collider.size.y / 2, 0);
	
		// reference cube
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.transform.parent = transform;
		cube.transform.localScale = m_collider.size;
		cube.transform.localPosition = m_collider.center;


		m_select = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		m_select.transform.localScale = (Vector3.one * pointGap);
		m_select.transform.parent = transform;

		MeshRenderer renderer = m_select.GetComponent<MeshRenderer>();
		renderer.material.color = Color.red;
		m_select.SetActive(false);

		Debug.Log("sculpt start cube " + cube.transform.localScale + " " + cube.transform.position + " " + cube.transform.localPosition);


		// create mesh
		/*GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.transform.localScale = new Vector3(0.2f, 0.6f, 0.3f);
		cube.transform.localPosition = new Vector3(0, 0.3f, 0);
		cube.transform.parent = transform;*/

		/*GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube2.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		cube2.transform.localPosition = new Vector3(0, 0.8f, 0);
		cube2.transform.parent = transform;*/


	}
	
	// Update is called once per frame
	void Update () {
		if (Input.touchCount != 1) {
			return;
		}
			
		Touch t = Input.GetTouch(0);
		//if (t.phase == TouchPhase.Began || t.phase == TouchPhase.Moved || t.phase == TouchPhase.) {
			
			Ray ray = Camera.main.ScreenPointToRay(t.position);
			RaycastHit hit;
			if (m_collider.Raycast(ray, out hit, 100.0F)) {
				//Debug.Log("Hit " + hit.point);
				//transform.position = ray.GetPoint(100.0F);
				m_lastHit = hit.point;
				m_select.SetActive(true);
				m_select.transform.position = m_lastHit;
			//} else {
//Debug.Log("Miss");
			}
		//}
	}

	public void OnGUI() {

			GUI.Label(new Rect(UI_LABEL_START_X,
				UI_LABEL_START_Y,
				UI_LABEL_SIZE_X,
				UI_LABEL_SIZE_Y),
				UI_FONT_SIZE + m_lastHit.ToString() + "</size>");

		GUI.Label(new Rect(UI_LABEL_START_X,
			UI_LABEL_START_Y+(UI_LABEL_SIZE_Y+UI_LABEL_GAP_Y),
			UI_LABEL_SIZE_X,
			UI_LABEL_SIZE_Y),
			UI_FONT_SIZE + transform.position + "</size>");
	}
}
