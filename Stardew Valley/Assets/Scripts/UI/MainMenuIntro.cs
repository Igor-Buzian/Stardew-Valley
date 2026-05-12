// File: Scripts/UI/MainMenuIntro.cs

using DG.Tweening;
using UnityEngine;


namespace FarmSim.UI
{
    public class MainMenuIntro : MonoBehaviour
    {
        [Header("Title")]
        [SerializeField] private RectTransform hops;
        [SerializeField] private RectTransform and;
        [SerializeField] private RectTransform herbs;

        [SerializeField] private CanvasGroup hopsGroup;
        [SerializeField] private CanvasGroup andGroup;
        [SerializeField] private CanvasGroup herbsGroup;

        [Header("Buttons")]
        [SerializeField] private RectTransform playButton;
        [SerializeField] private RectTransform optionsButton;
        [SerializeField] private RectTransform exitButton;

        [SerializeField] private CanvasGroup playGroup;
        [SerializeField] private CanvasGroup optionsGroup;
        [SerializeField] private CanvasGroup exitGroup;

        [Header("Settings")]
        [SerializeField] private float introDelay = 0.5f;
        [SerializeField] private float floatAmplitude = 8f;

        private Vector2 playPos;
        private Vector2 optionsPos;
        private Vector2 exitPos;

        private void Awake()
        {
            CachePositions();
            ResetState();
        }

        private void Start()
        {
            DG.Tweening.Sequence intro = DOTween.Sequence();

            intro.AppendInterval(introDelay);

            intro.Append(AnimateWord(hops, hopsGroup));
            intro.Append(AnimateWord(and, andGroup));
            intro.Append(AnimateWord(herbs, herbsGroup));

            intro.AppendCallback(StartFloating);

            intro.Append(AnimateButton(playButton, playGroup, playPos));
            intro.Append(AnimateButton(optionsButton, optionsGroup, optionsPos));
            intro.Append(AnimateButton(exitButton, exitGroup, exitPos));

            SetupButtons();
        }

        // ─────────────────────────────
        // INIT
        // ─────────────────────────────

        private void CachePositions()
        {
            playPos = playButton.anchoredPosition;
            optionsPos = optionsButton.anchoredPosition;
            exitPos = exitButton.anchoredPosition;
        }

        private void ResetState()
        {
            SetHidden(hops, hopsGroup);
            SetHidden(and, andGroup);
            SetHidden(herbs, herbsGroup);

            SetHidden(playButton, playGroup);
            SetHidden(optionsButton, optionsGroup);
            SetHidden(exitButton, exitGroup);
        }

        private void SetHidden(RectTransform t, CanvasGroup g)
        {
            t.localScale = Vector3.zero;
            g.alpha = 0;
        }

        // ─────────────────────────────
        // TITLE ANIMATION (AAA POP)
        // ─────────────────────────────

        private Sequence AnimateWord(RectTransform target, CanvasGroup group)
        {
            DG.Tweening.Sequence s = DOTween.Sequence();

            group.alpha = 0;
            target.localScale = Vector3.zero;

            s.Join(group.DOFade(1, 0.4f));

            s.Append(target.DOScale(1.15f, 0.35f)
                .SetEase(Ease.OutBack));

            s.Append(target.DOScale(1f, 0.2f)
                .SetEase(Ease.OutElastic));

            return s;
        }

        // ─────────────────────────────
        // BUTTONS (fly + fade + stagger)
        // ─────────────────────────────

        private Sequence AnimateButton(RectTransform btn, CanvasGroup group, Vector2 target)
        {
            DG.Tweening.Sequence s = DOTween.Sequence();

            Vector2 start = target + Vector2.down * 600f;
            
            btn.localScale = Vector3.one;
            btn.anchoredPosition = start;
            group.alpha = 0;

            s.Join(group.DOFade(1, 0.5f));

            s.Join(btn.DOAnchorPos(target, 0.9f)
                .SetEase(Ease.OutBack));

            return s;
        }

        // ─────────────────────────────
        // FLOAT (idle loop animation)
        // ─────────────────────────────

        private void StartFloating()
        {
            FloatLoop(hops, 0);
            FloatLoop(and, 0.2f);
            FloatLoop(herbs, 0.4f);
        }

        private void FloatLoop(RectTransform t, float offset)
        {
            Vector2 basePos = t.anchoredPosition;

            DOTween.To(() => 0f, x =>
            {
                float y = Mathf.Sin(Time.time * 1.2f + offset) * floatAmplitude;
                t.anchoredPosition = basePos + Vector2.up * y;

            }, 1f, 999999f).SetEase(Ease.Linear);
        }

        // ─────────────────────────────
        // BUTTON INTERACTION (HOVER + CLICK)
        // ─────────────────────────────

        private void SetupButtons()
        {
            AddButtonFX(playButton);
            AddButtonFX(optionsButton);
            AddButtonFX(exitButton);
        }

        private void AddButtonFX(RectTransform btn)
        {
            var trigger = btn.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();

            AddEvent(trigger, UnityEngine.EventSystems.EventTriggerType.PointerEnter, () =>
            {
                btn.DOScale(1.08f, 0.2f).SetEase(Ease.OutBack);
            });

            AddEvent(trigger, UnityEngine.EventSystems.EventTriggerType.PointerExit, () =>
            {
                btn.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
            });

            AddEvent(trigger, UnityEngine.EventSystems.EventTriggerType.PointerClick, () =>
            {
                btn.DOPunchScale(Vector3.one * 0.15f, 0.25f, 10, 1f);
            });
        }

        private void AddEvent(UnityEngine.EventSystems.EventTrigger trigger,
            UnityEngine.EventSystems.EventTriggerType type,
            System.Action action)
        {
            var entry = new UnityEngine.EventSystems.EventTrigger.Entry
            {
                eventID = type
            };

            entry.callback.AddListener(_ => action());
            trigger.triggers.Add(entry);
        }
    }
}