using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class horizScript1 : MonoBehaviour {

	public GameObject panel0;
	public GameObject panel1; // Central initial
	public GameObject panel2;

	private GameObject panelCentral;
	private GameObject panelLeft;
	private GameObject panelRight;

	private Vector3 _mouseDownPos;
	private Vector3 _panelDragStartPos;
	private Vector3 _panelDragLastPos;
	private bool _HorizontalOnly = true;

	private float _width;
	private int _iCurrentPage = 1;

	public int _iMinimumPage = 1;
	public int _iMaximumPage = 5;


	// Use this for initialization
	void Start () {
		// consider panel1 as the first middle element

		_width = panel1.GetComponent<RectTransform>().rect.width;
		panelCentral = panel1;
		panelLeft = panel0;
		panelRight = panel2;

		panelUpdatePositions ();
		textTest ();
	}

	private GameObject getChildGameObject(GameObject fromGameObject, string withName) {
		Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>();
		foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject;
		return null;
	}

	private void textTest()
	{
		Text p1 = getChildGameObject (panelLeft, "Text").GetComponent<Text>();
		Text p2 = getChildGameObject (panelCentral, "Text").GetComponent<Text>();
		Text p3 = getChildGameObject (panelRight, "Text").GetComponent<Text>();

		p1.text = "page "+ (_iCurrentPage-1);
		p2.text = "page "+ _iCurrentPage;
		p3.text = "page "+ (_iCurrentPage+1);

		panelLeft.SetActive (!(_iCurrentPage <= _iMinimumPage));
		panelRight.SetActive (!(_iCurrentPage >= _iMaximumPage));

	}

	private void panelUpdatePositions()
	{
		Vector3 panelPosLeft = panelLeft.transform.position;
		Vector3 panelPosRight = panelRight.transform.position;
		panelPosLeft.x = panelCentral.transform.position.x - _width;
		panelPosRight.x = panelCentral.transform.position.x + _width;
		panelLeft.transform.position = panelPosLeft;
		panelRight.transform.position = panelPosRight;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void pageLeft()
	{
		if (!(_iCurrentPage >= _iMaximumPage)) {
			_iCurrentPage++;
			GameObject newRight = panelLeft;
			panelLeft = panelCentral;
			panelCentral = panelRight;
			panelRight = newRight;
		}
	}

	private void pageRight()
	{
		if (!(_iCurrentPage <= _iMinimumPage)) {
			_iCurrentPage--;
			GameObject newLeft = panelRight;
			panelRight = panelCentral;
			panelCentral = panelLeft;
			panelLeft = newLeft;
		}
	}

	public void OnEndDrag(GameObject panel) {
		if (panel == panelCentral && (!_busy)) {
			Vector3 dist = Input.mousePosition - _panelDragLastPos;
			Debug.Log ("dist" + dist.x + " " + dist.y);
			if (dist.x < -5) {
				// swipe left
				pageLeft ();
			} else if (dist.x > 5) {
				// swipe right
				pageRight ();
			} else {
				// if passed threshold x pos
				float distDragged = panel.transform.position.x - _panelDragStartPos.x;
				if (distDragged < -(_width / 2)) { 
					// central panel been moved far enough to the left
					pageLeft ();
				} else if (distDragged > (_width / 2)) {
					// central panel been moved far enough to the right
					pageRight ();
				}
			}
				
			StartCoroutine (AnimatePanels ());
		}
	}

	public void OnBeginDrag(GameObject panel) {
		if (panel == panelCentral && (!_busy)) {
			_panelDragStartPos = panel.transform.position;
			_mouseDownPos = Input.mousePosition;
			_panelDragLastPos = Input.mousePosition;
		}
	}

	public void OnDrag(GameObject panel){ 
		if (panel == panelCentral && (!_busy)) {
			Vector3 delta = Input.mousePosition - _mouseDownPos;
			_panelDragLastPos = Input.mousePosition;
			if (_HorizontalOnly)
				delta.y = 0;

			// half movements if on the extremes
			if ((_iCurrentPage >= _iMaximumPage && delta.x < 0) ||
			    (_iCurrentPage <= _iMinimumPage && delta.x > 0)) {
				delta.x = delta.x / 2.0f;
			}

			panel.transform.position = _panelDragStartPos + delta; 

			panelUpdatePositions ();
		}
	}

	private bool _busy = false;
	IEnumerator AnimatePanels ()
	{
		if (!_busy) {
			_busy = true;
			Transform rt = panelCentral.transform;
			Vector3 startPos = rt.position;
			Vector3 endPos = _panelDragStartPos;

			rt.position = startPos;
			float time = 0.5f;

			float i = 0.0f;
			float rate = 1.0f / time;
			while (i < 1.0f) {
				i += Time.deltaTime * rate;

				float finalI = 1 - Mathf.Pow (1 - i, 4);
				rt.position = Vector3.Lerp (startPos, endPos, finalI);

				panelUpdatePositions ();

				yield return null; 
			}

			rt.position = endPos;
			textTest ();
		}
		_busy = false;
		yield return null; 
	}

}
