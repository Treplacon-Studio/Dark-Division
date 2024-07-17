using System;
using UnityEngine;

public class AnimationClipsHolder : MonoBehaviour
{
   //Weapon index is defined by int representation of Weapons.cs -> WeaponName enum
   public MovementLayerAnimations[] movementAnimations;
   public BaseWeaponAnimations[] baseWeaponAnimations;
}

[Serializable]
public class MovementLayerAnimations
{
   public AnimationClip idle;
   public AnimationClip walk;
   public AnimationClip run;
   public AnimationClip jump;
}

[Serializable]
public class BaseWeaponAnimations
{
   public AnimationClip inspect;
   public AnimationClip reload;
   public AnimationClip ads;
   public AnimationClip shootAds;
   public AnimationClip shootHfr;
}

