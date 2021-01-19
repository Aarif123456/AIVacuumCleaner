/* Action classes will hold what actions can be done by agent such as cleaning or moving to blocks */
namespace GameBrains.Actions
{
    public enum CompletionsStates { Complete, InProgress, Failed }

    public class Action
    {
        public CompletionsStates completionStatus;
        public float timeToLive;
    }
}