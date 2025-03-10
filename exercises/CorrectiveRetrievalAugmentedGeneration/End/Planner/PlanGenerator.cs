﻿using Microsoft.Extensions.AI;
using StructuredPrediction;

namespace Planner;

public class PlanGenerator(IChatClient chatClient)
{
    public async Task<Plan> GeneratePlanAsync(string task, CancellationToken cancellationToken = default)
    {
        List<ChatMessage> messages = [
            new(ChatRole.System,
                """
                For the given objective, come up with a simple step by step plan.
                This plan should involve individual tasks, that if executed correctly will yield the correct answer. Do not add any superfluous steps.
                The result of the final step should be the final answer. Make sure that each step has all the information needed - do not skip steps.
                """),
            new(ChatRole.User, task)];

        var result = await chatClient.CompleteAsync<Plan>(messages, cancellationToken: cancellationToken);

        if (!result.TryGetResult(out var plan))
        {
            throw new InvalidOperationException("No plan generated");
        }

        return plan;
    }
}
