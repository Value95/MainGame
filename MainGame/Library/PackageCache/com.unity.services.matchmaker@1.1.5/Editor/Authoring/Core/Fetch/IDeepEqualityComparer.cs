namespace Unity.Services.Matchmaker.Authoring.Core.Fetch
{
    interface IDeepEqualityComparer
    {
        bool IsDeepEqual<T>(T source, T target);
    }
}
