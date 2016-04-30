using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
	public Transform target;
	public float distance = 5.0f;

	public float xSpeed = 50.0f;
	public float ySpeed = 90.0f;

	public float yMinLimit = 0.0f;
	public float yMaxLimit = 80f;

	public float speed = 0.14f;

	private float x = 0f;
	private float y = 0f;

	//Vector3 origin;

	// Use this for initialization
	void Start () {
		//target = gameObject.transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (target && Input.GetMouseButton(0)) {
			x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
			y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

			//y = ClampAngle(y, yMinLimit, yMaxLimit);
			y = -25f;

			Quaternion rotation = Quaternion.Euler(y, x, 0);
			Vector3 position = (rotation * new Vector3(0.0f, 0.0f, distance)) + target.position;


			transform.position = position;
			transform.LookAt(target);
		}
	}
}
