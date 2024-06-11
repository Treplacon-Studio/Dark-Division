using UnityEngine;

//Handles mouse look rotation using mouse
namespace _6v6Shooter.Scripts.Gameplay.Player
{
    [System.Serializable]
    public class MouseLookAround {

        //Mouse sensitivity
        public float sensitivity = 2f;
    
        //Smoothing
        public bool smooth;
        public float smoothTime = 2f;

        //Transforms and rotations
        private Transform _tCharacter;
        private Quaternion _qCharacter;
    
        //Cursor
        private bool _cursorLocked = true;
    
        public void Init(Transform character)
        {
            _tCharacter = character;
            _qCharacter = character.localRotation;
        
            //Sets proper cursor state
            SetCursorState();
        }
    
        public void LookRotation()
        {
            _qCharacter *= Quaternion.Euler(0f, Input.GetAxis("Mouse X") * sensitivity, 0f);
        
            if (smooth)
            {
                //Mouse rotation smoothing if enabled
                _tCharacter.localRotation = Quaternion.Lerp(_tCharacter.localRotation, _qCharacter,
                    smoothTime * Time.deltaTime);
            }
            else
            {
                _tCharacter.localRotation = _qCharacter;
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
    }
}