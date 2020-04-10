using MediatR;
using MicroRMQ.Producer.Domain;
using MicroRMQ.Producer.Persistence;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MicroRMQ.Producer.Application
{
    public class AccountList
    {
        public class Query : IRequest<List<Account>>
        {

        }

        public class Handler : IRequestHandler<Query, List<Account>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                this._context = context;
            }

            public async Task<List<Account>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.Accounts.ToListAsync();
            }
        }
    }
}