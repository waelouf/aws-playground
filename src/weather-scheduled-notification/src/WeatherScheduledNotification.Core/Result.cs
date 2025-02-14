namespace WeatherScheduledNotification.Core;

public readonly struct Result<TValue, TError>
{
    private readonly TValue? _value;
    private readonly TError? _error;

    private Result(TValue value)
    {
        _value = value;
        IsError = false;
        _error = default;
    }

    private Result(TError error)
    {
        _error = error;
        IsError = true;
        _value = default;
    }

    public bool IsError { get; }

    public bool IsSuccess => !IsError;

    public static implicit operator Result<TValue, TError>(TValue value) => new(value);
        
    public static implicit operator Result<TValue, TError>(TError error) => new(error);

    public TResult Match<TResult>(
        Func<TValue, TResult> success,
        Func<TError, TResult> failure) => 
        !IsError ? success(_value!) : failure(_error!);
}
