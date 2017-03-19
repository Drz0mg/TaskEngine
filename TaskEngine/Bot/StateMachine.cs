using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using TaskEngine.Activities;
using TaskEngine.Helpers;
using TaskEngine.Parser;
using TaskEngine.Tasks;

namespace TaskEngine.Bot
{


    internal class StateMachine
    {
        private readonly Stack<Task> taskQueue;
        private readonly Stack<Activity> activityQueue;
        private Task rootTask;
        private RootNode root;
        private readonly Preprocessor pproc;
        private readonly TaskParser taskParser;
        private TextReader reader;
        private NodeTask astRoot;
        private Activity activity;

        private readonly GTimer taskTimer;
        private readonly GTimer nothingToDoTimer;
        private readonly GTimer tick;

        public TreeNode RootNode { get; set; }
        
        public StateMachine()
        {
            this.reader = null;
            this.astRoot = null;
            if (RemoteTask.FileExist(Settings.TaskFile))
            {
                this.reader = RemoteTask.GetFile(Settings.TaskFile);
            }
            else
            {
                MessageBox.Show(string.Format("Your selected task file wasn't found:\n{0}", Settings.TaskFile));
                this.StopAll("");
            }

            if (this.reader != null)
            {
                this.pproc = new Preprocessor(this.reader);
                this.taskParser = new TaskParser(new StreamReader(this.pproc.ProcessedStream));
                this.astRoot = this.taskParser.ParseTask(null);
                this.reader.Close();
            }
            else
            {
                this.StopAll("TextReader was null. File not found?");
            }
            this.root = new RootNode();
            this.root.AddTask(this.astRoot);
            this.root.BindSymbols(); // Just to make it a tad faster

            this.taskQueue = new Stack<Task>();
            this.activityQueue = new Stack<Activity>();
            this.rootTask = TaskCreator.CreateTaskFromNode(this.root, null);
            if (this.rootTask == null)
                this.StopAll("No root task");
            TaskInfo.Root = this.rootTask;
            TaskCreator.CreateTreeFromTasks(this.rootTask);
            this.activity = null;
            this.taskTimer = new GTimer(300);  //300
            this.nothingToDoTimer = new GTimer(3 * 1000);
            this.tick = new GTimer(100);
        }

        public void DoWork()
        {
            if (this.activity == null || this.taskTimer.IsReady())
            {
                this.taskTimer.Reset();
                Activity newActivity = null;
                if (this.rootTask.WantToDoSomething())
                {
                    newActivity = this.rootTask.GetActivity();
                    this.nothingToDoTimer.Reset();
                }

                if (newActivity != null)
                {
                    if (newActivity.Task.GetType().ToString() == "Pather.Tasks.LoadTask")
                    {
                        Logger.Log("Queueing the old task tree");
                        this.taskQueue.Push(this.rootTask);
                        this.activityQueue.Push(this.activity);
                        if (this.activity != null)
                        {
                            bool done;
                            int wait = 0;
                            do
                            {
                                done = this.activity.Do();
                                wait++;
                            } while (!done && (wait > 100));

                            if (!done)
                                this.activity.Stop();

                            Task tr = this.activity.Task;
                            while (tr != null)
                            {
                                tr.IsActive = false;
                                tr = tr.Parent;
                            }
                            this.activity = null;
                        }
                        this.reader = null;
                        this.astRoot = null;

                        string loadfile = Functions.GetTaskFilePath() + ((LoadTask)newActivity.Task).File;
                        Logger.Log("Loading file - " + loadfile);
                        if (RemoteTask.FileExist(loadfile))
                            this.reader = RemoteTask.GetFile(loadfile);
                        else
                            Logger.Log("File could not be loaded - " + loadfile);
                        if (this.reader != null)
                        {
                            Preprocessor pproc = new Preprocessor(this.reader);
                            TaskParser t = new TaskParser(new StreamReader(pproc.ProcessedStream));
                            this.astRoot = t.ParseTask(null);
                            this.reader.Close();
                            this.root = null;
                            this.root = new RootNode();
                            this.root.AddTask(this.astRoot);
                            this.root.BindSymbols(); // Just to make it a tad faster

                            this.rootTask = null;
                            this.rootTask = TaskCreator.CreateTaskFromNode(this.root, null);
                            Helpers.TaskInfo.Root = this.rootTask; // Desired?
                            TaskCreator.CreateTreeFromTasks(this.rootTask);
                            this.activity = null;
                            newActivity.Task.Restart();
                            newActivity = null;

                            if (this.rootTask == null)
                                Logger.Log("Load: No root task!");
                            else
                            {
                                Logger.Log("Load has been successful");
                                this.rootTask.Restart();
                                if (this.rootTask.WantToDoSomething())
                                {
                                    newActivity = this.rootTask.GetActivity();
                                    this.nothingToDoTimer.Reset();
                                }
                            }
                        }
                    }
                    else if (newActivity.Task.GetType().ToString() == "Pather.Tasks.UnloadTask")
                    {
                        if (this.activity != null)
                        {
                            this.activity.Stop();
                            this.activity = null;
                        }
                        this.rootTask = this.taskQueue.Pop();
                        this.activity = this.activityQueue.Pop();
                        newActivity.Task.Restart();
                        newActivity = null;
                        if (this.rootTask.WantToDoSomething())
                        {
                            newActivity = this.rootTask.GetActivity();
                            this.nothingToDoTimer.Reset();
                        }
                    }
                }

                if (newActivity != this.activity)
                {
                    if (this.activity != null)
                    {
                        // change _activity before it was finished
                        this.activity.Stop();
                        Task tr = this.activity.Task;
                        while (tr != null)
                        {
                            tr.IsActive = false;
                            tr = tr.Parent;
                        }
                    }
                    this.activity = newActivity;
                    if (this.activity != null)
                    {
                        Task tr = this.activity.Task;
                        while (tr != null)
                        {
                            tr.IsActive = true;
                            tr = tr.Parent;
                        }
                    }
                    else
                    {
                        Logger.Log("Got a null activity");
                    }

                    Logger.Log("Current activity: {0}", this.activity);
                    
                    if (this.activity != null)
                    {
                        Logger.Log("Got a new activity: " + this.activity);
                        this.activity.Start();
                    }
                }
                if (newActivity == null)
                    this.activity = null;
            }
            if (this.activity == null)
            {
                if (this.nothingToDoTimer.IsReady())
                {
                    Logger.Log("Script ended. Stopping");
                    this.StopAll("Script ended - stopping");
                    return;
                }
            }
            else
                this.nothingToDoTimer.Reset();
            //form.SetTask(task); 

            if (this.activity != null)
            {
                bool done = this.activity.Do();
                this.nothingToDoTimer.Reset(); // did something
                if (done)
                {
                    this.activity.Task.ActivityDone(this.activity);
                    Task tr = this.activity.Task;
                    while (tr != null)
                    {
                        tr.IsActive = false;
                        tr = tr.Parent;
                    }
                    this.activity = null;
                }
            }

            this.tick.Reset();
        }

        public void StopAll(String reason)
        {
            try
            {
                Logger.Log("Botting stopped: " + reason);
                Engine.StopEngine();
            }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                Logger.Log("Exception in stop: " + ex);
            }
        }
    }
}