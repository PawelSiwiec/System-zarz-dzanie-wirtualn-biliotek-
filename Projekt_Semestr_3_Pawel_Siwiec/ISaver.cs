namespace LibraryManager;

public interface ISaver<T>
{
    Task SaveAsync(T data);
}
