using System.Collections.Generic;
using System.Linq;
using System.Text;
using core;
using core.Account;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace web.Controllers
{
    [ApiController]
    [Authorize("admin")]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private IMediator _mediator;
        private IAccountStorage _storage;
        private IPortfolioStorage _portfolio;

        public AdminController(
            IMediator mediator,
            IAccountStorage storage,
            IPortfolioStorage portfolio)
        {
            _mediator = mediator;
            _storage = storage;
            _portfolio = portfolio;
        }

        [HttpGet("users")]
        public async System.Threading.Tasks.Task<ActionResult> ActiveAccountsAsync()
        {
            var users = await _storage.GetUsers();

            var sb = new StringBuilder();

            sb.Append("<html><body><table><tr><th>Email</th><th>Last Login</th><th>Stocks</th><th>Options</th><th>Notes</th></tr>");

            foreach(var (email,userId) in users)
            {
                sb.Append($"<tr>");
                sb.Append($"<td>{email}</td>");

                var guid = new System.Guid(userId);

                var user = await _storage.GetUser(guid);

                sb.Append($"<td>{user?.LastLogin?.ToString()}</td>");
                
                var options = await _portfolio.GetOwnedOptions(guid);
                var notes = await _portfolio.GetNotes(guid);
                var stocks = await _portfolio.GetStocks(guid);

                sb.Append($"<td>{stocks.Count()}</td>");
                sb.Append($"<td>{options.Count()}</td>");
                sb.Append($"<td>{notes.Count()}</td>");

                sb.Append("</tr>");
            }

            sb.Append("<table></body></html>");

            return new ContentResult {
                Content = sb.ToString(),
                ContentType = "text/html"
            };
        }
    }
}