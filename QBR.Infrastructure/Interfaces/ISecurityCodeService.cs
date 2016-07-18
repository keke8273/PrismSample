namespace QBR.Infrastructure.Interfaces
{
    public interface ISecurityCodeService
    {
        string CalculateSecurityCode(string fileContent);
    }
}