﻿namespace UniSportUAQ_API.Data.Base
{
	public class PaginatedResponse<T>
	{
		public T? Data { get; set; }
		public int TotalCount { get; set; }
		public int PageNumber { get; set; }
		public int PageSize { get; set; }
	}

}
