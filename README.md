# golang-channel-in-csharp

Simple yet thread-safe implementation of golang channels in C#. This hopefully will teach C# developers how golang channels behave.

```c#
public interface IChannel<T> : IDisposable
{
    /// <summary>
    /// Send an item to the channel
    /// </summary>
    /// <param name="t"></param>
    /// <exception cref="InvalidOperationException">if adding to the channel that is closed</exception>
    /// <returns></returns>
    bool Send(T t);

    /// <summary>
    /// Takes an item from the channel
    /// </summary>
    /// <param name="val"></param>
    /// <exception cref="InvalidOperationException">if queue is empty or disposed</exception>
    /// <returns></returns>
    bool Receive(out T val);

    /// <summary>
    /// Closes any addition to the channel
    /// </summary>
    void Close();

    /// <summary>
    /// Iterates over the channel
    /// </summary>
    /// <returns></returns>
    IEnumerable<T> Range();
}
```
