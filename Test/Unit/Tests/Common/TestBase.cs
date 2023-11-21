using System.Text;

namespace Unit.Tests;

public abstract class TestBase
{
    protected static readonly Guid PublicKey = Guid.NewGuid();
    protected static readonly string PrivateKey = 
        Convert.ToBase64String(
            Encoding.UTF8.GetBytes("thisIsMySuperCoolPrivateKey")); 
}