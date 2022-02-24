 using UnityEngine;
 using UnityEngine.PlayerLoop;
 public class InfiniteRotation : MonoBehaviour 
 {
     private float _speed = 5.0f;

     private void FixedUpdate()
     {
         transform.Rotate(0, _speed * Time.fixedDeltaTime, 0);
     }

     public void SetSpeed(float value)
     {
         _speed = value;
     }
 }
