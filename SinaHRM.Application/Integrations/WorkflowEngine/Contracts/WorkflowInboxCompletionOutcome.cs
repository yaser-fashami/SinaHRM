namespace SinaHRM.Application.Integrations.WorkflowEngine.Contracts;

public enum WorkflowInboxCompletionOutcome
{
    Processed = 1,
    AlreadyCompleted = 2,
    InvalidContractState = 3,
    LeaseLost = 4
}
