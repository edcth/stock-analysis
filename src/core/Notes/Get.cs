using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace core.Notes
{
    public class Get
    {
        public class Query : IRequest<object>
        {
            public Query(string userId, string noteId)
            {
                this.UserId = userId;
                this.NoteId = noteId;
            }

            public string UserId { get; }
            public string NoteId { get; }
        }

        public class Handler : HandlerWithStorage<Query, object>
        {
            public Handler(IPortfolioStorage storage) : base(storage)
            {
            }

            public override async Task<object> Handle(Query request, CancellationToken cancellationToken)
            {
                var notes = await _storage.GetNotes(request.UserId);

                return notes.SingleOrDefault(n => n.State.Id == request.NoteId)?.State;
            }
        }
    }
}