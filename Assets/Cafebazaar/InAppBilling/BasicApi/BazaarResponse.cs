public abstract class BazaarResponse
{
    protected BazaarResponse(string message, bool successful)
    {
        Message = message;
        Successful = successful;
    }

    protected BazaarResponse(bool successful) : this(null, successful) { }
    private string _message;

    public bool Successful { get; private set; }

    public string Message
    {
        get
        {
            return _message;
        }
        private set { _message = value; }
    }
}

public class BazaarResponse<T> : BazaarResponse where T : class
{
    public BazaarResponse(string message, T body, bool successful) : base(message, successful)
    {
        Body = body;
    }

    public BazaarResponse(T body, bool successful) : this(null, body, successful) { }

    public T Body { get; private set; }


    public static BazaarResponse<T> Error(string message)
    {
        return new BazaarResponse<T>(message, null, false);
    }

    public static BazaarResponse<T> Success(T body)
    {
        return new BazaarResponse<T>(body, true);
    }
}

