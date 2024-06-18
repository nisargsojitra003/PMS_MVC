﻿namespace PMS_MVC.Models
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public bool hasPrevious { get; set; } = false;
        public bool hasNext { get; set; } = false;

        public PagedList(IEnumerable<T> currentPage, int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            PageSize = pageSize;
            TotalCount = count;
            if (pageNumber > 1)
            {
                hasPrevious = true;
            }

            if (pageNumber < TotalPages)
            {
                hasNext = true;
            }

            AddRange(currentPage);
        }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int count, int pageNumber, int pageSize)
        {

            return new PagedList<T>(source, count, pageNumber, pageSize);
        }
    }
}
