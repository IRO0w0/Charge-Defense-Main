using NUnit.Framework;
using UnityEngine;
using System; // 이벤트 사용을 위해 추가

public class PlayerMove : MonoBehaviour
{
    public float moveDistance = 1f; // 한 번 이동할 거리
    public float moveSpeed = 5f; // 이동 속도


    public static int playerHP = 100; // 플레이어의 HP

    private Vector3 targetPosition; // 이동할 목표 위치
    private Vector3 originalPosition; // 이동 전 원래 위치
    private bool isMoving = false; // 캐릭터가 이동 중인지 여부
                                   //private bool isDeath = false;

    public static event Action OnPlayerMoveComplete; // 플레이어 이동 완료 이벤트

    public static PlayerMove Instance { get; private set; } // 싱글톤 인스턴스

    [SerializeField] GameObject player;


    Animator animator;

    public static void TakeDamage(int damage)
    {
        playerHP -= damage;
        
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        targetPosition = transform.position; // 시작 위치를 현재 위치로 설정
        DieManager.Instance.RegisterPlayer(this); // GameManager에 플레이어 등록
    }

    void Update()
    {
        if (!isMoving) // 캐릭터가 이동 중이 아닐 때만 입력을 받음
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                Move(Vector3.forward);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                Move(Vector3.back);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                Move(Vector3.left);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                Move(Vector3.right);

            }

            if (playerHP < 1)
            {
                //GameManager.Instance.playerHP = 0;
                DieManager.Instance.Die();
            }
        }

        // 캐릭터를 목표 위치로 부드럽게 이동
        if (isMoving)
        {
            animator.SetBool("IsRun", true);



            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
            {
                if (!CheckCollisionAtTargetPosition())
                {
                    transform.position = targetPosition;
                    isMoving = false; // 이동 완료

                    // 이동 완료 시 이벤트 발생
                    OnPlayerMoveComplete?.Invoke();

                }
            }
        }
        else
        {
            animator.SetBool("IsRun", false);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            Block block = collision.gameObject.GetComponent<Block>();
            if (block != null)
            {
                block.HitByPlayer(); // 블록이 플레이어에 의해 밟혔음을 알림
            }
        }

        GameManager.Instance.HandleCollision(collision.gameObject); // 기타 충돌 처리 (총알 등)
    }

    void Move(Vector3 direction)
    {
        originalPosition = transform.position; // 이동 전 위치를 저장
        targetPosition = originalPosition + direction * moveDistance;

        transform.rotation = Quaternion.LookRotation(direction);

        if (!CheckCollisionAtTargetPosition())
        {
            isMoving = true;
            GameManager.Instance.IncrementTurn(); // 이동이 유효할 때만 턴 수 증가
        }
    }

    bool CheckCollisionAtTargetPosition()
    {
        // 이동하려는 위치에 Collider가 있는지 확인
        Collider[] colliders = Physics.OverlapSphere(targetPosition, 0.1f);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("TransparentBlock"))
            {
                return true; // TransparentBlock 태그가 있는 오브젝트와 충돌 시 true 반환
            }
        }
        return false; // 충돌이 없으면 false 반환
    }

}
