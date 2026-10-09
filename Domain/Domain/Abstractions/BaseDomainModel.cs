namespace Domain
{
    public abstract class BaseDomainModel
    {
        public Guid Id { get; init; }
        public string CreatedBy { get; private set; } = string.Empty;
        public DateTime CreatedDate { get;  set; }
        public string? UpdatedBy { get;  set; }
        public DateTime? UpdatedDate { get;  set; }
        public bool IsActive { get;  set; }

        private readonly List<IDomainEvent> domainEvents = [];
        protected BaseDomainModel(Guid id)
        {
            Id = id;
        }
        protected BaseDomainModel()
        {
        }
        public IReadOnlyList<IDomainEvent> GetDomainEvents()
        {
            return domainEvents.ToList();
        }
        public void ClearDomainEvents()
        {
            domainEvents.Clear();
        }
        protected void RaiseDomainEvents(IDomainEvent domainEvent)
        {
            domainEvents.Add(domainEvent);
        }
        public void SetCreatedAuditory(string createdBy, DateTime createdDate)
        {
            CreatedBy = createdBy;
            CreatedDate = createdDate;
        }
        public void SetUpdatedAuditory(string updatedBy, DateTime updatedDate)
        {
            UpdatedBy = updatedBy;
            UpdatedDate = updatedDate;
        }
        public virtual void SetIsActive(bool isActive)
        {
            IsActive = isActive;
        }
        public virtual void Disable()
        {
            IsActive = false;
        }
        public virtual void Enable()
        {
            IsActive = true;
        }
    }
}
