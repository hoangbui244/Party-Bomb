using UnityEngine;
public class Biathlon_SpurRenderer : MonoBehaviour {
    private enum Mode {
        Normal,
        Upward
    }
    [SerializeField]
    private TrailRenderer leftNormalTrail;
    [SerializeField]
    private TrailRenderer rightNormalTrail;
    [SerializeField]
    private Transform leftNormalTrailAnchor;
    [SerializeField]
    private Transform rightNormalTrailAnchor;
    [SerializeField]
    private LineRenderer[] leftUpwardSlopeTrails;
    [SerializeField]
    private LineRenderer[] rightUpwardSlopeTrails;
    [SerializeField]
    private Transform[] leftUpwardSlopeAnchors;
    [SerializeField]
    private Transform[] rightUpwardSlopeAnchors;
    [SerializeField]
    private ParticleSystem leftSnowDust;
    [SerializeField]
    private ParticleSystem rightSnowDust;
    [SerializeField]
    private ParticleSystem leftUpwardDust;
    [SerializeField]
    private ParticleSystem rightUpwardDust;
    [SerializeField]
    private Biathlon_CharacterAnimator animator;
    private Biathlon_Character playingCharacter;
    private Mode mode;
    private bool leftIsGround;
    private bool rightIsGround;
    private int leftUpwardSlopeTrailIndex;
    private int rightUpwardSlopeTrailIndex;
    private bool playingSnowDust;
    public void Init(Biathlon_Character character) {
        playingCharacter = character;
        ActivateNormalRenderer();
        leftSnowDust.Play();
        rightSnowDust.Play();
        //??leftSnowDust.emission.rateOverTimeMultiplier = 0f;
        //??rightSnowDust.emission.rateOverTimeMultiplier = 0f;
    }
    public void UpdateMethod() {
        switch (mode) {
            case Mode.Normal:
                UpdateNormal();
                break;
            case Mode.Upward:
                UpdateUpward();
                break;
        }
    }
    public void ActivateNormalRenderer() {
        mode = Mode.Normal;
        leftNormalTrail.emitting = true;
        rightNormalTrail.emitting = true;
        LineRenderer[] array = leftUpwardSlopeTrails;
        for (int i = 0; i < array.Length; i++) {
            array[i].gameObject.SetActive(value: false);
        }
        array = rightUpwardSlopeTrails;
        for (int i = 0; i < array.Length; i++) {
            array[i].gameObject.SetActive(value: false);
        }
        ParticleSystem.EmissionModule emission = leftSnowDust.emission;
        ParticleSystem.EmissionModule emission2 = rightSnowDust.emission;
        emission.enabled = true;
        emission2.enabled = true;
    }
    public void DeactivateNormalRenderer() {
        leftNormalTrail.emitting = false;
        rightNormalTrail.emitting = false;
    }
    public void ActivateUpwardSlopeRenderer() {
        mode = Mode.Upward;
        leftIsGround = true;
        rightIsGround = true;
        ParticleSystem.EmissionModule emission = leftSnowDust.emission;
        ParticleSystem.EmissionModule emission2 = rightSnowDust.emission;
        emission.enabled = false;
        emission2.enabled = false;
    }
    public void PlaySnowDust() {
        if (!playingSnowDust) {
            playingSnowDust = true;
        }
    }
    public void StopSnowDust() {
        if (playingSnowDust) {
            playingSnowDust = false;
            //??float num3 = leftSnowDust.emission.rateOverTimeMultiplier = (rightSnowDust.emission.rateOverTimeMultiplier = 0f);
        }
    }
    private void UpdateNormal() {
        if (playingSnowDust) {
            ParticleSystem.EmissionModule emission = leftSnowDust.emission;
            ParticleSystem.EmissionModule emission2 = rightSnowDust.emission;
            float speed = playingCharacter.Speed;
            float num3 = emission.rateOverTimeMultiplier = (emission2.rateOverTimeMultiplier = speed * 90f);
        }
        Vector3 position = leftNormalTrailAnchor.position;
        RaycastDown(ray: new Ray(position, Vector3.down), trail: leftNormalTrail);
        position = rightNormalTrailAnchor.position;
        RaycastDown(ray: new Ray(position, Vector3.down), trail: rightNormalTrail);
    }
    private void UpdateUpward() {
        if (!animator.IsHardRun) {
            leftNormalTrail.emitting = true;
            rightNormalTrail.emitting = true;
            return;
        }
        leftNormalTrail.emitting = false;
        rightNormalTrail.emitting = false;
        bool num = leftIsGround;
        leftIsGround = IsGround(leftNormalTrailAnchor.position, out RaycastHit _);
        if (!num && leftIsGround) {
            LineRenderer obj = leftUpwardSlopeTrails[leftUpwardSlopeTrailIndex];
            obj.gameObject.SetActive(value: true);
            obj.SetPosition(0, leftUpwardSlopeAnchors[0].position);
            obj.SetPosition(1, leftUpwardSlopeAnchors[1].position);
            leftUpwardSlopeTrailIndex++;
            if (leftUpwardSlopeTrailIndex >= leftUpwardSlopeTrails.Length) {
                leftUpwardSlopeTrailIndex = 0;
            }
            leftUpwardDust.Play(withChildren: true);
        }
        bool num2 = rightIsGround;
        rightIsGround = IsGround(rightNormalTrailAnchor.position, out RaycastHit _);
        if (!num2 && rightIsGround) {
            LineRenderer obj2 = rightUpwardSlopeTrails[rightUpwardSlopeTrailIndex];
            obj2.gameObject.SetActive(value: true);
            obj2.SetPosition(0, rightUpwardSlopeAnchors[0].position);
            obj2.SetPosition(1, rightUpwardSlopeAnchors[1].position);
            rightUpwardSlopeTrailIndex++;
            if (rightUpwardSlopeTrailIndex >= rightUpwardSlopeAnchors.Length) {
                rightUpwardSlopeTrailIndex = 0;
            }
            rightUpwardDust.Play(withChildren: true);
        }
    }
    private bool IsGround(Vector3 point, out RaycastHit hit) {
        UnityEngine.Debug.DrawRay(point, Vector3.down * 0.1f, Color.red);
        if (!Physics.Raycast(new Ray(point, Vector3.down), out hit, 0.13f, 8388608)) {
            return false;
        }
        return true;
    }
    private void RaycastDown(TrailRenderer trail, Ray ray) {
        if (!Physics.Raycast(ray, out RaycastHit hitInfo, 0.15f, 8388608)) {
            trail.emitting = false;
            return;
        }
        trail.emitting = true;
        Vector3 position = hitInfo.point + hitInfo.normal * 0.02f;
        trail.transform.position = position;
    }
}
