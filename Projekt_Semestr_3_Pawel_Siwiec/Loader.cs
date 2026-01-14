namespace LibraryManager;

public interface ILoader<T>
{
    Task<T> LoadAsync();
}
