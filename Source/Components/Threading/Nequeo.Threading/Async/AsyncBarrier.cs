/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

#region Nequeo Pty Ltd License
/*
    Permission is hereby granted, free of charge, to any person
    obtaining a copy of this software and associated documentation
    files (the "Software"), to deal in the Software without
    restriction, including without limitation the rights to use,
    copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the
    Software is furnished to do so, subject to the following
    conditions:

    The above copyright notice and this permission notice shall be
    included in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
    OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
    HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
    WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
    FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
    OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Nequeo.Threading.Async
{
    /// <summary>Provides an asynchronous barrier.</summary>
    [DebuggerDisplay("ParticipantCount={ParticipantCount}, RemainingCount={RemainingCount}")]
    public sealed class AsyncBarrier
    {
        /// <summary>The number of participants in the barrier.</summary>
        private readonly int _participantCount;
        /// <summary>The task used to signal completion of the current round.</summary>
        private TaskCompletionSource<object> _currentSignalTask;
        /// <summary>The number of participants remaining to arrive for this round.</summary>
        private int _remainingParticipants;

        /// <summary>Initializes the BarrierAsync with the specified number of participants.</summary>
        /// <param name="participantCount">The number of participants in the barrier.</param>
        public AsyncBarrier(int participantCount)
        {
            if (participantCount <= 0) throw new ArgumentOutOfRangeException("participantCount");
            _participantCount = participantCount;

            _remainingParticipants = participantCount;
            _currentSignalTask = new TaskCompletionSource<object>();
        }

        /// <summary>Gets the participant count.</summary>
        public int ParticipantCount { get { return _participantCount; } }
        /// <summary>Gets the number of participants still not yet arrived in this round.</summary>
        public int RemainingCount { get { return _remainingParticipants; } }

        /// <summary>Signals that a participant has arrived.</summary>
        /// <returns>A Task that will be signaled when the current round completes.</returns>
        public Task SignalAndWait()
        {
            var curCts = _currentSignalTask;
        #pragma warning disable 420
            if (System.Threading.Interlocked.Decrement(ref _remainingParticipants) == 0)
        #pragma warning restore 420
            {
                _remainingParticipants = _participantCount;
                _currentSignalTask = new TaskCompletionSource<object>();
                curCts.SetResult(null);
            }
            return curCts.Task;
        }
    }
}