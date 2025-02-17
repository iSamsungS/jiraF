﻿using jiraF.Goal.API.Domain.Dtos;
using jiraF.Goal.API.Exceptions;
using jiraF.Goal.API.ValueObjects;

namespace jiraF.Goal.API.Domain;

public class GoalModel
{
    public Title Title { get; set; }
    public Description Description { get; set; }
    public User Reporter { get; set; }
    public User Assignee { get; set; }
    public DateTime DateOfCreate { get; }
    public DateTime DateOfUpdate { get; set; }
    public LabelModel Label { get; private set; }

    public GoalModel(
        Title title,
        Description description,
        User reporter,
        User assignee,
        LabelModel label)
    {
        Title = title;
        Description = description;
        Reporter = reporter;
        Assignee = assignee;
        DateOfCreate = DateTime.UtcNow;
        DateOfUpdate = default(DateTime);
        Label = label;
    }

    public void EditLabel(IEnumerable<LabelModel> availableLabels, string label)
    {
        bool containsLabel = availableLabels
            .Where(x => x.Title.Value == label).Any();

        if (!containsLabel)
            throw new DomainException($"Not contains label: '{label}'.");

        Label = new LabelModel(new Title(label));
    }
}
