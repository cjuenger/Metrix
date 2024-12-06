using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Io.Juenger.Scrum.GitLab.Services.Infrastructure
{
    internal interface IPaginationService
    {
        Task<IEnumerable<T>> BrowseAllAsync<T>(Func<int, Task<List<T>>> func);
        Task<IEnumerable<T>> BrowseToEndAsync<T>(int firstPage, Func<int, Task<List<T>>> func);
        Task<IEnumerable<T>> BrowseAsync<T>(int firstPage, int lastPage, Func<int, Task<List<T>>> func);
    }
}