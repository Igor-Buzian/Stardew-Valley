// File: Scripts/Systems/TimeSystem.cs
// Purpose: Drives in-game time: minutes, hours, days, day/night cycle.
// WHY: Time is a global concern that many systems need (farming growth,
//      NPC schedules, UI clock). It lives as a service and fires events
//      rather than being polled — pull vs push; push wins for perf.

using UnityEngine;
using FarmSim.Core;

namespace FarmSim.Systems
{
    public class TimeSystem : MonoBehaviour
    {
        // ─── Config ──────────────────────────────────────────────────────────
        [Header("Time Settings")]
        [SerializeField] private float realSecondsPerGameMinute = 0.5f; // 1 real sec = 2 game min
        [SerializeField] private int dayStartHour  = 6;   // 6 AM
        [SerializeField] private int nightStartHour = 22;  // 10 PM (auto-sleep)
        [SerializeField] private int minutesPerDay  = 1440; // 24 * 60

        // ─── State ───────────────────────────────────────────────────────────
        private float _elapsedRealSeconds;
        private int   _currentDay   = 1;
        private int   _currentHour;
        private int   _currentMinute;
        private bool  _isNight;
        private bool  _paused;

        // ─── Public Accessors ────────────────────────────────────────────────
        public int   CurrentDay    => _currentDay;
        public int   CurrentHour   => _currentHour;
        public int   CurrentMinute => _currentMinute;
        public bool  IsNight       => _isNight;
        /// Normalized 0..1 representing progress through the day
        public float DayProgress   => GetTotalMinutes() / (float)minutesPerDay;

        public void Init()
        {
            SetTime(dayStartHour, 0);
            _paused = false;
        }

        private void Update()
        {
            if (_paused) return;

            _elapsedRealSeconds += Time.deltaTime;

            if (_elapsedRealSeconds >= realSecondsPerGameMinute)
            {
                _elapsedRealSeconds -= realSecondsPerGameMinute;
                TickMinute();
            }
        }

        private void TickMinute()
        {
            _currentMinute++;
            if (_currentMinute >= 60)
            {
                _currentMinute = 0;
                _currentHour++;

                if (_currentHour >= 24)
                {
                    AdvanceDay();
                    return;
                }

                CheckDayNightTransition();
            }

            GameEvents.RaiseTimeChanged(DayProgress);
        }

        private void CheckDayNightTransition()
        {
            bool shouldBeNight = _currentHour >= nightStartHour || _currentHour < dayStartHour;

            if (shouldBeNight && !_isNight)
            {
                _isNight = true;
                GameEvents.RaiseNightBegan();
            }
            else if (!shouldBeNight && _isNight)
            {
                _isNight = false;
                GameEvents.RaiseMorningBegan();
            }
        }

        public void AdvanceDay()
        {
            _currentDay++;
            SetTime(dayStartHour, 0);
            GameEvents.RaiseDayChanged(_currentDay);
            GameEvents.RaiseMorningBegan();
            _isNight = false;
        }

        public void SetTime(int hour, int minute)
        {
            _currentHour   = Mathf.Clamp(hour,   0, 23);
            _currentMinute = Mathf.Clamp(minute, 0, 59);
            _elapsedRealSeconds = 0f;
            GameEvents.RaiseTimeChanged(DayProgress);
        }

        public void SetPaused(bool paused) => _paused = paused;

        private int GetTotalMinutes() => _currentHour * 60 + _currentMinute;

        /// Returns "HH:MM AM/PM" formatted string for UI
        public string GetFormattedTime()
        {
            int displayHour = _currentHour % 12;
            if (displayHour == 0) displayHour = 12;
            string amPm = _currentHour < 12 ? "AM" : "PM";
            return $"{displayHour}:{_currentMinute:00} {amPm}";
        }

        // ─── Save/Load Support ───────────────────────────────────────────────
        public TimeData GetSaveData() => new TimeData
        {
            day    = _currentDay,
            hour   = _currentHour,
            minute = _currentMinute
        };

        public void LoadFromData(TimeData data)
        {
            _currentDay = data.day;
            SetTime(data.hour, data.minute);
        }
    }

    [System.Serializable]
    public class TimeData
    {
        public int day;
        public int hour;
        public int minute;
    }
}