using System.Text;

namespace Unit.Tests;

public abstract class TestBase
{
    protected static readonly Guid PublicKey = Guid.NewGuid();
    protected static readonly string PrivateKey = 
        Convert.ToBase64String(
            Encoding.UTF8.GetBytes("thisIsMySuperCoolPrivateKey")); 

    protected static readonly Guid PublicKey2 = Guid.NewGuid();
    protected static readonly string PrivateKey2 = 
        Convert.ToBase64String(
            Encoding.UTF8.GetBytes("thisIsMyOtherSuperCoolPrivateKey")); 
}