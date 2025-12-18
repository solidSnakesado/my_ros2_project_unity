using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;           // 이미지 메시지 타입

public class CameraPublisher : MonoBehaviour
{
    public  string          topicName           = "camera/image_raw";   // ROS 2에서 볼 주제 이름
    public  RenderTexture   robotViewTexture    = null;                 // 로봇 카메라에 지정된 랜더 텍스처 연결
    public  float           publushRate         = 10f;                  // 1초에 10장 전송

    private float           timeElapsed         = 0;
    private ROSConnection   ros                 = null;
    private Texture2D       tempTexture         = null;
    private Rect            rect                = Rect.zero;

    // Start is called before the first frame update
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<ImageMsg>(topicName);     // 이미지 메시지 등록

        // 텍스처 데이터를 읽기 위한 임시 변수들 초기화
        tempTexture = new Texture2D(robotViewTexture.width, robotViewTexture.height, TextureFormat.RGB24, false);
        rect = new Rect(0, 0, robotViewTexture.width, robotViewTexture.height);
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed > (1.0f / publushRate))
        {
            SendImage();
            timeElapsed = 0;
        }
    }

    private void SendImage()
    {
        // 1. 현재 렌더 텍스처를 활성화해서 데이터를 읽어옴
        RenderTexture.active = robotViewTexture;
        tempTexture.ReadPixels(rect, 0, 0);
        tempTexture.Apply();

        // 2. 이미지 데이터를 바이트 배열로 변환
        byte[] rawData = tempTexture.GetRawTextureData();

        // 3. ROS 메시지 생성 (이미지 포맷 설정)
        ImageMsg imgMsg = new ImageMsg();
        imgMsg.header.frame_id = "camera_link";
        imgMsg.height = (uint)robotViewTexture.height;
        imgMsg.width = (uint)robotViewTexture.width;
        imgMsg.encoding = "rgb8";                           // 픽셀 포맷
        imgMsg.step = (uint)(robotViewTexture.width * 3);   // 한줄의 바이트 수 (RGB는 3바이트 여서 * 3)
        imgMsg.data = rawData;

        // 4. 메시지 발송!
        ros.Publish(topicName, imgMsg);
    }
}
