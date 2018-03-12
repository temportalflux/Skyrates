using UnityEngine;


namespace Skyrates.Client.Entity
{
	public class CanvasWorldSpaceToScreenSpace : MonoBehaviour
	{
		//TODO: doxygen
		private RectTransform _rectTransform;
		public Transform Target;
		public Vector3 Offset;

		void Awake()
		{
			_rectTransform = GetComponent<RectTransform>();
		}

		void Start()
		{
			//TODO: DRY, put this in a function instead.
			_rectTransform.anchoredPosition = Camera.main.WorldToScreenPoint(Target.transform.position + Offset) - new Vector3(Screen.width / 2.0f, Screen.height / 2.0f); //For middle-aligned text.
		}


		void Update()
		{
			//TODO: DRY, put this in a function instead.
			_rectTransform.anchoredPosition = Camera.main.WorldToScreenPoint(Target.transform.position + Offset) - new Vector3(Screen.width / 2.0f, Screen.height / 2.0f); //For middle-aligned text.
		}
	}
}