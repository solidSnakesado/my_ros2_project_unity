using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;

public class JointStatePublisher : MonoBehaviour
{
    // ROS Topic 이름 (표준 이름인 "joint_states" 사용)
    public string topicName = "joint_states";

    // 로봇의 관절들 (NiryoSubscriber에 넣은 것과 똑같은 순서로 넣으세요!)
    public ArticulationBody[] joints;

    // 관절 이름 (ROS에서 식별할 이름, 예: joint_1, joint_2...)
    public string[] jointNames = new string[]
    {
        "joint_1", "joint_2", "joint_3", "joint_4", "joint_5", "joint_6"
    };

    public float publishRate = 10f; // 1초에 10번 전송

    private ROSConnection ros;
    private float timeElapsed;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<JointStateMsg>(topicName);
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed > (1.0f / publishRate))
        {
            PublishJointState();
            timeElapsed = 0;
        }
    }

    void PublishJointState()
    {
        JointStateMsg msg = new JointStateMsg();

        // 헤더 설정
        msg.header = new HeaderMsg();
        msg.header.frame_id = "base_link";

        // 관절 이름 설정
        msg.name = jointNames;

        // 관절 각도 설정 (Unity Degree -> ROS Radian 변환 필수!)
        double[] positions = new double[joints.Length];
        for (int i = 0; i < joints.Length; i++)
        {
            // Unity의 xDrive.target은 목표값이므로, 현재 실제 각도인 jointPosition[0]을 사용
            // ArticulationBody는 라디안으로 값을 리턴하므로 그대로 사용하거나,
            // Inspector값과 맞추려면 변환 확인 필요. 보통 xDrive.target은 Degree입니다.
            // 여기서는 가장 확실한 xDrive.target(현재 명령값)을 Degree -> Radian으로 변환해 보냅니다.

            positions[i] = joints[i].xDrive.target * Mathf.Deg2Rad;
        }

        msg.position = positions;

        // 메시지 발송
        ros.Publish(topicName, msg);
    }
}