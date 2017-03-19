using System.Drawing;
using System.Windows.Forms;
using TaskEngine.Parser;
using TaskEngine.Tasks;

namespace TaskEngine.Helpers
{
    public static class TaskCreator
    {
        public static Task CreateTaskFromNode(NodeTask node, Task parent)
        {
            Task n = ParserTask.GetTask(node);
            if (n != null)
            {
                n.Parent = parent;
            }
            else
            {
                Logger.Log("!Warning:Unknown task type: " + node.Type);
                return null;
            }
            return n;
        }

        public static TreeNode CreateTreeFromTasks(Task rootTask)
        {
            return CreateNodeFromTask(rootTask);
        }

        public static TreeNode CreateNodeFromTask(Task task)
        {
            if (task == null)
                return new TreeNode();
            Task[] children = task.GetChildren();
            TreeNode[] childNodes = null;
            if (children != null)
            {
                childNodes = new TreeNode[children.Length];
                for (int i = 0; i < children.Length; i++)
                {
                    childNodes[i] = CreateNodeFromTask(children[i]);
                }
            }

            TreeNode n;
            if (childNodes != null)
                n = new TreeNode(task.ToString(), childNodes);
            else
                n = new TreeNode(task.ToString());
            n.Tag = task;
            return n;
        }

        /// <summary>
        /// Updates the tree node status.
        /// </summary>
        /// <param name="n">The n.</param>
        public static void UpdateTreeNodeStatus(TreeNode n)
        {
            Task t = (Task)n.Tag;

            if (t != null)
            {
                Task.TaskState state = t.State;
                if (state == Task.TaskState.Idle)
                    n.BackColor = Color.White;
                else if (state == Task.TaskState.Done)
                    n.BackColor = Color.Blue;
                else if (state == Task.TaskState.Active)
                    n.BackColor = Color.Green;
                else if (state == Task.TaskState.Want)
                    n.BackColor = Color.Yellow;
            }

            TreeNode child = n.FirstNode;
            while (child != null)
            {
                UpdateTreeNodeStatus(child);
                child = child.NextNode;
            }
        }
    }
}
