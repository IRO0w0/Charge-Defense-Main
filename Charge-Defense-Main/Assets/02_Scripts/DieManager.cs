using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class DieManager : MonoBehaviour
{
    public static DieManager Instance { get; private set; } // 싱글톤 인스턴스

    private PlayerMove player; // PlayerMove 스크립트 참조
    private bool isFirstDeath = true; // 첫 사망 여부를 추적

    public RawImage[] deathImages; // 사망 시 표시할 이미지 배열
    public Canvas deathCanvas; // 이미지들을 표시할 캔버스
    public float fadeDuration = 0.5f; // 페이드 효과의 지속 시간 (0.5초로 설정)
    public float respawnDelay = 3.0f; // 부활 지연 시간 (3초로 설정)

    private void Awake()
    {
        // 싱글톤 인스턴스 초기화
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 전환되어도 이 객체가 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 다른 인스턴스가 존재하면 파괴
        }
    }
    public void RegisterPlayer(PlayerMove player)
    {
        this.player = player;
    }

    public void Die()
    {
        Debug.Log("플레이어가 사망했습니다.");

        if (player != null)
        {
            if (isFirstDeath)
            {
                StartCoroutine(ShowDeathImagesAndRespawn());
            }
            else
            {
                HandleDeath(); // 첫 사망이 아닐 경우 즉시 처리
                spawn.Instance.Respawn(); // 부활 처리
                GameManager.Instance.ResetTurn(); // 턴 수 리셋
            }
        }
    }



    IEnumerator ShowDeathImagesAndRespawn()
    {
        isFirstDeath = false; // 첫 사망 이후 다시 실행되지 않도록 설정

        // 캔버스를 활성화하여 이미지를 표시할 준비
        deathCanvas.gameObject.SetActive(true);

        // 모든 이미지를 비활성화
        foreach (RawImage img in deathImages)
        {
            img.gameObject.SetActive(false);
        }

        // 이미지 순차적으로 표시
        for (int i = 0; i < deathImages.Length; i++)
        {
            RawImage currentImage = deathImages[i];
            currentImage.gameObject.SetActive(true);
            currentImage.canvasRenderer.SetAlpha(0f); // 투명하게 설정
            StartCoroutine(FadeIn(currentImage)); // 페이드 인 시작

            // 페이드 인 후 잠시 대기
            yield return new WaitForSeconds(fadeDuration);

            // 페이드 아웃을 위해 잠시 대기
            yield return new WaitForSeconds(1f - fadeDuration);

            StartCoroutine(FadeOut(currentImage)); // 페이드 아웃 시작
            yield return new WaitForSeconds(fadeDuration); // 페이드 아웃 완료 대기

            currentImage.gameObject.SetActive(false); // 이미지 비활성화
        }

        // 모든 이미지 표시가 끝나면 캔버스를 비활성화하고 부활 처리
        deathCanvas.gameObject.SetActive(false);
        HandleDeath(); // 플레이어 오브젝트 제거
        yield return new WaitForSeconds(respawnDelay); // 부활 지연 시간 대기
        spawn.Instance.Respawn(); // 플레이어 부활
        GameManager.Instance.ResetTurn(); // 턴 수 초기화
    }

    IEnumerator FadeIn(RawImage image)
    {
        image.CrossFadeAlpha(1f, fadeDuration, false);
        yield return new WaitForSeconds(fadeDuration);
    }

    IEnumerator FadeOut(RawImage image)
    {
        image.CrossFadeAlpha(0f, fadeDuration, false);
        yield return new WaitForSeconds(fadeDuration);
    }

    public void HandleDeath()
    {
        // 플레이어 오브젝트 제거
        if (player != null)
        {
            Destroy(player.gameObject); // 플레이어 오브젝트 제거
        }
    }
}
