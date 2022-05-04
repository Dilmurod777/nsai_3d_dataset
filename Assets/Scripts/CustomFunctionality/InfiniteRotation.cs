using UnityEngine;

namespace CustomFunctionality
{
    public class InfiniteRotation : MonoBehaviour 
    {
        private float _speed = 5.0f;
        private Vector3 _direction = new Vector3(0, 1, 0);

        private void FixedUpdate()
        {
            var delta = _speed * Time.fixedDeltaTime;
            transform.Rotate(_direction.x * delta, _direction.y * delta , _direction.z * delta);
        }

        public void SetSpeed(float speed)
        {
            _speed = speed;
        }

        public void SetDirection(Vector3 direction)
        {
            _direction = direction;
        }
    }
}
