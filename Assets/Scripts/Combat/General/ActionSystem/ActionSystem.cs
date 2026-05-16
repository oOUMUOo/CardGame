using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSystem : Singleton<ActionSystem>
{
    private List<GameAction> reactions = null;
    public bool IsPerforming {get; private set;} = false;
    private static Dictionary<Type, List<Action<GameAction>>> preSubs = new();
    private static Dictionary<Type, List<Action<GameAction>>> postSubs = new();
    private static Dictionary<Type, Func<GameAction, IEnumerator>> performers = new();
    private static Dictionary<Type, Dictionary<Delegate, Action<GameAction>>> preWrappers = new();
    private static Dictionary<Type, Dictionary<Delegate, Action<GameAction>>> postWrappers = new();
    
    public void Perform(GameAction action, System.Action OnPerformFinished = null)
    {
        if (IsPerforming) return;
        IsPerforming = true;
        StartCoroutine(Flow(action, () =>
        {
            IsPerforming = false;
            OnPerformFinished?.Invoke();
        }));
    }

    public void AddReaction(GameAction gameAction)
    {
        reactions?.Add(gameAction);
    }

    private IEnumerator Flow(GameAction action, Action OnFlowFinished = null)
    {
        reactions = action.PreReactions;
        PerformSubscribers(action, preSubs);
        yield return PerformReactions();

        reactions = action.PerformReactions;
        yield return PerformPerformer(action);
        yield return PerformReactions();

        reactions = action.PostReactions;
        PerformSubscribers(action, postSubs);
        yield return PerformReactions();

        OnFlowFinished?.Invoke();
    }

    private IEnumerator PerformPerformer(GameAction action)
    {
        Type type = action.GetType();
        if (performers.ContainsKey(type))
        {
            yield return performers[type](action);
        }
    }

    private void PerformSubscribers(GameAction action, Dictionary<Type, List<Action<GameAction>>> subs)
    {
        Type type = action.GetType();
        if (subs.ContainsKey(type))
        {
            foreach (var sub in subs[type])
            {
                sub(action);
            }
        }
    }

    private IEnumerator PerformReactions()
    {
        foreach (var reaction in reactions)
        {
            yield return Flow(reaction);
        }
    }

    public static void AttachPerformer<T>(Func<T, IEnumerator> performer) where T : GameAction
    {
        Type type = typeof(T);
        IEnumerator wrappedPerformer(GameAction action) => performer((T)action);
        if (performers.ContainsKey(type)) performers[type] = wrappedPerformer;
        else performers.Add(type, wrappedPerformer);
    }

    public static void DetachPerformer<T>() where T : GameAction
    {
        Type type = typeof(T);
        if (performers.ContainsKey(type)) performers.Remove(type);
    }

    public static void SubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
    {
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs;
        Dictionary<Type, Dictionary<Delegate, Action<GameAction>>> wrappers = timing == ReactionTiming.PRE ? preWrappers : postWrappers;
        Type type = typeof(T);

        if (!subs.ContainsKey(type))
        {
            subs.Add(type, new());
        }

        if (!wrappers.ContainsKey(type))
        {
            wrappers.Add(type, new());
        }

        if (wrappers[type].ContainsKey(reaction))
        {
            return;
        }

        void wrappedReaction(GameAction action) => reaction((T)action);
        wrappers[type].Add(reaction, wrappedReaction);
        subs[type].Add(wrappedReaction);
    }

    public static void UnsubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
    {
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs;
        Dictionary<Type, Dictionary<Delegate, Action<GameAction>>> wrappers = timing == ReactionTiming.PRE ? preWrappers : postWrappers;
        Type type = typeof(T);

        if (!subs.ContainsKey(type) || !wrappers.ContainsKey(type))
        {
            return;
        }

        if (!wrappers[type].ContainsKey(reaction))
        {
            return;
        }

        Action<GameAction> wrappedReaction = wrappers[type][reaction];
        subs[type].Remove(wrappedReaction);
        wrappers[type].Remove(reaction);

        if (subs[type].Count == 0)
        {
            subs.Remove(type);
        }

        if (wrappers[type].Count == 0)
        {
            wrappers.Remove(type);
        }
    }

}
