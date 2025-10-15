using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class PlayPauseVideo : MonoBehaviour
{
    public VideoClip[] videoclips;
    public Button playButton; // 播放按鈕
    public Button pauseButton; // 暫停按鈕
    public Button nextButton;  // 下一個按鈕
    public VideoPlayer videoPlayer; // Video Player
    private bool isPaused = true; // 標記影片是否被暫停

    private int videoClipsIndex; //影片索引
    public AudioSource audioSource;
    public AudioClip[] audioClip;
    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
    }
    void Start()
    {
        videoPlayer.clip = videoclips[0];

        // 監聽播放按鈕的點擊事件
        playButton.onClick.AddListener(PlayVideo);

        // 監聽暫停按鈕的點擊事件
        pauseButton.onClick.AddListener(PauseVideo);

        nextButton.onClick.AddListener(NextVideo);
    }

    void PlayVideo()
    {
        if (isPaused)
        {
            videoPlayer.Play(); // 如果影片被暫停，則開始播放
            isPaused = false; // 更新狀態為非暫停
            audioSource.PlayOneShot(audioClip[0]);
        }
    }

    void PauseVideo()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause(); // 如果影片正在播放，則暫停它
            isPaused = true; // 更新狀態為暫停
            audioSource.PlayOneShot(audioClip[1]);
        }
    }

    void NextVideo()
    {
        PauseVideo();
        videoClipsIndex++;
        if(videoClipsIndex>=videoclips.Length)
        {
            videoClipsIndex = videoClipsIndex % videoclips.Length;
        }
        videoPlayer.clip = videoclips[videoClipsIndex];
        audioSource.PlayOneShot(audioClip[2]);
    }
}
