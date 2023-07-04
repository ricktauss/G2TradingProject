using Consul;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ConsulKeyValueService.Services
{
    public class KeyValueService
    {

        private Uri _consulServerUrl;
        private readonly ConsulClient _consulClient;



        public KeyValueService(IConfiguration configuration)
        {
            _consulServerUrl = new Uri(configuration.GetValue<string>("ServiceDiscovery:Url"));
            _consulClient = new ConsulClient(c => c.Address = _consulServerUrl);

        }


        public async Task<ServiceResult<string>> GetKeyValue (string key)
        {
            try
            {

            var getPair = await _consulClient.KV.Get(key);

            if(getPair.Response == null)
            {
                    return new ServiceResult<string>
                    {
                        Success = false,
                        ErrorMessage = "Key not found in Consul!"
                    };
            }

            var secretValue = Encoding.UTF8.GetString(getPair.Response.Value, 0, getPair.Response.Value.Length);

                return new ServiceResult<string>
                {
                    Success = true,
                    Data = secretValue
                };

            }
            catch (Exception ex)
            {
                return new ServiceResult<string>
                {
                    Success = false,
                    ErrorMessage = $"An error occured while trying to get the KeyValue pair!: {ex.Message}"
                };
            }

        }



        public async Task<ServiceResult<string>> SetKeyValue (string key, string value)
        {
            try
            {

            var putPair = new KVPair(key);
            putPair.Value = Encoding.UTF8.GetBytes(value);

            var putAttempt = await _consulClient.KV.Put(putPair);

            if (!putAttempt.Response)
            {
                    return new ServiceResult<string>
                    {
                        Success = false,
                        ErrorMessage = "Failed to add Key Value Pair to Consul!"
                    };
             }

                return new ServiceResult<string>
                {
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<string>
                {
                    Success = false,
                    ErrorMessage = $"An error occured while trying to set the keyValue pair!: {ex.Message}"
                };
            }
        }

    }
}
