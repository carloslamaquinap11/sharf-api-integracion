namespace Application
{
    public class ResponseModel<T>
    {
        public int StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public T? Result { get; set; }
    }
}
