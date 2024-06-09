using UnityEngine;

//Handles mouse look rotation using mouse
namespace _6v6Shooter.Scripts.Gameplay.Player
{
    [System.Serializable]
    public class MouseLook {

        //Mouse sensitivity
        public float xSens = 2f;
        public float ySens = 2f;
    
        //Vertical rotation
        public bool yClampRot = true;
        public Vector2 yClampRange = new(-90f, 90f);
    
        //Smoothing
        public bool smooth;
        public float smoothTime = 2f;

        //Transforms and rotations
        private Transform _tCamera;
        private Transform _tCharacter;
        private Quaternion _qCharacter;
        private Quaternion _qCamera;
    
        //Cursor
        private bool _cursorLocked = true;
    
        public void Init(Transform character, Transform camera)
        {
            _tCharacter = character;
            _tCamera = camera;
            _qCharacter = character.localRotation;
            _qCamera= camera.localRotation;
        
            //Sets proper cursor state
            SetCursorState();
        }
    
        public void LookRotation()
        {
            _qCharacter *= Quaternion.Euler(0f, Input.GetAxis("Mouse X") * xSens, 0f);
            _qCamera *= Quaternion.Euler(-Input.GetAxis("Mouse Y") * ySens, 0f, 0f);

            //Clamp vertical rotation if enabled
            if (yClampRot)
                _qCamera = ClampRotation(_qCamera);
        
            if (smooth)
            {
                //Mouse rotation smoothing if enabled
                _tCharacter.localRotation = Quaternion.Lerp(_tCharacter.localRotation, _qCharacter,
                    smoothTime * Time.deltaTime);
                _tCamera.localRotation = Quaternion.Lerp(_tCamera.localRotation, _qCamera,
                    smoothTime * Time.deltaTime);
            }
            else
            {
                _tCharacter.localRotation = _qCharacter;
                _tCamera.localRotation = _qCamera;
            }
        }
    
        public void SetCursorState()
        {
            if (_cursorLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    
        //Clamps rotation on axis
        private Quaternion ClampRotation(Quaternion rot)
        {
            //Normalize rotation
            rot.x /= rot.w;
            rot.y /= rot.w;
            rot.z /= rot.w;
            rot.w = 1.0f;

            //To degrees, clamp and return to radians
            var a = 2.0f * Mathf.Rad2Deg * Mathf.Atan(rot.x);
            a = Mathf.Clamp(a, yClampRange[0], yClampRange[1]);
            rot.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * a);
        
            return rot;
        }
    }
}