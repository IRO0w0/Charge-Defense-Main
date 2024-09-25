using UnityEngine;

public class StageManager : MonoBehaviour
{
    public GameObject character;        // 플레이어 오브젝트
    public Camera mainCamera;           // 메인 카메라
    public Vector3 offset;              // 카메라의 플레이어와의 거리 오프셋
    private bool shouldFollow = false;  // 카메라가 따라와야 하는지 여부
    private bool hasFollowed = false;   // 카메라가 이미 따라왔는지 여부를 추적



    public GameObject obstaclesGroup;   // 제거할 오브젝트 그룹

    void Start()
    {
        // 초기 카메라 위치 설정
        offset = mainCamera.transform.position - character.transform.position;
    }

    private void FixedUpdate()
    {
        if (shouldFollow && !hasFollowed)
        {
            Vector3 currentCameraPosition = mainCamera.transform.position;

            Vector3 targetPosition = new Vector3(character.transform.position.x+ 7.189f + offset.x, currentCameraPosition.y, currentCameraPosition.z);

            mainCamera.transform.position = Vector3.MoveTowards(currentCameraPosition, targetPosition, 15 * Time.deltaTime);

            // 턴 수 초기화
            // 카메라가 목표 위치에 거의 도달했는지 확인
            Debug.Log("Distance to target: " + Vector3.Distance(mainCamera.transform.position, targetPosition));
            if (Vector3.Distance(mainCamera.transform.position, targetPosition) < 9)
            {
                // 턴 초기화를 한 번만 실행하고, 이후 더 이상 실행하지 않도록 설정
                GameManager.Instance.ResetTurn();
                hasFollowed = true;               // 한 번만 따라오도록 설정
                shouldFollow = false;             // 카메라 추적 중지

            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == character && !hasFollowed)
        {
            // 플레이어가 트리거 영역에 들어가면 한 번만 따라오도록 설정
            shouldFollow = true;

            // 현재 위치를 체크포인트로 설정
            //GameManager.Instance.SetSpawnPosition(character.transform.position);
            //Debug.Log("뉴스폰");
            // 그룹 전체를 제거
            // 충돌한 오브젝트의 위치를 새로운 리스폰 포인트로 설정
            spawn.respawnPoint = other.transform.position;
            Debug.Log("Respawn point updated!");
            Destroy(obstaclesGroup);
           // GetComponent<Collider>().enabled = false;
        }
    }

}
