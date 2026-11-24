using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SignalPuzzleController : InteractionFocus
{
    [Header("Puzzle Settings")]
    public List<PipeBase> pipes = new List<PipeBase>();
    public UnityEvent onPuzzleComplete;
    public bool isCleard { get; private set; } = false;

    [Header("Slider")]
    [SerializeField] private Slider frequencySlider;
    [SerializeField] private Slider phaseSlider;
    [SerializeField] private Slider amplitudeSlider;

    [Header("Graphs")]
    [SerializeField] private WaveGraph targetGraph;
    [SerializeField] private WaveGraph currentGraph;

    [Header("Sync Settings")]
    [SerializeField] private float frequencyTolerance = 0.1f;
    [SerializeField] private float phaseTolerance = 8f;
    [SerializeField] private float amplitudeTolerance = 0.03f;

    [Header("Animation Settings")]
    public float animationSpeed = 60f;
    private float animationPhaseDegrees;

    private void OnEnable()
    {
        if (frequencySlider != null)
            frequencySlider.onValueChanged.AddListener(OnValueChange);

        if (phaseSlider != null)
            phaseSlider.onValueChanged.AddListener(OnValueChange);

        if (amplitudeSlider != null)
            amplitudeSlider.onValueChanged.AddListener(OnValueChange);
    }

    private void OnDisable()
    {
        if (frequencySlider != null)
            frequencySlider.onValueChanged.RemoveListener(OnValueChange);

        if (phaseSlider != null)
            phaseSlider.onValueChanged.RemoveListener(OnValueChange);

        if (amplitudeSlider != null)
            amplitudeSlider.onValueChanged.RemoveListener(OnValueChange);
    }


    protected override void Update()
    {
        base.Update();

        animationPhaseDegrees = Mathf.Repeat(
            animationPhaseDegrees + animationSpeed * Time.deltaTime,
            360f
        );

        targetGraph.animationPhaseDegrees = animationPhaseDegrees;
        targetGraph.SetVerticesDirty();

        currentGraph.animationPhaseDegrees = animationPhaseDegrees;
        currentGraph.SetVerticesDirty();
    }

    public override void ExitFocus()
    {
        base.ExitFocus();
        floatingUITransform.gameObject.SetActive(false);
    }

    void OnValueChange(float temp)
    {
        if (currentGraph == null) return;

        float frequency = frequencySlider ? frequencySlider.value : 5;
        float phase = phaseSlider ? phaseSlider.value : 0;
        float amplitude = amplitudeSlider ? amplitudeSlider.value : 0.1f;

        currentGraph.SetWave(frequency, phase, amplitude);

        if(IsSame())
        {
            // 근사치인 경우
            currentGraph.SetWave(
                targetGraph.frequency, 
                targetGraph.phaseDegrees, 
                targetGraph.amplitude
                );       // 동일한 모양으로 맞춰줌

            OnPuzzleComplete();     // 퍼즐 클리어
        }
    }

    protected override void Complete()
    {
        base.Complete();
        floatingUITransform.gameObject.SetActive(true);
        SetNewGraphs();
    }

    void SetNewGraphs()
    {
        if (targetGraph == null || currentGraph == null) return;

        targetGraph.SetRandom();
        currentGraph.SetRandom();

        while(IsSame())                 // 이미 근사치인 경우 다시 돌리도록
        {
            currentGraph.SetRandom();
        }

        if (frequencySlider != null) 
            frequencySlider.SetValueWithoutNotify(currentGraph.frequency);

        if (phaseSlider != null) 
            phaseSlider.SetValueWithoutNotify(currentGraph.phaseDegrees);

        if (amplitudeSlider != null)
            amplitudeSlider.SetValueWithoutNotify(currentGraph.amplitude);
    }

    bool IsSame()
    {
        if (targetGraph == null || currentGraph == null) return false;

        float frequencyError = Mathf.Abs(targetGraph.frequency - currentGraph.frequency);

        float phaseError = Mathf.Abs(
            Mathf.DeltaAngle(
                targetGraph.phaseDegrees,
                currentGraph.phaseDegrees
            )
        );

        float amplitudeError = Mathf.Abs(targetGraph.amplitude - currentGraph.amplitude);

        bool isSynchronized =
            frequencyError <= frequencyTolerance &&
            phaseError <= phaseTolerance &&
            amplitudeError <= amplitudeTolerance;

        if (isSynchronized)
        {
            Debug.Log("SIGNAL SYNCHRONIZED");
        }
        else
        {
            Debug.Log("SYNCHRONIZATION FAILED");
        }


        return isSynchronized;
    }

    public void OnPuzzleComplete()
    {
        Debug.Log("퍼즐 완료");
        ExitFocus();
        interactable = false;
        isCleard = true;
        onPuzzleComplete?.Invoke();
    }
}
