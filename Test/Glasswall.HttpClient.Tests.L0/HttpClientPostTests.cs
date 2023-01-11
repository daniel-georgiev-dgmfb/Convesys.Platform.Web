using Convesys.HttpClient.Tests.L0.MockData;
using Convesys.Kernel.Web;
using Convesys.Platform.Web.HttpClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Convesys.HttpClient.Tests.L0
{
    internal class Foo
    {
        public double D1 { get; set; }
        public double D2 { get; set; }
    }

    [TestFixture]
    [Category("Convesys.HttpClient.Tests.L0")]
    public class HttpClientPostTests
    {
        [Test]
        public async Task KSTest_test()
        {
            //ARRANGE
            try
            {
                var configuration = new ResourceRetrieverCustomConfigurator();
                var configuration1 = new Convesys.Platform.Web.HttpClient.CertificateValidationConfigurationProvider();
                var loggerValidator = new Providers.Logging.Microsoft.Logger<Convesys.Platform.Web.HttpClient.BackchannelCertificateValidator>(new Logger());
                //var loggerValidator = new Mock<IEventLogger<Convesys.Platform.Web.HttpClient.BackchannelCertificateValidator>>();
                ;
                var httpLogger = new Providers.Logging.Microsoft.Logger<Convesys.Platform.Web.HttpClient.HttpClient>(new LoggerFactory());
                //var logger = new Mock<IEventLogger<Convesys.Platform.Web.HttpClient.HttpClient>>();
                var backchannelValidator = new Convesys.Platform.Web.HttpClient.BackchannelCertificateValidator(configuration1, loggerValidator);

                //var url = "http://localhost:4449/stats/tests/kstest";
                var url = "https://localhost:44331/api/Stats";

                //var message = "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,100,101,101";
                var message = "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,100,101,101";

                var httpClient = new Convesys.Platform.Web.HttpClient.HttpClient(backchannelValidator, httpLogger);
                httpClient.HttpDocumentRetrieverConfigurator = configuration;
                httpClient.RequireHttps = false;
                //ACT


                IResourceRetriever resourceRetriever = httpClient;
                var dir = new Dictionary<string, string>();
                dir.Add("readings", message);
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(url),
                    Content = new FormUrlEncodedContent(dir)
                };
                //request.Headers.Add("Content-Type", "json/xml");
                var result = await resourceRetriever.SendAsync(request, CancellationToken.None);

                var dataAsString = await result.Content.ReadAsStringAsync();
                var dataAsStream = await result.Content.ReadAsByteArrayAsync();
                var split = dataAsString.Trim('[').TrimEnd(']').Split(',');
                if (split.Length != 2)
                    throw new FormatException("length");
                var value1 = double.Parse(split[0].Trim());
                var value2 = double.Parse(split[1].Trim());
                var testResult = new Tuple<double, double>(value1, value2);
            }
            //ASSERT
            catch (Exception ex)
            {
                throw;
            }
        }

        [Test]
        public async Task mode_test()
        {
            //ARRANGE
            var configuration = new ResourceRetrieverCustomConfigurator();
            var configuration1 = new CertificateValidationConfigurationProvider();
            //var loggerValidator = new Mock<IEventLogger<Convesys.Platform.Web.HttpClient.BackchannelCertificateValidator>>();
            //var logger = new Mock<IEventLogger<Convesys.Platform.Web.HttpClient.HttpClient>>();
            var logger = new HttpClientLogger();
            var validatorLogger = new BackchanelLogger();
            var backchannelValidator = new Convesys.Platform.Web.HttpClient.BackchannelCertificateValidator(configuration1, validatorLogger);

            var url = "http://localhost:4449/stats/funcs/mode";

            var message = "1,2,3,4,5,6,7,8,9,10,10,10,11,12,13,14,15,16,17,18,19,20,21,100,101,101";
            var message1 = "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,101,101,101,101,101,101,101,101,101,101,101,101,101,101,102,102,102,102,102,102,102,102,102,102,102,102,102,102,102";
            var httpClient = new Convesys.Platform.Web.HttpClient.HttpClient(backchannelValidator, logger);
            httpClient.HttpDocumentRetrieverConfigurator = configuration;
            httpClient.RequireHttps = false;
            //ACT
            try
            {
                IResourceRetriever resourceRetriever = httpClient;
                var dir = new Dictionary<string, string>();
                //dir.Add("readings", message);
                dir.Add("readings", message1);
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(url),
                    Content = new FormUrlEncodedContent(dir)
                };
                var result = await resourceRetriever.SendAsync(request, CancellationToken.None);

                var dataAsString = await result.Content.ReadAsStringAsync();
                var dataAsStream = await result.Content.ReadAsByteArrayAsync();
                var ser = new JsonSerializer();
                var reader = new StringReader(dataAsString);
                //var ms = new MemoryStream(dataAsStream);
                Newtonsoft.Json.Linq.JObject sr = (Newtonsoft.Json.Linq.JObject)ser.Deserialize(new JsonTextReader(reader));
                var type = sr.GetType();
                var nodes = sr.Children();
                //var coll = nodes.Select(item => 
                //                Tuple.Create(((Newtonsoft.Json.Linq.JProperty)item).Name, 
                //                ((Newtonsoft.Json.Linq.JProperty)item).Value));
                var coll = nodes.Select(item =>
                                new Foo
                                {
                                    D1 = Double.Parse(((Newtonsoft.Json.Linq.JProperty)item).Name),
                                    D2 = Double.Parse(((Newtonsoft.Json.Linq.JProperty)item).Value.ToString())
                                });
                //Tuple.Create<double, double>(Double.Parse(((Newtonsoft.Json.Linq.JProperty)item).ToString()), 10.0));
                //foreach (Newtonsoft.Json.Linq.JProperty item in nodes)
                //{
                //    var ty = item.GetType();
                //    var tup = Tuple.Create<double,double>(Double.Parse(item.Name), ((double)item.Value));
                //    var n = item.ToString(Formatting.Indented);
                //    var foo = new Foo
                //    {
                //        D1 = tup.Item1,
                //        D2 = tup.Item2
                //    };
                //}
                
                //var f = ser.Deserialize<Foo>(new JsonTextReader(reader));
                
                //var split = dataAsString.Trim('[').TrimEnd(']').Trim('{').Trim('}').Trim('\'').Trim('\"').Split(':');
                
                //var value1 = double.Parse(split[0].Trim());
                //var value2 = double.Parse(split[1].Trim());
                //var testResult = new Tuple<double, double>(value1, value2);
            }
            //ASSERT
            catch (Exception ex)
            {

            }
        }
    }
}