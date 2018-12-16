using System.Threading.Tasks;

namespace SmartDormitory.Services.HttpClients
{
    public interface IIcbHttpClient
    {
        Task<string> FetchAllSensors();
        Task<string> FetchSensorById(string id);
    }
}