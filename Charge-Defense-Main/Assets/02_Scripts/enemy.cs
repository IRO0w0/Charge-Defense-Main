using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    RaycastHit hit;
    float MaxDistance = 30; // 레이 범위
    private Vector3 playerLastPosition; // 플레이어의 이전 위치 저장
    public GameObject player; // 플레이어 오브젝트
    public float moveDistance = 2f; // 적 이동 거리
    public float movementThreshold = 0.1f; // 플레이어 이동 감지 임계치
    private bool playerInSight = false; // 플레이어가 레이 범위 안에 있는지 여부


    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        // 플레이어의 초기 위치 저장
        playerLastPosition = player.transform.position;
        // 플레이어 이동 완료 이벤트 구독
        PlayerMove.OnPlayerMoveComplete += MoveEnemyOnPlayerMove;
    }

    void OnDestroy()
    {
        // 이벤트 구독 해제
        PlayerMove.OnPlayerMoveComplete -= MoveEnemyOnPlayerMove;
    }

    private void MoveEnemyOnPlayerMove()
    {
        // 플레이어가 레이 범위 안에 있을 때만 적이 이동
        if (playerInSight)
        {
            animator.SetBool("IsRun", true);
            // 적이 X축으로 2만큼 이동
            Vector3 direction = Vector3.right;
            transform.position += direction * moveDistance;
        }
    }


    void Update()
    {
        // 적의 레이를 시각화
        Debug.DrawRay(transform.position, transform.forward * MaxDistance, Color.blue, 0.3f);

        // 레이캐스트로 플레이어를 감지
        if (Physics.Raycast(transform.position, transform.forward, out hit, MaxDistance))
        {
            if (hit.collider.gameObject == player)
            {
                // 플레이어가 레이 범위 안에 있음
                playerInSight = true;

                // 플레이어가 일정 거리 이상으로 움직였을 때만 적이 이동
                Vector3 playerCurrentPosition = player.transform.position;
                if (Vector3.Distance(playerCurrentPosition, playerLastPosition) > movementThreshold)
                {
                    Debug.Log("Player is moving in range!");
                    // 이동 완료 시 플레이어의 위치 업데이트
                    playerLastPosition = playerCurrentPosition;
                }
            }
            else
            {
                playerInSight = false; // 플레이어가 레이 범위를 벗어남
            }
        }
        else
        {
            playerInSight = false; // 레이 범위 내에 아무것도 감지되지 않음
        }
    }
}
