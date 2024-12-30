using UnityEngine;

public class ScrollingObject : MonoBehaviour
{
    [SerializeField] MeshRenderer Sky;
    [SerializeField] MeshRenderer Buildings;
    [SerializeField] MeshRenderer Ground;

    [SerializeField] float skySpeed;
    [SerializeField] float buildingSpeed;
    [SerializeField] float groundSpeed;

    private void Start()
    {
        Global.IsPlayerMoveRight -= CalScrollingSpeed;
        Global.IsPlayerMoveRight += CalScrollingSpeed;
    }

    private void CalScrollingSpeed(int side)
    {
        float sky = skySpeed;
        if (side == -1 || side == 1)
            sky = skySpeed + (skySpeed / 2 * side);

        Scrolling(sky, buildingSpeed * side, groundSpeed * side);
    }

    private void Scrolling(float sky, float buildings, float ground)
    {
        Sky.material.mainTextureOffset += new Vector2(sky * Time.deltaTime, 0);
        Buildings.material.mainTextureOffset += new Vector2(buildings * Time.deltaTime, 0);
        Ground.material.mainTextureOffset += new Vector2(ground * Time.deltaTime, 0);
    }
}