namespace Brimborium.Extensions.RequestPipe {
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;

    public class RequestExecutionService : IRequestExecutionService {
        //public Task<TResponce> ExecuteAsync<TRequest, TResponce>(TRequest request) 
        //    where TRequest : IRequest<TResponce> {
        //    throw new NotImplementedException();
        //}
        public virtual Task<TResponce> ExecuteAsync<TResponce>(IRequest<TResponce> request) {
            throw new NotImplementedException();
        }
    }
}
