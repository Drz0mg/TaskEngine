using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using TaskEngine.Helpers;
using TaskEngine.Tasks;
using ZzukBot.Constants;
using ZzukBot.ExtensionFramework.Interfaces;
using ZzukBot.Game.Statics;

namespace TaskEngine.Bot
{
    [Export(typeof(IBotBase))]
    public class BotBase : IBotBase
    {
        public Action OnStopCallback { get; private set; }
        public Action OnPauseCallBack { get; private set; }

        public string Name => "TaskEngine";
        public string Author => "z0mg";
        public int Version => 0;

        public BotBase()
        {
            RegisterTasks();
            DirectX.Instance.OnEndSceneExecution += ptr => Engine.Run();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool Start(Action onStopCallback)
        {
            this.OnStopCallback = onStopCallback;
            Engine.StartEngine();
            return true;
        }

        public void PauseBotbase(Action onPauseCallback)
        {
            this.OnPauseCallBack = onPauseCallback;
            Engine.PauseEngine();
            this.OnPauseCallBack();
        }

        public bool ResumeBotbase()
        {
            Engine.ResumeEngine();
            return true;
        }

        public void Stop()
        {
            Engine.StopEngine();
            this.OnStopCallback();
        }

        public void ShowGui()
        {
            Logger.Log("{0} - {1} - {2}" , ObjectManager.Instance.Player.Position.X, ObjectManager.Instance.Player.Position.Y, ObjectManager.Instance.Player.Position.Z);
        }

        private static void RegisterTasks()
        {
            ParserTask.registeredTasks.Clear();

            Type taskType = typeof(ParserTask);
            Assembly cur = Assembly.GetExecutingAssembly();
            Assembly[] allAsms = System.AppDomain.CurrentDomain.GetAssemblies();

            // this is only sorted for debugging
            SortedList<string, Type> internalTypes = new SortedList<string, Type>();
            SortedList<string, Type> userTypes = new SortedList<string, Type>();

            foreach (Type t in cur.GetTypes())
            {
                if (t.IsSubclassOf(taskType) && !t.IsAbstract)
                {
                    internalTypes[t.FullName] = t;
                }
            }


            foreach (Assembly a in allAsms)
            {
                if (a == cur)
                    continue; // already did them

                try
                {
                    foreach (Type t in a.GetTypes())
                    {
                        if (t.IsSubclassOf(taskType) && !t.IsAbstract)
                        {
                            userTypes[t.FullName] = t;
                        }
                    }
                }
                catch { }
            }

            List<Type> allTypes = new List<Type>();
            allTypes.AddRange(internalTypes.Values);
            allTypes.AddRange(userTypes.Values);

            foreach (Type t in allTypes)
            {
                try
                {
                    FieldInfo f = t.GetField("ParserKeyword");
                    string s = "";

                    if (null != f)
                    {
                        s = f.GetValue(null).ToString();
                    }
                    else
                    {
                        // if the field doesn't exist, use the class name
                        int index = t.Name.LastIndexOf("Task", StringComparison.Ordinal);
                        s = index >= 0 ? t.Name.Substring(0, index) : t.Name;
                    }

                    foreach (string ss in s.Split(','))
                    {
                        string sss = ss.Trim();

                        if (sss != "")
                            ParserTask.RegisterTask(sss, t);
                    }
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
