using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ProfileImages.asset", menuName = "Data/ProfileImages", order = 4)]
public class ProfileImages : ScriptableObject
{
    public List<Texture> textures;
    public List<Sprite> sprites;
}