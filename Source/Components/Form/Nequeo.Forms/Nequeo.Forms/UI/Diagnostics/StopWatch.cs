using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Nequeo.Forms.UI.Diagnostics
{
    /// <summary>
    /// Stopwatch user control.
    /// </summary>
    public partial class StopWatch : UserControl
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public StopWatch()
        {
            InitializeComponent();

            LoadStaticInfo();

            _stopWatch = new System.Diagnostics.Stopwatch();

            LoadInstanceInfo();

            timerStopWatch.Enabled = true;
            timerStopWatch.Interval = 1000;
        }

        private System.Diagnostics.Stopwatch _stopWatch;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart_Click(object sender, EventArgs e)
        {
            _stopWatch.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStop_Click(object sender, EventArgs e)
        {
            _stopWatch.Stop();
            LoadInstanceInfo();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_Click(object sender, EventArgs e)
        {
            _stopWatch.Reset();
            LoadInstanceInfo();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNew_Click(object sender, EventArgs e)
        {
            _stopWatch = System.Diagnostics.Stopwatch.StartNew();
            LoadInstanceInfo();
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadStaticInfo()
        {
            txtFrequency.Text = System.Diagnostics.Stopwatch.Frequency.ToString("N");
            chkSystemHighResTimer.Checked = System.Diagnostics.Stopwatch.IsHighResolution;
            txtTimeStamp.Text = System.Diagnostics.Stopwatch.GetTimestamp().ToString("N");
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadInstanceInfo()
        {
            txtElaspedTime.Text = _stopWatch.Elapsed.ToString();
            txtElaspedMilliseconds.Text = _stopWatch.ElapsedMilliseconds.ToString("N");
            txtElaspedTicks.Text = _stopWatch.ElapsedTicks.ToString("N");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerStopWatch_Tick(object sender, EventArgs e)
        {
            if (_stopWatch.IsRunning == true)
                LoadInstanceInfo();
        }
    }
}
