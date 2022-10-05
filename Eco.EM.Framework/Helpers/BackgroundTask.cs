//Proposed by nidaren on 20-Sep-2022

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#nullable enable

namespace Eco.EM.Framework.Helpers
{
    /// <summary>
    /// Provides a background task, based on a new PeriodicTimer available in net6, also takes into account execution time of the routine method.
    /// It can be stopped, started, awaited, cancelled and protects against spawning a new task, when old one is still running.
    /// </summary>
    public class BackgroundTask
    {
        private Task? _timerTask;
        private readonly PeriodicTimer _timer;
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public BackgroundTask(TimeSpan interval)
        {
            _timer = new PeriodicTimer(interval);
        }

        public void Start(Action routineToDo)
        {
            _timerTask = DoWorkAsync(routineToDo);
        }

        private async Task DoWorkAsync(Action routineToDo)
        {
            try
            {
                while (await _timer.WaitForNextTickAsync(_cancellationTokenSource.Token))
                {
                    routineToDo();
                }
            }
            catch (OperationCanceledException)
            {

            }
        }

        public async Task StopAsync()
        {
            if (_timerTask is null)
            {
                return;
            }

            _cancellationTokenSource.Cancel();
            await _timerTask;
            _cancellationTokenSource.Dispose();
        }
    }
}
