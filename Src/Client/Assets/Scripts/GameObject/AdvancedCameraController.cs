using UnityEngine;
using System.Collections;

/// <summary>
/// 高级相机控制器：支持自由、锁定（攻击）、特写/剧情、瞬移状态，并实现状态融合、双击校正、屏幕震动及碰撞检测。
/// 参考《战双》和《师傅》的相机设计思路，不依赖第三方插件。
/// </summary>
public class AdvancedCameraController : MonoSingleton<AdvancedCameraController>
{
    public enum CameraState { Free, LockOn, Attack, Cinematic, Teleport }
    public CameraState currentState = CameraState.Free;

    [Header("目标设置")]
    public Transform playerTarget;         // 玩家角色
    public Transform lockOnTarget;         // 锁定目标（战斗时使用）
    public Transform cinematicTarget;      // 剧情/特写预设目标

    [Header("自由状态设置")]
    public float freeDistance = 6.0f;
    public float freeHeight = 2.5f;
    public float freeFollowSpeed = 5.0f;
    public float freeRotationSpeed = 200f;
    public float freeVerticalMin = -30f;
    public float freeVerticalMax = 60f;

    [Header("锁定/攻击状态设置")]
    public float lockOnDistance = 7.0f;
    public float lockOnHeight = 3.0f;
    public float lockOnFollowSpeed = 5.0f;
    // 攻击状态参数可与锁定状态稍有差异
    public float attackDistance = 6.0f;
    public float attackHeight = 2.8f;
    public float attackFollowSpeed = 7.0f;

    [Header("剧情状态设置")]
    public float cinematicBlendTime = 1.0f;  // 过场状态下的插值时间

    [Header("碰撞检测")]
    public float collisionRadius = 0.3f;
    public float collisionOffset = 0.5f;

    [Header("状态切换控制")]
    public float movementHoldThreshold = 1.0f;  // 按住移动键超过 1 秒切换为自由状态

    [Header("震动效果")]
    // 调用 ShakeCamera(duration, magnitude) 可触发震动

    // 内部状态变量
    private float yaw, pitch;
    private Vector2 lastMousePosition;
    private bool isDragging = false;
    private float dragThreshold = 10f;
    private float lastTapTime = 0f;
    private float doubleTapThreshold = 0.3f;
    private float movementHoldTime = 0f;

    // 状态过渡变量
    private float transitionTime = 0f;
    private float transitionDuration = 0f;
    private CameraState previousState;
    private bool isTransitioning = false;

    // 摄像机震动偏移
    private Vector3 shakeOffset = Vector3.zero;

    // 上一帧玩家位置，用于检测瞬移
    private Vector3 lastPlayerPos;

    void Start()
    {
        if (playerTarget == null)
        {
            Debug.LogError("AdvancedCameraController：未设置 playerTarget！");
            enabled = false;
            return;
        }
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
        lastMousePosition = Input.mousePosition;
        lastPlayerPos = playerTarget.position;
    }

    void LateUpdate()
    {
        HandleInput();
        CheckState();
        UpdateTransition();

        // 根据当前状态计算理想位置和旋转
        Vector3 desiredPosition;
        Quaternion desiredRotation;
        ComputeDesiredTransform(out desiredPosition, out desiredRotation);

        // 防止穿墙
        desiredPosition = AdjustForCollision(desiredPosition, playerTarget.position + Vector3.up * freeHeight);

        // 状态切换/融合：瞬移状态直接重置，其它状态用插值平滑过渡
        if (currentState == CameraState.Teleport)
        {
            transform.position = desiredPosition;
            transform.rotation = desiredRotation;
        }
        else if (isTransitioning)
        {
            // 过渡期间采用插值融合
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime / transitionDuration);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime / transitionDuration);
        }
        else
        {
            float followSpeed = GetFollowSpeed();
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, followSpeed * Time.deltaTime);
        }

        // 应用震动偏移
        transform.position += shakeOffset;

        // 更新上次玩家位置（用于瞬移检测）
        lastPlayerPos = playerTarget.position;
    }

    /// <summary>
    /// 根据当前状态返回相应的跟随速度
    /// </summary>
    float GetFollowSpeed()
    {
        switch (currentState)
        {
            case CameraState.Free:
                return freeFollowSpeed;
            case CameraState.LockOn:
                return lockOnFollowSpeed;
            case CameraState.Attack:
                return attackFollowSpeed;
            case CameraState.Cinematic:
                return 1f;
            default:
                return freeFollowSpeed;
        }
    }

    /// <summary>
    /// 处理输入：屏幕拖动、双击校正、移动键检测以及模拟攻击输入
    /// </summary>
    void HandleInput()
    {
        Vector2 currentMousePos = Input.mousePosition;
        if (Input.GetMouseButton(0))
        {
            Vector2 delta = currentMousePos - lastMousePosition;
            if (delta.magnitude > dragThreshold)
            {
                isDragging = true;
                // 拖动时立即切换到自由状态
                if (currentState != CameraState.Free)
                    SwitchState(CameraState.Free, 0.3f);
                yaw += delta.x * freeRotationSpeed * Time.deltaTime * 0.1f;
                pitch -= delta.y * freeRotationSpeed * Time.deltaTime * 0.1f;
                pitch = Mathf.Clamp(pitch, freeVerticalMin, freeVerticalMax);
            }
        }
        else
        {
            isDragging = false;
            // 双击校正相机正方向
            if (Input.GetMouseButtonDown(0))
            {
                float currentTime = Time.time;
                if (currentTime - lastTapTime < doubleTapThreshold)
                {
                    ResetCameraDirection();
                }
                lastTapTime = currentTime;
            }
        }
        lastMousePosition = currentMousePos;

        // 移动键检测（假设 "Horizontal" 和 "Vertical" 轴）
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            movementHoldTime += Time.deltaTime;
            if (movementHoldTime >= movementHoldThreshold && currentState != CameraState.Free)
                SwitchState(CameraState.Free, 0.3f);
        }
        else
        {
            movementHoldTime = 0f;
        }

        // 模拟攻击输入：按下空格键时切换到攻击状态
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchState(CameraState.Attack, 0.1f);
        }
    }

    /// <summary>
    /// 重置相机正方向：使 yaw 与玩家正方向一致
    /// </summary>
    void ResetCameraDirection()
    {
        yaw = playerTarget.eulerAngles.y;
    }

    /// <summary>
    /// 检查状态：根据外部输入和玩家状态（例如瞬移）切换状态
    /// </summary>
    void CheckState()
    {
        // 检测玩家瞬移：如果本帧与上一帧位置变化超过阈值，切换到瞬移状态
        float teleportThreshold = 10f;
        if (Vector3.Distance(lastPlayerPos, playerTarget.position) > teleportThreshold)
        {
            SwitchState(CameraState.Teleport, 0f);
        }
        else if (lockOnTarget != null)
        {
            // 如果存在锁定目标且未进行自由拖动，保持锁定状态
            if (currentState != CameraState.Attack && !isDragging)
                SwitchState(CameraState.LockOn, 0.2f);
        }
        else if (cinematicTarget != null && Input.GetKey(KeyCode.C))
        {
            SwitchState(CameraState.Cinematic, cinematicBlendTime);
        }
        else
        {
            // 默认自由状态
            if (currentState != CameraState.Free && !isDragging)
                SwitchState(CameraState.Free, 0.3f);
        }
    }

    /// <summary>
    /// 根据当前状态计算理想的相机位置和旋转
    /// </summary>
    void ComputeDesiredTransform(out Vector3 pos, out Quaternion rot)
    {
        switch (currentState)
        {
            case CameraState.Free:
                {
                    Quaternion freeRot = Quaternion.Euler(pitch, yaw, 0);
                    pos = playerTarget.position - freeRot * Vector3.forward * freeDistance + Vector3.up * freeHeight;
                    rot = Quaternion.LookRotation(playerTarget.position + Vector3.up * freeHeight - pos);
                }
                break;
            case CameraState.LockOn:
                {
                    if (lockOnTarget != null)
                    {
                        Vector3 midPoint = (playerTarget.position + lockOnTarget.position) * 0.5f;
                        Quaternion lookRot = Quaternion.LookRotation(midPoint - playerTarget.position);
                        pos = playerTarget.position - lookRot * Vector3.forward * lockOnDistance + Vector3.up * lockOnHeight;
                        rot = Quaternion.LookRotation(midPoint - pos);
                    }
                    else
                    {
                        // 回退到自由状态
                        Quaternion freeRot = Quaternion.Euler(pitch, yaw, 0);
                        pos = playerTarget.position - freeRot * Vector3.forward * freeDistance + Vector3.up * freeHeight;
                        rot = Quaternion.LookRotation(playerTarget.position + Vector3.up * freeHeight - pos);
                    }
                }
                break;
            case CameraState.Attack:
                {
                    if (lockOnTarget != null)
                    {
                        Vector3 midPoint = (playerTarget.position + lockOnTarget.position) * 0.5f;
                        Quaternion lookRot = Quaternion.LookRotation(midPoint - playerTarget.position);
                        pos = playerTarget.position - lookRot * Vector3.forward * attackDistance + Vector3.up * attackHeight;
                        rot = Quaternion.LookRotation(midPoint - pos);
                    }
                    else
                    {
                        Quaternion freeRot = Quaternion.Euler(pitch, yaw, 0);
                        pos = playerTarget.position - freeRot * Vector3.forward * freeDistance + Vector3.up * freeHeight;
                        rot = Quaternion.LookRotation(playerTarget.position + Vector3.up * freeHeight - pos);
                    }
                }
                break;
            case CameraState.Cinematic:
                {
                    if (cinematicTarget != null)
                    {
                        pos = cinematicTarget.position;
                        rot = cinematicTarget.rotation;
                    }
                    else
                    {
                        Quaternion freeRot = Quaternion.Euler(pitch, yaw, 0);
                        pos = playerTarget.position - freeRot * Vector3.forward * freeDistance + Vector3.up * freeHeight;
                        rot = Quaternion.LookRotation(playerTarget.position + Vector3.up * freeHeight - pos);
                    }
                }
                break;
            case CameraState.Teleport:
                {
                    Quaternion freeRot = Quaternion.Euler(pitch, yaw, 0);
                    pos = playerTarget.position - freeRot * Vector3.forward * freeDistance + Vector3.up * freeHeight;
                    rot = Quaternion.LookRotation(playerTarget.position + Vector3.up * freeHeight - pos);
                }
                break;
            default:
                {
                    Quaternion freeRot = Quaternion.Euler(pitch, yaw, 0);
                    pos = playerTarget.position - freeRot * Vector3.forward * freeDistance + Vector3.up * freeHeight;
                    rot = Quaternion.LookRotation(playerTarget.position + Vector3.up * freeHeight - pos);
                }
                break;
        }
    }

    /// <summary>
    /// 状态切换：通过设定过渡时间，实现自然融合
    /// </summary>
    public void SwitchState(CameraState newState, float blendTime)
    {
        if (newState == currentState)
            return;
        previousState = currentState;
        currentState = newState;
        transitionDuration = blendTime;
        transitionTime = 0f;
        isTransitioning = (blendTime > 0f);
    }

    /// <summary>
    /// 更新状态过渡计时
    /// </summary>
    void UpdateTransition()
    {
        if (isTransitioning)
        {
            transitionTime += Time.deltaTime;
            if (transitionTime >= transitionDuration)
                isTransitioning = false;
        }
    }

    /// <summary>
    /// 使用 SphereCast 检测障碍，调整摄像机位置防止穿墙
    /// </summary>
    Vector3 AdjustForCollision(Vector3 desiredPos, Vector3 pivotPos)
    {
        Vector3 dir = desiredPos - pivotPos;
        float dist = dir.magnitude;
        dir.Normalize();
        RaycastHit hit;
        if (Physics.SphereCast(pivotPos, collisionRadius, dir, out hit, dist))
        {
            return hit.point + hit.normal * collisionOffset;
        }
        return desiredPos;
    }

    /// <summary>
    /// 触发摄像机震动效果（例如在受到攻击或大招关键帧调用）
    /// </summary>
    public void ShakeCamera(float duration, float magnitude)
    {
        StopCoroutine("DoCameraShake");
        StartCoroutine(DoCameraShake(duration, magnitude));
    }

    IEnumerator DoCameraShake(float duration, float magnitude)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            shakeOffset = Random.insideUnitSphere * magnitude;
            elapsed += Time.deltaTime;
            yield return null;
        }
        shakeOffset = Vector3.zero;
    }
}

