using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Customer.Contract;
using Xunit;

namespace Customer.Api.IntegrationTest
{
    public class CustomerControllerTests
    {
        private readonly HttpClient _client;

        public CustomerControllerTests()
        {
            var testServer = new TestServer(new WebHostBuilder()
                .UseStartup<TestStartup>()
                .UseEnvironment("Development"));
            _client = testServer.CreateClient();
        }


        [Theory]
        [InlineData("/api/customer", "/api/customer")]
        public async Task Insert_GetAll_Update_GetByCityCode_Should_Return_Expected_Result(string postUrl, string getUrl)
        {
            #region Insert
            var expectedResult = string.Empty;
            var expectedStatusCode = HttpStatusCode.OK;

            // Arrange-1
            var request = new CustomerDto
            {
                FullName = "Caner Tosuner",
                CityCode = "Ist",
                BirthDate = new DateTime(1990, 1, 1)
            };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            _client.DefaultRequestHeaders.Add("audity-user", "admin-abraham lincoln");
            // Act-1
            var response = await _client.PostAsync(postUrl, content);

            var actualStatusCode = response.StatusCode;
            var actualResult = await response.Content.ReadAsStringAsync();

            // Assert-1
            Assert.Equal(expectedResult, actualResult);
            Assert.Equal(expectedStatusCode, actualStatusCode);
            #endregion



            #region GetAll
            // Act-2
            var responseGet = await _client.GetAsync(getUrl);
            responseGet.EnsureSuccessStatusCode();

            var actualGetResult = await responseGet.Content.ReadAsStringAsync();
            var getResultList = JsonConvert.DeserializeObject<List<CustomerDto>>(actualGetResult);

            var insertedCustomerExist = getResultList.Any(c => c.CityCode == request.CityCode);

            // Assert-2
            Assert.NotEmpty(getResultList);
            Assert.True(insertedCustomerExist);
            Assert.Equal("admin-abraham lincoln", getResultList.Single(c => c.CityCode == request.CityCode).CreatedBy);
            #endregion



            #region Update
            // Arrange-3
            var insertedCustomer = getResultList.Single(c => c.CityCode == request.CityCode);
            var requestUpdate = new CustomerDto
            {
                FullName = "Ali Tosuner",
                CityCode = "Ist",
                BirthDate = new DateTime(1994, 1, 1),
                Id = insertedCustomer.Id
            };
            var contentUpdate = new StringContent(JsonConvert.SerializeObject(requestUpdate), Encoding.UTF8, "application/json");
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("audity-user", "super-admin nicola tesla");

            // Act-3
            var responseUpdate = await _client.PutAsync(postUrl, contentUpdate);
            responseUpdate.EnsureSuccessStatusCode();
            var updateActualResult = await responseUpdate.Content.ReadAsStringAsync();
            var obj = await JsonConvert.DeserializeObjectAsync<CustomerDto>(updateActualResult);
            // Assert-3
            Assert.Equal(obj.FullName, requestUpdate.FullName);
            Assert.Equal("super-admin nicola tesla", obj.ModifiedBy);
            #endregion



            #region GetByCityCode
            // Act-2
            var responseGetByCityCode = await _client.GetAsync("/api/customer/getbycitycode/" + requestUpdate.CityCode);
            responseGetByCityCode.EnsureSuccessStatusCode();

            var actualGetByCityCodeResult = await responseGetByCityCode.Content.ReadAsStringAsync();
            var getByCityCodeResultList = JsonConvert.DeserializeObject<List<CustomerDto>>(actualGetByCityCodeResult);

            var updatedCustomerExist = getByCityCodeResultList.Any(c => c.CityCode == request.CityCode);

            // Assert-2
            Assert.NotEmpty(getByCityCodeResultList);
            Assert.True(updatedCustomerExist);
            Assert.Equal("super-admin nicola tesla", getByCityCodeResultList.Single(c => c.CityCode == request.CityCode).ModifiedBy);
            #endregion

        }
    }
}
