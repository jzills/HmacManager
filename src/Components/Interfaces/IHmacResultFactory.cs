namespace HmacManager.Components;

public interface IHmacResultFactory
{
    HmacResult Success(Hmac hmac);
    HmacResult Failure();
}