﻿using IssueTrackerApi.Models;
using Marten;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace IssueTrackerApi.Controllers;

[ApiController]
public class IssuesController : ControllerBase
{
    private readonly IDocumentStore _documentStore;

    public IssuesController(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }


    // GET /greeting

    [HttpGet("/greeting")]
    public async Task<ActionResult> GetTheGreeting()
    {
        return Ok("Nice to see you");
    }

    [HttpGet("/issues")]
    public async Task<ActionResult> GetIssues(CancellationToken ct, [FromQuery] string status = "All")
    {
        using var session = _documentStore.LightweightSession();
        IReadOnlyList<IssueResponse>? data = null;
        if (status == "All")
        {
            data = await session.Query<IssueResponse>().ToListAsync();
        }
        else
        {
            IssueStatus statusEnum;
            if (Enum.TryParse<IssueStatus>(status, true, out statusEnum))
            {
                data = await session.Query<IssueResponse>().Where(i => i.Status == statusEnum).ToListAsync(ct);
            }
            else
            {
                data = new List<IssueResponse>();
            }
        }
        return Ok(new { issues = data });
    }

    [HttpPost("/closed-issues")]
    public async Task<ActionResult> CloseTheIssue([FromBody] IssueResponse issue)
    {
        using var session = _documentStore.LightweightSession();
        var savedIssue = await session.Query<IssueResponse>()
            .Where(i => i.Id == issue.Id && i.Status == IssueStatus.Open)
            .SingleOrDefaultAsync();
        if (savedIssue is null)
        {
            return BadRequest("We don't have that issue");
        }
        else
        {
            savedIssue.Status = IssueStatus.Closed;
            session.Store(savedIssue);
            await session.SaveChangesAsync();
            return Accepted();
        }
    }

    [HttpGet("/issues/{issueId}")]
    public async Task<ActionResult> GetIssueById(Guid issueId)
    {
        using var session = _documentStore.LightweightSession();
        var data = await session.Query<IssueResponse>()
            .Where(i => i.Id == issueId)
            .SingleOrDefaultAsync();

        if(data is not null)
        {
            return Ok(data);
        }
        else
        {
            return NotFound();
        }
    }

    [HttpPost("/issues")]
    public async Task<ActionResult> AddIssue([FromBody] IssueCreateRequest request)
    {
        // Save it to a database

        var issue = new IssueResponse
        {
            Id = Guid.NewGuid(),
            Application = request.Application,
            ContactEmail = request.ContactEmail,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            Status = IssueStatus.Open
        };

        using var session = _documentStore.LightweightSession();
        session.Insert(issue);
        await session.SaveChangesAsync();

        return Ok(issue);
    }
}
