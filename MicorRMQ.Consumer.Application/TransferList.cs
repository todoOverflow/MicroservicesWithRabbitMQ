using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MicorRMQ.Consumer.Domain;
using MicorRMQ.Consumer.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MicorRMQ.Consumer.Application
{
    public class TransferList
    {
        public class Query : IRequest<List<TransferLog>>
        {

        }

        public class Hanlder : IRequestHandler<Query, List<TransferLog>>
        {
            private readonly DataContext _dataContext;
            public Hanlder(DataContext dataContext)
            {
                this._dataContext = dataContext;
            }

            public async Task<List<TransferLog>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _dataContext.TransferLogs.ToListAsync();
            }
        }
    }
}