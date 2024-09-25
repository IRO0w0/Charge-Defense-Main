using System;
using UnityEngine;

public class spawn : MonoBehaviour
{
    public static spawn Instance;  // 싱글톤 인스턴스

    public static Vector3 respawnPoint; // 부활 위치

    public GameObject Player;

    public int maxHealth = 100;  // 최대 체력
//private int currentHealth;   // 현재 체력
    void Awake()
    {
        // 싱글톤 인스턴스 할당
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 이 오브젝트를 씬 전환 시 파괴하지 않음
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 있으면 자신을 파괴
        }
    }
    void Start()
    {
        PlayerMove.playerHP = maxHealth;  // 시작할 때 체력을 100으로 설정
        respawnPoint = transform.position; // 현재 위치를 부활 지점으로 설정
    }
    // 플레이어를 리스폰 위치로 이동시키는 함수
    public void Respawn()
    {
        //Instantiate(Player, respawnPoint, Quaternion.identity);
        PlayerMove.playerHP = maxHealth;  // 체력을 100으로 복구
        Debug.Log("Player Respawned!");
    }
}
