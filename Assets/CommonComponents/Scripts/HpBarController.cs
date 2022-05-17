using UnityEngine;
using UnityEngine.UI;

namespace SimpleVoxelTanks.CommonComponents
{
    [RequireComponent(typeof(Canvas))]
    public sealed class HpBarController : MonoBehaviour
    {
        [SerializeField]
        private Camera _camera;

        [SerializeField]
        private Canvas _canvas;

        [SerializeField]
        private Image _image;

        private Transform _transform;

        public Camera Camera
        {
            get => _camera;
            set
            {
                _camera = value;
                _canvas.worldCamera = _camera;
            }
        }

        public float Health { get => _image.fillAmount; set => _image.fillAmount = value; }

        private void Awake ()
        {
            _transform = transform;
            _canvas ??= GetComponent<Canvas>();
        }

        private void Update ()
        {
            if (Camera != null)
            {
                var postion = _transform.parent.forward + _transform.parent.position;
                postion.y = Camera.transform.position.y;
                if (postion.z > Camera.transform.position.z)
                    postion.z *= -1;
                _transform.LookAt(postion);
            }
        }
    }
}