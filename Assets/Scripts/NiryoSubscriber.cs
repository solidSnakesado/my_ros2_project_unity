using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Trajectory;       // ROS 메시지 타입 사용

public class NiryoSubscriber : MonoBehaviour
{
    // ROS와 통신할 주체(Topic) 이름
    public  string              topicName   = "set_joint_trajectory";

    // 로봇의 관절들 (상위에서 차례대로)
    public  ArticulationBody[]  joints      = null;
    // Start is called before the first frame update
    void Start()
    {
        // 1. ROS 연결 담당자(Connector)
        ROSConnection ros = ROSConnection.GetOrCreateInstance();

        // 2. 특정 주제(Topic)로 들어오는 메시지를 구독(Subscribe)
        ros.Subscribe<JointTrajectoryMsg>(topicName, MoveJoints);
    }

    private void MoveJoints(JointTrajectoryMsg msg)
    {
        // 메시지에 담긴 관절 목표 위치(points)가 있는 지 확인
        if (msg.points.Length > 0)
        {
            // 첫 번째 목표 지점의 데이터
            JointTrajectoryPointMsg point = msg.points[0];

            // 데이터 개수와 관절 개수가 맞는지 확인
            if (point.positions.Length != joints.Length)
            {
                Debug.LogError($"개수 불일치! 관절: {joints.Length}, 데이터: {point.positions.Length}");
                return;
            }

            // 로봇의 각 관절을 순회하며 목표 각도를 움직임
            for (int i = 0; i < joints.Length; ++i)
            {
                // ROS는 라디안(Radian) 단위를 쓰지만 Unity는 도(Degree)를 쓰기 때문에
                // 변환이 필요
                float targetAngle = (float)point.positions[i] * Mathf.Rad2Deg;

                // 로그 추가: 각 관절에 들어가는 목표 값 확인
                Debug.Log($"Joint[{i}] Target: {targetAngle}");

                // 관절을 움직임
                ArticulationDrive drive = joints[i].xDrive;
                drive.target = targetAngle;
                joints[i].xDrive = drive;
            }
        }
    }
}
