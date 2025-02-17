﻿using jiraF.Goal.API.Contracts;
using jiraF.Goal.API.Domain;
using jiraF.Goal.API.Domain.Dtos;
using jiraF.Goal.API.Infrastructure.Data.Contexts;
using jiraF.Goal.API.Infrastructure.Data.Entities;
using jiraF.Goal.API.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace jiraF.Goal.API.Infrastructure.Data.Repositories;

public class GoalRepository : IGoalRepository
{
    private readonly AppDbContext _dbContext;

    public GoalRepository(
        AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<GoalModel>> GetAsync()
    {
        return await _dbContext.Goals
            .Select(x => new GoalModel(
                new Title(x.Title),
                new Description(x.Description),
                new User(),
                new User(),
                new LabelModel(new Title(x.LabelId.ToString()))))
            .ToListAsync();
    }

    public async Task<GoalModel> GetByIdAsync(Guid id)
    {
        return await _dbContext.Goals
            .Where(x => x.Id == id)
            .Select(x => new GoalModel(
                new Title(x.Title),
                new Description(x.Description),
                new User(),
                new User(),
                new LabelModel(new Title(x.LabelId.ToString()))))
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(GoalModel model)
    {
        Guid labelId = await GetLabelIdByTitle(model.Label.Title.Value);
        _dbContext.Goals.Add(new GoalEntity
        {
            Title = model.Title.Value,
            AssigneeId = model.Assignee.Number,
            ReporterId = model.Reporter.Number,
            LabelId = labelId,
            Description = model.Description.Value,
            DateOfCreate = model.DateOfCreate,
        });
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Guid id, GoalModel model)
    {
        Guid labelId = await GetLabelIdByTitle(model.Label.Title.Value);
        GoalEntity entity = await _dbContext.Goals.FirstOrDefaultAsync(x => x.Id == id);
        entity.Title = model.Title.Value;
        entity.AssigneeId = model.Assignee.Number;
        entity.ReporterId = model.Reporter.Number;
        entity.DateOfCreate = model.DateOfCreate;
        entity.DateOfUpdate = model.DateOfUpdate;
        entity.Description = model.Description.Value;
        entity.LabelId = labelId;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteByIdAsync(Guid id)
    {
        GoalEntity entity = await _dbContext.Goals.FirstOrDefaultAsync(x => x.Id == id);
        _dbContext.Goals.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }


    private async Task<Guid> GetLabelIdByTitle(string title)
    {
        return await _dbContext.Labels
            .Where(x => x.Title == title)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();
    }
}
