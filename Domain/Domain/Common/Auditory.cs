namespace Domain
{
    public class Auditory : BaseDomainModel
    {
        public Guid IdEntity { get; private set; }

        public DateTime OperationDate { get; private set; }

        public string EntityName { get; private set; } = string.Empty;

        public OperationTypeEnum OperationType { get; private set; }

        public string? PreviousValue { get; private set; } = null;

        public string? CurrentValue { get; private set; } = null;

        public string? UserName { get; private set; } = null;
        private Auditory(Guid id, string entityName, Guid idEntity,
            DateTime operationDate, OperationTypeEnum operationType, string? previousValue, string? currentValue, string? userName) : base(id)
        {
            IdEntity = idEntity;
            EntityName = entityName;
            OperationDate = operationDate;
            OperationType = operationType;
            PreviousValue = previousValue;
            CurrentValue = currentValue;
            UserName = userName;
        }

        public static Auditory Create(string entityName,
                                        Guid idEntity,
                                        DateTime operationDate,
                                        OperationTypeEnum operationType,
                                        string? previousValue,
                                        string? currentValue,
                                        string? userName)
        {
            return new Auditory(Guid.NewGuid(),
                                entityName,
                                idEntity,
                                operationDate,
                                operationType,
                                previousValue,
                                currentValue,
                                userName);
        }
    }
}
