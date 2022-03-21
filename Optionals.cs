namespace ParserTools.Optionals;


public struct Optional<T> where T : struct
{
    T? value;

    public static implicit operator bool(Optional<T> val)
    {
        return val.value is not null;
    }

    public static implicit operator Optional<T>(T? val)
    {
        return new Optional<T>(val);
    }

    public T Value
    {
        get
        {
            return this.value.GetValueOrDefault();
        }

        set
        {
            this.value = value;
        }
    }


    public Optional<T> IfNotNull(Action<Optional<T>> func)
    {
        func(this);
        return this;
    }


    public Optional(T? value)
    {
        this.value = value;
    }
}




public class Result<T, E>
{
    public static Result<T, E> Create(T val)
    {
        return new Success(val);
    }    
    
    public static Result<T, E> Create(E val)
    {
        return new Error(val);
    }


    public static implicit operator bool(Result<T, E> val)
    {
        return val switch
        {
            Success => true,
            Error => false,
            _ => false
        };
    }
    

    public void OnResult(Action<Success> onSuccess, Action<Error>? onFailure = null)
    {
        if (this )
        {
            onSuccess((Success)this);
        }

        else
        {
            if(onFailure is not null) onFailure((Error)this);
        }
    }


    public T GetResult()
    {
        return ((Success)this).Payload;
    }

    public E GetError()
    {
        return ((Error)this).Payload;
    }

    public class Success: Result<T, E>
    {
        public T Payload { get; }
        public Success(T payload) => Payload = payload;
        public void Deconstruct(out T payload) => payload = Payload;
    }

    public class Error: Result<T, E>
    {
        public E Payload { get; }
        public Error(E payload) => Payload = payload;
        public void Deconstruct(out E payload) => payload = Payload;
    }


}



