using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Ws.Extensions.Mvvm.ViewModels;

namespace Ws.Extensions.UI.Wpf.Controls
{
    /// <summary>
    /// Wrapper class for DispatcherTimer
    /// </summary>
    public class AdjustedTimer : IDisposable
    {
        private int _intervalMilliSeconds;
        private DispatcherTimer _timer;
        protected bool _disposed = false;
        private int _minIntervalMilliSeconds = 2000;

        private Action _timerTickAction;

        public AdjustedTimer(Action timerTickAction)
        {
            Debug.Assert(timerTickAction != null, "Action<object, EventArgs> - timerTickAction should be supplied");

            _timerTickAction = timerTickAction;
        }

        /// <summary>
        /// Defines minimum timer interval in milliseconds
        /// </summary>
        public int MinIntervalMilliSeconds
        {
            get { return _minIntervalMilliSeconds; }
            set
            {
                Debug.Assert(value >= 100, "MinIntervalSeconds can't be < 100");
                _minIntervalMilliSeconds = value;
            }
        }

        /// <summary>
        /// Defines whether to execute action first before starting the timer.
        /// </summary>
        public bool ExecuteActionOnStart { get; set; } = true;

        public int IntervalMilliSeconds
        {
            get { return _intervalMilliSeconds; }
            set
            {
                if(_intervalMilliSeconds != value)
                {
                    _intervalMilliSeconds = value;
                    SetTimerInterval();
                }
            }
        }

        /// <summary>
        /// Disposes resources
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                StopTimer();
                _timer = null;

                _timerTickAction = null;
            }

            _disposed = true;
        }

        /// <summary>
        /// Starts Timer
        /// </summary>
        public void StartTimer()
        {
            Debug.Assert(_timerTickAction != null, "_timerTickAction should not be null");

            if (_timer == null)
                _timer = CreateTimer();

            //Starting from execution action first if timer was disabled(stopped) before and ExecuteEactionOnStart = true
            if (!_timer.IsEnabled && ExecuteActionOnStart)
            {
                _timerTickAction();
            }

            //If timer is running, just change interval for the next cycle
            SetTimerInterval();

            _timer.IsEnabled = true;
            _timer.Start();
        }

        /// <summary>
        /// Stops Timer
        /// </summary>
        public void StopTimer()
        {
            if (_timer == null)
                return;

            _timer.Stop();
            _timer.IsEnabled = false;
        }

        private DispatcherTimer CreateTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += OmTimerTick;

            return timer;
        }

        private void OmTimerTick(object sender, EventArgs e)
        {
            SetTimerInterval();
            _timerTickAction();
        }

        private void SetTimerInterval()
        {
            if (_timer == null)
                _timer = CreateTimer();

            TimeSpan currentInterval = _timer.Interval;
            TimeSpan newInterval = TimeSpan.FromMilliseconds(IntervalMilliSeconds < 100 ? MinIntervalMilliSeconds : IntervalMilliSeconds);
            if (currentInterval != newInterval)
                _timer.Interval = newInterval;
        }
    }
}
