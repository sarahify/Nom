namespace NomRentals.Api.Models
{
    public class ResponseDto<T>
    {
        public int StatusCode { get; set; }
        public string DisplayMessage { get; set; }
        public T Result { get; set; }
        public object ErrorMessages { get; set; }
    }
}
