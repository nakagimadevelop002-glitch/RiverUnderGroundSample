using UnityEngine;

public class MoveWhileInState_Rigidbody_Late : StateMachineBehaviour
{
    [Header("When to move (normalized 0..1)")]
    [SerializeField, Range(0f, 1f)]
    private float startNormalized = 0.6f;   // ここ以降で動く

    [SerializeField, Range(0f, 1f)]
    private float endNormalized = 1.0f;     // ここまで動く（1.0 = 最後まで）

    [SerializeField]
    private bool treatAsLoopingState = true;

    [Header("Movement (Rigidbody)")]
    [SerializeField]
    private Vector3 velocity = new Vector3(0f, 0f, 1f); // m/s

    [SerializeField]
    private bool useLocalSpace = true;

    [Header("Exit Behavior")]
    [SerializeField]
    private bool stopOnExit = true;

    private Rigidbody _rb;

    public override void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        _rb = animator.GetComponent<Rigidbody>();

        if (_rb == null)
        {
            Debug.LogWarning("Rigidbody not found. This behaviour requires Rigidbody.");
        }
    }

    public override void OnStateUpdate(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        if (_rb == null) return;

        // normalizedTime の扱い（ループ対応）
        float t01 = treatAsLoopingState
            ? stateInfo.normalizedTime % 1f
            : Mathf.Clamp01(stateInfo.normalizedTime);

        // 後半ウィンドウ外なら XZ 速度を止める
        if (t01 < startNormalized || t01 > endNormalized)
        {
            _rb.velocity = new Vector3(0f, _rb.velocity.y, 0f);
            return;
        }

        Vector3 v = useLocalSpace
            ? animator.transform.TransformDirection(velocity)
            : velocity;

        // Y は既存の重力やジャンプを保持
        _rb.velocity = new Vector3(v.x, _rb.velocity.y, v.z);
    }

    public override void OnStateExit(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        if (_rb == null || !stopOnExit) return;

        // Stateを抜けたら横移動だけ止める
        _rb.velocity = new Vector3(0f, _rb.velocity.y, 0f);
    }
}
