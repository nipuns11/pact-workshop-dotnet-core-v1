using System;
using System.Collections.Generic;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using PactNet;
using PactNet.Infrastructure.Outputters;
using tests.XUnitHelpers;
using Xunit;
using Xunit.Abstractions;

namespace tests
{
    public class ProviderApiTests
    {

        private string _providerUri { get; }
        private string _pactServiceUri { get; }
        private IWebHost _webHost { get; }

        private ITestOutputHelper _outputhelper { get; }
        public ProviderApiTests(ITestOutpurHelper output)
        {
            _outputhelper = output;
            _providerUri = "http://localhost:9000";
            _pactServiceUri = "http://localhost:9001";
            _webHost = _webHost.CreateDefaultBuilder()
            .UseUrls(_pactServiceUri)
            .UseStartUp<TestStartup>()
            .Build();
            _webHost.Start();
        }

        [Fact]
        public void EnsureProviderApiHonoursPactWithConsumer()
        {

            //Arrange
            var config = new PactVerifierConfig
            {
                Outputters = new List<IOutput>
                {
                  new XUnitOuput(_outputhelper)
                },
                Verbose = true
            };

            //Act/Assert
            IPactVerifier pactVerifier = new PactVerifier(config);
            pactVerifier.ProviderState($"{_pactServiceUri}/provider-states")
            .ServiceProvider("Provider", _providerUri)
            .HonoursPactWith("Consumer")
            .PactUri(@"..\..\..\..\..\pacts\consumer-provider.json")
            .Verify();

        }

        #region IDisposable Support
        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _webHost.StopAsync().GetAwaiter().GetResult();
                    _webHost.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}

