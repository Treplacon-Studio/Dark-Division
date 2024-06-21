using UnityEngine;


//Handles horizontal mouse look rotation using mouse 
[System.Serializable]
public class MouseLookAround
{
    //Mouse sensitivity
    public float sensitivity = 2f;

    //Smoothing
    public bool smooth;
    public float smoothTime = 2f;

    //Transforms and rotations
    private Transform _tCharacter;
    private Quaternion _qCharacter;

    public void Init(Transform character)
    {
        _tCharacter = character;
        _qCharacter = character.localRotation;
    }

    public void LookRotation()
    {
        _qCharacter *= Quaternion.Euler(0f, Input.GetAxis("Mouse X") * sensitivity, 0f);

        if (smooth)
            _tCharacter.localRotation = Quaternion.Lerp(_tCharacter.localRotation, _qCharacter,
                smoothTime * Time.deltaTime);
        else
            _tCharacter.localRotation = _qCharacter;
    }
}