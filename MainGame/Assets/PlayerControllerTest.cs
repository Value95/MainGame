using UnityEngine;
using FishNet.Object;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 180f; // 초당 회전 각도

    [SerializeField] private GameObject cubePrefab;

    
    private void Update()
    {
        
        // 내가 소유한 캐릭터만 조작
        if (!IsOwner) return;

        
        // // 이동 입력
        float move = Input.GetAxis("Vertical"); // W/S
        float strafe = Input.GetAxis("Horizontal"); // A/D
        Vector3 moveDir = new Vector3(strafe, 0, move);
        transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.Self);
        
        // 회전 (화살표 좌/우)
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("QQQ");
            SpawnCubeServerRpc();
        }
    }
    
    [ServerRpc]
    private void SpawnCubeServerRpc()
    {
        Debug.Log("SpawnCubeServerRpc");
        GameObject cube = Instantiate(cubePrefab, transform.position + Vector3.forward, Quaternion.identity);
        base.Spawn(cube); // FishNet의 Spawn
    }
}
