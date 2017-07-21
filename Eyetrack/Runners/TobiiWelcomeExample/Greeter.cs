using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Tobii.Interaction;

namespace Eyetrack.Runners.TobiiWelcomeExample
{
    public class Greeter : IDisposable
    {
        private readonly Host _host;
        private readonly VirtualInteractorAgent _interactorAgent;
        private readonly VirtualInteractor _greetingInteractor;

        public Greeter(
            string defaultWindowId,
            Rectangle defaultWindowBounds,
            Action onGazeEnters,
            Action onGazeLeaves)
        {
            _host = new Host();
            _interactorAgent = _host.InitializeVirtualInteractorAgent(defaultWindowId);
            _greetingInteractor = _interactorAgent.AddInteractorFor(defaultWindowBounds);
            _greetingInteractor
                .WithGazeAware()
                .HasGaze(onGazeEnters)
                .LostGaze(onGazeLeaves);
        }

        public void Dispose()
        {
            _host.Dispose();
        }
    }
}
