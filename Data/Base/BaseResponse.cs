namespace UniSportUAQ_API.Data.Base
{
    public class BaseResponse<T>
    {
        public bool? Ok { get => string.IsNullOrEmpty(Message); }
        public T? Data { get; set; }
        public string? Message { get => Error?.Description; }
        public ErrorBase? Error { get; set; }
    }
}
