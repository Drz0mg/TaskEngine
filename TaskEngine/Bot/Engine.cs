using System;
using System.Threading;
using System.Windows.Forms;
using TaskEngine.Helpers;
using ZzukBot.Game.Statics;

namespace TaskEngine.Bot
{
    internal class Engine
    {
        #region Field
        public static int PrioStateIdle = int.MinValue;
        public static int PrioStateAlive = 1;
        public static int PrioStateGhost = 2;
        public static int PrioStateDead = 3;
        public static string CurrentStateName = "";
        public static RunState CurrentState;
        private static StateMachine stateMachine;

        public enum RunState
        {
            Stopped,
            Paused,
            Running
        };
        #endregion

        public static void StartEngine()
        {
            stateMachine = new StateMachine();
            CurrentState = RunState.Running;
        }

        public static void Run()
        {
            try
            {
                if (ObjectManager.Instance.IsIngame && CurrentState.Equals(RunState.Running))
                {
                    stateMachine.DoWork();
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception e)
            {
                Logger.Log("Doh exception in the botting function: " + e);
            }
        }

        public static void PauseEngine()
        {
            CurrentState = RunState.Paused;
            ObjectManager.Instance.Player.CtmStopMovement();
        }

        public static void ResumeEngine()
        {
            CurrentState = RunState.Running;
        }

        public static void RestartEngine()
        {
            StopEngine();
            StartEngine();
        }

        public static void StopEngine()
        {
            CurrentState = RunState.Stopped;
            ObjectManager.Instance.Player.CtmStopMovement();
        }
    }
}