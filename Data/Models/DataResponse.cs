namespace UniSportUAQ_API.Data.Models
{
	public class DataResponse
	{
		public object? Data { get; set; }
		public string? ErrorMessage { get; set; }
		public bool? HasError { get { return string.IsNullOrEmpty(ErrorMessage); }  }
	}
}
